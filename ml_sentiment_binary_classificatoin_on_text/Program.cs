using Microsoft.ML;
using Microsoft.ML.Data;

Console.WriteLine("Demo Sentiment Analysis Binary Classification on Text !");
/*
 Decide whether a review is positive. FeaturizeText turns words into numbers, and a logistic regression learns which words matter. The training set is tiny, so use it to learn the API and not to judge accuracy.


featureColumnName: "Features" tells the ML.NET trainer which input column contains the processed feature vector that should be used for training and prediction.
 
 */
var ml = new MLContext(seed: 1);


var reviews = new List<Review>
{
    new() { Text = "I loved it, works perfectly and arrived early", Positive = true },
    new() { Text = "Excellent quality, very happy with this purchase", Positive = true },
    new() { Text = "Great value, would buy again", Positive = true },
    new() { Text = "Fantastic service and a lovely product", Positive = true },
    new() { Text = "Really pleased, exactly as described", Positive = true },
    new() { Text = "Brilliant, easy to use and sturdy", Positive = true },
    new() { Text = "Terrible, it broke after one day", Positive = false },
    new() { Text = "Awful quality and slow delivery", Positive = false },
    new() { Text = "Very disappointed, not as described", Positive = false },
    new() { Text = "Waste of money, would not recommend", Positive = false },
    new() { Text = "Poor build and the support was rude", Positive = false },
    new() { Text = "Broken on arrival, want a refund", Positive = false },
};

IDataView data = ml.Data.LoadFromEnumerable(reviews);

//  FeaturizeText turns words into numbers
// logistic regression learns which words matter
var pipeline = ml.Transforms.Text.FeaturizeText("Features", nameof(Review.Text))
    .Append(ml.BinaryClassification.Trainers.SdcaLogisticRegression(labelColumnName: "Label", featureColumnName: "Features"));

// Train the model
ITransformer model = pipeline.Fit(data);

// Create a prediction engine from the model
var predictionEngine = ml.Model.CreatePredictionEngine<Review, ReviewPrediction>(model);
foreach (var text in new[] { "Lovely product, very happy", "Broke straight away, awful" })
{
    var p = predictionEngine.Predict(new Review { Text = text });
    Console.WriteLine($"{(p.Positive ? "positive" : "negative"),-9} {p.Probability:P0}   {text}");
}




// "ml", "data", and "pipeline" are from the sentiment example
// Split the data into a training set and a test set, train the model on the training set, and evaluate it on the test set
// testFraction: 0.3 means 30% of the data will be used for testing, and 70% for training
// seed is used to make the split reproducible
var split = ml.Data.TrainTestSplit(data, testFraction: 0.3, seed: 1);
// Train the model on the training set
// TrainSet is the training set, and TestSet is the test set
ITransformer model11 = pipeline.Fit(split.TrainSet);
IDataView predictions = model11.Transform(split.TestSet);
// CalibratedBinaryClassificationMetrics means that the model outputs probabilities, not just binary predictions. The metrics are calculated based on the probabilities and the true labels.
CalibratedBinaryClassificationMetrics m = ml.BinaryClassification.Evaluate(predictions, labelColumnName: "Label");

// P1 means percentage with 1 decimal place, F3 means fixed point with 3 decimal places

Console.WriteLine($"Accuracy:               {m.Accuracy:P1}   share of correct answers (misleading when one class is rare)");
Console.WriteLine($"AUC:                    {m.AreaUnderRocCurve:F3}   0.5 is chance, 1.0 is perfect");
Console.WriteLine($"AUC precision-recall:   {m.AreaUnderPrecisionRecallCurve:F3}   better than AUC when positives are rare");
Console.WriteLine($"F1 score:               {m.F1Score:F3}   balance of precision and recall");
Console.WriteLine($"Positive precision:     {m.PositivePrecision:F3}   of the items flagged positive, the share that were");
Console.WriteLine($"Positive recall:        {m.PositiveRecall:F3}   of the real positives, the share found");
Console.WriteLine($"Negative precision:     {m.NegativePrecision:F3}");
Console.WriteLine($"Negative recall:        {m.NegativeRecall:F3}");
Console.WriteLine($"Log loss:               {m.LogLoss:F3}   lower is better, punishes confident mistakes");
Console.WriteLine($"Log loss reduction:     {m.LogLossReduction:F3}   improvement over always guessing the base rate");
// GetFormattedConfusionTable() returns a string that shows the confusion matrix in a nice table format
Console.WriteLine(m.ConfusionMatrix.GetFormattedConfusionTable());




// Beat a baseline first with the Prior trainer

// "ml" and "split" from the earlier examples; the pipeline builds a Features column first
var prep = ml.Transforms.Text.FeaturizeText("Features", "Text");

var baseline = prep.Append(ml.BinaryClassification.Trainers.Prior(labelColumnName: "Label"));
var real = prep.Append(ml.BinaryClassification.Trainers.SdcaLogisticRegression(labelColumnName: "Label"));

double Auc(IEstimator<ITransformer> p) =>
    ml.BinaryClassification.EvaluateNonCalibrated(p.Fit(split.TrainSet).Transform(split.TestSet)).AreaUnderRocCurve;

Console.WriteLine($"Baseline AUC: {Auc(baseline):F3}");     // about 0.5
Console.WriteLine($"Model AUC:    {Auc(real):F3}");


Console.ReadLine();

/// <summary>
/// Class that is the schema of the data for teh Review Text.
/// </summary>
public class Review
{
    public string Text { get; set; } = "";
    [ColumnName("Label")] public bool Positive { get; set; }
}

/// <summary>
/// The class for the Review Prdiction
/// </summary>
public class ReviewPrediction
{
    [ColumnName("PredictedLabel")] public bool Positive { get; set; }
    public float Probability { get; set; }
    public float Score { get; set; }
}