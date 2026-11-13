using Microsoft.ML;
using Microsoft.ML.Data;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

// "ml", "data", and "pipeline" are from the sentiment example
var ml = new MLContext(seed: 1);
var split = ml.Data.TrainTestSplit(data, testFraction: 0.3, seed: 1);
ITransformer model = pipeline.Fit(split.TrainSet);
IDataView predictions = model.Transform(split.TestSet);

CalibratedBinaryClassificationMetrics m = ml.BinaryClassification.Evaluate(predictions, labelColumnName: "Label");

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
Console.WriteLine(m.ConfusionMatrix.GetFormattedConfusionTable());