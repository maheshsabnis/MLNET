using Microsoft.ML;
using Microsoft.ML.Data;
using mllib;
using System.Text.Json;
// Demo: load orders.csv, train a model that predicts whether an order will be returned,
// and ask the user for an order total and payment mode to get a prediction.
Console.WriteLine("Loading the CSV file");

// MLContext is the entry point to all ML.NET operations (data, transforms, trainers, models).
var mlContext = new MLContext();
// Load the comma-separated CSV as an IDataView using OrderCsv's [LoadColumn] mappings.
// Skip the header and allow quoted fields, including values containing commas.
// Rows are read lazily when the IDataView is consumed.
// allowQuoting: true allows quoted fields, including values containing commas. 
// hasHeader: true skips the first line of the file, which contains column names.
IDataView dataView = mlContext.Data.LoadFromTextFile<OrderCsv>(@"D:\maheshapps\net10\MLNET\csvdata\orders.csv", hasHeader: true, separatorChar: ',', allowQuoting: true );


// Preview() reads up to 100 rows by default, so this prints the number of preview rows, not the file's total.
Console.WriteLine($"{dataView.Preview().RowView.Length}, rows loaded");

// Preview of the loaded data (useful for debugging, for example in the commented loop below).
var preview = dataView.Preview();

// Iterate over the IDataView's schema to see the values
//foreach (var row in preview.RowView)
//{
//    Console.WriteLine($"Keys {string.Join(", ", row.Values.Select(v => v.Key))}, Values {string.Join(", ", row.Values.Select(v => v.Value))}");
//}

// one column as an array
//float [] totals = dataView.GetColumn<float>("Total").ToArray();

//Console.WriteLine($"Total Average: {totals.Average()}");

// Put this data in C# objects. reuseRowObject: false creates a new object per row,
// which is required because the list keeps all of them.
List<OrderCsv> orders = mlContext.Data.CreateEnumerable<OrderCsv>(dataView, reuseRowObject: false).ToList();
//Console.WriteLine($"Orders Data: {JsonSerializer.Serialize(orders)}");

// Wrap the interactive part in try/catch so bad input (for example a non-numeric total)
// or a training problem is shown as a message instead of crashing the app.
try
{
    // Helper from the mllib project that prepares the features, picks the trainer and trains.
    DataTrainer dataTrainer = new DataTrainer();

    // Read the order total entered by the user (float.Parse throws on invalid input).
    Console.WriteLine("Enter the Order Total in Rs./-");
    float orderTotal = float.Parse(Console.ReadLine());

    // Read the payment mode, for example credit_card, debit_card, paypal, cash or gift_card.
    Console.WriteLine("Enter the  Payment Mode to check the status for return status:");
    string payment_mode = Console.ReadLine();

    // Train the model using the DataTrainer class. Here the Total and Payment columns are the features
    // used to predict the Returned column. Because Returned is a bool, DataTrainer picks binary
    // classification, and it one-hot encodes the text column Payment.
    var model = dataTrainer.Train<OrderCsv>(mlContext, orders, "Returned", "Total", "Payment");

    // Create a prediction engine from the trained model. It maps one input object (OrderCsv)
    // to one output object (OrderPrediction). It is not thread safe, so use one per thread.
    var engine = mlContext.Model.CreatePredictionEngine<OrderCsv, OrderPrediction>(model);

    // Predict using the values the user entered. ItemCount is left at 0 because it is not a feature.
    var prediction = engine.Predict(new OrderCsv() { Total = orderTotal, Payment = payment_mode });

    Console.WriteLine($"Prediction: Returned = {prediction.Returned}");

}
catch (Exception ex)
{
    // Show the error message (for example invalid number format or a training error).
    Console.WriteLine($"The Error: {ex.Message}");
}
// Keep the console window open until Enter is pressed.
Console.ReadLine();

/// <summary>
/// One row of orders.csv. [LoadColumn(n)] maps each property to the zero-based CSV column index,
/// in the order: item_count, total, payment, returned.
/// </summary>
public class OrderCsv
{
    /// <summary>Number of items in the order (CSV column 0).</summary>
    [LoadColumn(0)]
    public float ItemCount { get; set; }

    /// <summary>Order total (CSV column 1). Used as a feature.</summary>
    [LoadColumn(1)]
    public float Total { get; set; }

    /// <summary>Payment mode, such as credit_card or paypal (CSV column 2). Text feature, one-hot encoded.</summary>
    [LoadColumn(2)]
    public string Payment { get; set; } = null!;

    /// <summary>Whether the order was returned (CSV column 3). This is the label the model predicts.</summary>
    [LoadColumn(3)]
    public bool Returned { get; set; }
}

/// <summary>
/// The prediction output. The binary classifier writes its true/false result to the
/// PredictedLabel column, so the property is mapped to that name.
/// </summary>
public class OrderPrediction
{
    /// <summary>The predicted value: true if the order is predicted to be returned.</summary>
    [ColumnName("PredictedLabel")]
    public bool Returned { get; set; }
}