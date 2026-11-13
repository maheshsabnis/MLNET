using Microsoft.ML;
using Microsoft.ML.Data;
using System.Text.Json;

namespace mllib
{
    /// <summary>
    /// Trains an ML.NET model from an in-memory sequence of objects. The feature columns are prepared
    /// automatically (text is one-hot encoded, other numeric/bool types are converted to Single) and the
    /// trainer is chosen from the type of the label column.
    /// </summary>
    public class DataTrainer
    {
        /// <summary>
        /// Trains a model and returns the fitted transformer pipeline.
        /// </summary>
        /// <remarks>
        /// Trainer selection by label column type:
        /// <list type="bullet">
        /// <item><description>Boolean: binary classification (SDCA logistic regression). Predict with a class property mapped to <c>PredictedLabel</c> (bool), plus optional <c>Probability</c> / <c>Score</c>.</description></item>
        /// <item><description>Numeric (Single, Double, Int32, ...): regression (SDCA). Predict with a float property mapped to <c>Score</c>.</description></item>
        /// <item><description>Text: multiclass classification (SDCA maximum entropy). Predict with a string property mapped to <c>PredictedLabel</c>.</description></item>
        /// </list>
        /// Feature columns of type string are one-hot encoded before being concatenated into <c>Features</c>.
        /// </remarks>
        /// <typeparam name="TData">The input data type mapped to the ML.NET schema.</typeparam>
        /// <param name="mlc">The MLContext to use.</param>
        /// <param name="database">The data records used to build the training data view.</param>
        /// <param name="outputColumnName">The label column (the value to predict).</param>
        /// <param name="featureColumnNames">The source columns to use as features.</param>
        /// <returns>An <see cref="ITransformer"/> representing the trained pipeline.</returns>
        public ITransformer Train<TData>(MLContext mlc, IEnumerable<TData> database, string outputColumnName, params string[] featureColumnNames) where TData : class
        {
            // Load the in-memory objects as an IDataView. ML.NET builds the schema from the public
            // properties of TData (float -> Single, string -> Text, bool -> Boolean, and so on).
            IDataView data = mlc.Data.LoadFromEnumerable<TData>(database);

            // Print a small sample of the data and the schema. This is only for diagnostics and
            // helps to confirm that the column types are what we expect before training.
            var vrdata = data.Preview(maxRows: 5);
            Console.WriteLine($"Data (first {vrdata.RowView.Length} rows): {JsonSerializer.Serialize(vrdata.RowView)}");
            foreach (var column in data.Schema)
            {
                Console.WriteLine($"Column Name: {column.Name}, Column Type: {column.Type}");
            }

            // Find the label column. Fail early with a clear message if the name is wrong.
            var labelColumn = data.Schema.GetColumnOrNull(outputColumnName)
                ?? throw new ArgumentException($"Label column '{outputColumnName}' was not found in the data.", nameof(outputColumnName));

            // Concatenate requires every input column to have the same type (Single), so each
            // feature is prepared first. The pipeline starts empty and grows one step at a time.
            IEstimator<ITransformer> pipeline = new EstimatorChain<ITransformer>();

            // The names of the columns that will actually be concatenated into "Features"
            // (original column names, or the converted/encoded replacements).
            var finalFeatureColumns = new List<string>();
            foreach (var name in featureColumnNames)
            {
                // Find the feature column. Fail early with a clear message if the name is wrong.
                var col = data.Schema.GetColumnOrNull(name)
                    ?? throw new ArgumentException($"Feature column '{name}' was not found in the data.", nameof(featureColumnNames));

                if (col.Type is TextDataViewType)
                {
                    // Text (for example Payment) cannot be used as a number. One-hot encoding turns each
                    // distinct value into its own 0/1 slot in a Single vector.
                    var encoded = name + "Encoded";
                    pipeline = pipeline.Append(mlc.Transforms.Categorical.OneHotEncoding(encoded, name));
                    finalFeatureColumns.Add(encoded);
                }
                else if (col.Type is NumberDataViewType nt && nt.RawType == typeof(float))
                {
                    // Already a Single, so no conversion is needed.
                    finalFeatureColumns.Add(name);
                }
                else if (col.Type is NumberDataViewType || col.Type is BooleanDataViewType)
                {
                    // Other numeric types (int, double, ...) and bool are converted to Single.
                    var converted = name + "Single";
                    pipeline = pipeline.Append(mlc.Transforms.Conversion.ConvertType(converted, name, DataKind.Single));
                    finalFeatureColumns.Add(converted);
                }
                else
                {
                    // Already a vector column (e.g. float[]) or another supported type: pass through.
                    finalFeatureColumns.Add(name);
                }
            }

            // Combine all prepared feature columns into the single "Features" vector that trainers read.
            pipeline = pipeline.Append(mlc.Transforms.Concatenate("Features", finalFeatureColumns.ToArray()));

            // Standardize every feature (subtract the mean, divide by the standard deviation). The SDCA
            // trainers learn badly when one feature is huge (for example an order total of 100000) next
            // to others that are 0 or 1, because the large values dominate and training does not converge.
            // Standardizing keeps all features on a similar scale without squeezing them into a tiny
            // range, which also keeps regression on small data sets accurate. Scaling is part of the
            // pipeline, so the same scaling is applied automatically when the model makes predictions.
            pipeline = pipeline.Append(mlc.Transforms.NormalizeMeanVariance("Features"));

            // Pick the trainer from the label type: bool -> binary classification, text -> multiclass,
            // numeric -> regression.
            var labelType = labelColumn.Type;
            if (labelType is BooleanDataViewType)
            {
                // Binary classification (true/false label, e.g. Returned). Adds the PredictedLabel,
                // Probability and Score output columns.
                pipeline = pipeline.Append(mlc.BinaryClassification.Trainers.SdcaLogisticRegression(
                    labelColumnName: outputColumnName,
                    featureColumnName: "Features",
                    maximumNumberOfIterations: 100));
            }
            else if (labelType is TextDataViewType)
            {
                // Multiclass classification (text label). The multiclass trainer needs a key-typed label,
                // so the text is mapped to keys first, and PredictedLabel is mapped back to text after training.
                const string keyLabel = "LabelKey";
                pipeline = pipeline
                    .Append(mlc.Transforms.Conversion.MapValueToKey(keyLabel, outputColumnName))
                    .Append(mlc.MulticlassClassification.Trainers.SdcaMaximumEntropy(
                        labelColumnName: keyLabel,
                        featureColumnName: "Features",
                        maximumNumberOfIterations: 100))
                    .Append(mlc.Transforms.Conversion.MapKeyToValue("PredictedLabel"));
            }
            else if (labelType is NumberDataViewType nl && nl.RawType == typeof(float))
            {
                // Regression (Single label, e.g. Price). Adds the Score output column.
                pipeline = pipeline.Append(mlc.Regression.Trainers.Sdca(
                    labelColumnName: outputColumnName,
                    featureColumnName: "Features",
                    maximumNumberOfIterations: 100));
            }
            else if (labelType is NumberDataViewType)
            {
                // Regression (int/double label). The regression trainer needs a Single label,
                // so the label is converted to a new column first.
                const string floatLabel = "LabelSingle";
                pipeline = pipeline
                    .Append(mlc.Transforms.Conversion.ConvertType(floatLabel, outputColumnName, DataKind.Single))
                    .Append(mlc.Regression.Trainers.Sdca(
                        labelColumnName: floatLabel,
                        featureColumnName: "Features",
                        maximumNumberOfIterations: 100));
            }
            else
            {
                // Any other label type (vector, date, and so on) has no matching trainer here.
                throw new NotSupportedException($"Label column '{outputColumnName}' has unsupported type {labelType}.");
            }


            Console.WriteLine($"Feature Columns {string.Join(", ", featureColumnNames)}");
            // Fit runs every step (encoding, concatenation, training) on the data and returns the
            // trained model. The same model applies the same prepared steps when it makes predictions.

            return pipeline.Fit(data);
        }
    }
}
