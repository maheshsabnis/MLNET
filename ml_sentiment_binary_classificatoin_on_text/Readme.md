# Sentiment Analysis on Text Using Binary Classification

This example shows how ML.NET converts review text into numeric features and trains a binary classifier to predict sentiment.

```csharp
var pipeline = ml.Transforms.Text.FeaturizeText(
        "Features",
        nameof(Review.Text))
    .Append(
        ml.BinaryClassification.Trainers.SdcaLogisticRegression(
            labelColumnName: "Label",
            featureColumnName: "Features"));
```

`featureColumnName: "Features"` tells the trainer which input column contains the processed feature vector used for training and prediction.

## Pipeline walkthrough

### 1. Start with labeled data

Suppose your class looks like this:

```csharp
public class Review
{
    public bool Label { get; set; }
    public string Text { get; set; } = "";
}
```

Sample data:

| Label | Text |
| --- | --- |
| true | Excellent service |
| false | Very bad experience |
| true | Fast delivery |

### 2. Convert text into features

```csharp
ml.Transforms.Text.FeaturizeText(
    outputColumnName: "Features",
    inputColumnName: nameof(Review.Text))
```


This transform:

- Reads the `Text` column.
- Tokenizes words.
- Generates numerical values (such as n-grams and TF-IDF features).
- Produces a new column named `Features`.

Conceptually:

| Label | Text | Features |
| --- | --- | --- |
| true | Excellent service | `[0.12, 0.56, 0.78, ...]` |
| false | Very bad experience | `[0.91, 0.22, 0.15, ...]` |

The model cannot learn directly from raw text. It learns from the numeric vector in `Features`.

### 3. Train using the features column

```csharp
ml.BinaryClassification.Trainers.SdcaLogisticRegression(
    labelColumnName: "Label",
    featureColumnName: "Features")
```


The trainer uses these columns:

- `Label`: the expected answer (`true` or `false`).
- `Features`: the input data used for prediction.

In other words, the model learns the mapping:

```text
Input: Features
Output: Label
```

## Why use the name `Features`?

`Features` is the conventional name for the feature vector column in ML.NET. Many trainers use `Features` and `Label` by default when column names are not specified.

For example:

```csharp
var trainer = ml.BinaryClassification.Trainers.SdcaLogisticRegression();
```

This trainer expects:

- `Features` as the input.
- `Label` as the target.

## Can the feature column have a different name?

Yes. Set both the featurizer output column and the trainer's feature column to the same name:

```csharp
ml.Transforms.Text.FeaturizeText(
        "ReviewVector",
        nameof(Review.Text))
    .Append(
        ml.BinaryClassification.Trainers.SdcaLogisticRegression(
            labelColumnName: "Label",
            featureColumnName: "ReviewVector"));
```

Here, the trainer uses `ReviewVector` instead of `Features`.

## Interview summary

`featureColumnName` specifies the column containing the input features used by the machine-learning algorithm. In this example, `FeaturizeText()` converts the text into a numeric vector in the `Features` column, and `SdcaLogisticRegression` uses that column to train and make predictions.

Remember:

- `Label` is what the model should predict.
- `Features` are the data used to make the prediction.
- `featureColumnName` names the column containing those features.

## How labels and features work together

The relationship between labels and features is central to supervised machine learning. In this pipeline, `FeaturizeText` creates the feature vector from review text, and the trainer learns to predict the label from that vector.

### What is the label column?

The `Label` column contains the correct answer the model is expected to learn.

For example:

```csharp
public class Review
{
    public bool Label { get; set; }
    public string Text { get; set; } = "";
}
```

Example data:

| Label | Text |
| --- | --- |
| true | Excellent service |
| true | Fast delivery |
| false | Very bad support |
| false | Product arrived damaged |

Here, `true` represents a positive review and `false` represents a negative review.

The label is also called the target, ground truth, expected output, or dependent variable.

### What is the features column?

After `FeaturizeText("Features", "Text")`, ML.NET creates a numeric feature vector:

| Label | Text | Features |
| --- | --- | --- |
| true | Excellent service | `[0.12, 0.56, 0.21, ...]` |
| false | Very bad support | `[0.89, 0.03, 0.77, ...]` |

The numeric vector in `Features` represents the text in a form the algorithm can understand.

### How are they related?

Think of training as `Features → Label`, or `Input → Output`. The algorithm tries to answer: given these features, what label should be predicted?

For example:

| Features (from text) | Label |
| --- | --- |
| Vector for "Excellent service" | true |
| Vector for "Fast delivery" | true |
| Vector for "Very bad support" | false |

SDCA can learn patterns such as:

- `excellent` → likely positive
- `fast` → likely positive
- `bad` → likely negative
- `damaged` → likely negative

### Real-world analogy

Suppose you are training a model to predict house prices:

| Bedrooms | Area | Garage | Price |
| --- | --- | --- | --- |
| 2 | 1,000 | 1 | 200,000 |
| 4 | 2,500 | 2 | 550,000 |

The features are the number of bedrooms, area, and garage spaces. The label is the price. The model learns the relationship `(Bedrooms, Area, Garage) → Price`.

Similarly, for sentiment analysis, the model learns `Text → Sentiment`:

- Features: converted text.
- Label: positive or negative sentiment.

### In ML.NET terms

```csharp
SdcaLogisticRegression(
    labelColumnName: "Label",
    featureColumnName: "Features")
```

Training means taking input from `Features`, comparing predictions against `Label`, and adjusting weights to improve predictions.

### Interview definition

The Label column contains the expected output or target value, while the Features column contains the input data used for prediction. During training, ML.NET learns the relationship between the feature values and the corresponding labels so that it can predict labels for new unseen feature data.

A simple formula to remember:

- `Features` + `Label` = training data
- `Features` = input
- `Label` = expected output
- Model = learned relationship between them


# Beat the baseline

The phrase "Beat a baseline first with the Prior trainer" is a very important Machine Learning principle.

What is a Baseline?

A baseline is the simplest possible model that serves as a benchmark.

Before celebrating that your ML model achieves 80%, 90%, or 95% accuracy, you should ask:

"Is my model actually learning something useful, or could a trivial approach achieve the same result?"

That's where the Prior Trainer comes in.

What Does the Prior Trainer Do?
var baseline = prep.Append(
    ml.BinaryClassification.Trainers.Prior(
        labelColumnName: "Label"));


The Prior trainer:

Completely ignores the Features column
Does not learn from the text
Looks only at the label distribution

Example training data:

LabelTrue
True
True
True
False

80% are True.

The Prior trainer learns:

Probability(True)  = 80%
Probability(False) = 20%


For every new row it simply predicts:

"Most likely True"


without reading the text.

Why Is It Called "Prior"?

In statistics:

Prior Probability


means the probability of an event before considering any evidence.

Example:

80% of reviews are positive


That's the prior probability.

The trainer uses only that information.

Your Example
Baseline Model
var baseline =
    prep.Append(
        ml.BinaryClassification.Trainers.Prior(
            labelColumnName: "Label"));


Even though you create:

FeaturizeText("Features", "Text")


the Prior trainer ignores those features.

Conceptually:

Text                Prediction
--------------------------------
"Great service"     Positive
"Terrible support"  Positive
"Awful product"     Positive


because positive is the majority class.

Real Model
var real =
    prep.Append(
        ml.BinaryClassification.Trainers.SdcaLogisticRegression(
            labelColumnName: "Label"));


SDCA actually uses:

Features -> Label


and learns relationships:

great      -> positive
excellent  -> positive
bad        -> negative
terrible   -> negative

What Does "Beat the Baseline" Mean?

Suppose you evaluate both models:

Baseline AUC = 0.50
SDCA AUC     = 0.92


This means:

✅ The text features are useful.

✅ SDCA learned meaningful patterns.

✅ Your model significantly outperforms a trivial predictor.

Bad Example
Baseline AUC = 0.82
SDCA AUC     = 0.81


Your fancy ML model is actually worse!

This indicates:

Features are poor
Data quality is low
Not enough training data
Wrong algorithm

In such a situation:

The model has NOT beaten the baseline.

Why AUC Near 0.5?

For binary classification:

AUC = 0.5


means:

Random guessing

AUC = 1.0


means:

Perfect classification


Since Prior does not use actual features, its AUC is usually around:

0.5


which makes it a good benchmark.

Interview Answer

"Beat a baseline first" means comparing your machine learning model against a very simple benchmark model. In ML.NET, the Prior trainer is often used as that benchmark because it predicts only based on the label distribution and ignores all feature data. If a trained model such as SDCA Logistic Regression cannot achieve better metrics than the Prior trainer, then the features are not providing useful predictive information and the model is not adding value.

One-line explanation for your ML.NET session

The Prior trainer answers: "What performance can I get without using any features?" and my real model must perform better than that baseline to prove that it is actually learning from the data.



## For a Sentiment Analysis (Binary Classification) project in ML.NET, the recommended implementation order should be:

1. Define the Input Schema
public class Review
{
    public bool Label { get; set; }   // Positive=True, Negative=False
    public string Text { get; set; } = string.Empty;
}


Here:

Label = Expected output
Text = Raw input data
2. Load the Data
IDataView data = ml.Data.LoadFromTextFile<Review>(
    path: "reviews.csv",
    hasHeader: true,
    separatorChar: ',');

3. Split Data into Train/Test
var split = ml.Data.TrainTestSplit(
    data,
    testFraction: 0.2);


Typically:

80% Training
20% Testing
4. Create Features

Text cannot be used directly by the trainer.

var prep = ml.Transforms.Text.FeaturizeText(
    outputColumnName: "Features",
    inputColumnName: nameof(Review.Text));


This creates:

Text
  ↓
Features (Numeric Vector)

5. Build a Baseline Model First (Prior Trainer)
var baseline =
    prep.Append(
        ml.BinaryClassification.Trainers.Prior(
            labelColumnName: "Label"));


Purpose:

Can I get acceptable results
without using the Features?


Evaluate:

var baselineModel = baseline.Fit(split.TrainSet);

var baselinePredictions =
    baselineModel.Transform(split.TestSet);

var baselineMetrics =
    ml.BinaryClassification.EvaluateNonCalibrated(
        baselinePredictions);

Console.WriteLine(
    $"Baseline AUC = {baselineMetrics.AreaUnderRocCurve:F3}");

6. Build the Real Model
var model =
    prep.Append(
        ml.BinaryClassification.Trainers.SdcaLogisticRegression(
            labelColumnName: "Label",
            featureColumnName: "Features"));

7. Train the Model
var trainedModel =
    model.Fit(split.TrainSet);

8. Evaluate the Model
var predictions =
    trainedModel.Transform(split.TestSet);

var metrics =
    ml.BinaryClassification.Evaluate(
        predictions);

Console.WriteLine(
    $"AUC = {metrics.AreaUnderRocCurve:F3}");


Compare:

Baseline AUC = 0.50
SDCA AUC     = 0.91


Since:

0.91 > 0.50


the model is learning useful patterns.

9. Save the Model
ml.Model.Save(
    trainedModel,
    data.Schema,
    "sentimentModel.zip");

10. Make Predictions
var engine =
    ml.Model.CreatePredictionEngine<Review, Prediction>(
        trainedModel);

var result = engine.Predict(
    new Review
    {
        Text = "Excellent service and support"
    });

Console.WriteLine(result.PredictedLabel);

Production-Grade Flow
1. Define Schema
          ↓
2. Load Data
          ↓
3. Train/Test Split
          ↓
4. Featurize Text
          ↓
5. Prior Trainer (Baseline)
          ↓
6. Evaluate Baseline
          ↓
7. SDCA Logistic Regression
          ↓
8. Evaluate Metrics
          ↓
9. Compare with Baseline
          ↓
10. Save Model
          ↓
11. Predict New Reviews

Simple Rule for ML.NET Sessions

First create the "Features" column, then establish a baseline using the Prior trainer, then train a real algorithm such as SDCA Logistic Regression, compare metrics, and only keep the model if it beats the baseline.

This is the order typically followed by experienced ML engineers because it proves that the features are actually adding predictive value before investing time in model tuning.