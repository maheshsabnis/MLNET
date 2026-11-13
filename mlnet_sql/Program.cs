using Microsoft.Data.SqlClient;
using Microsoft.ML;
using Microsoft.ML.Data;
using mllib;

// Demo: load data from SQL Server (AdventureWorks2025) instead of a CSV file, train a model
// that predicts whether a sales order was placed ONLINE (true) or through a reseller/salesperson
// (false), and let the user try their own order values.
//
// Source table: Sales.SalesOrderHeader (about 31,000 orders)
//   Label    : OnlineOrderFlag  (bit)  -> the value we want to predict
//   Features : SubTotal, Freight (money) and TerritoryID (a code, treated as a category)

// Connection string. Uses Windows authentication to the local default instance.
// To use another server, set the environment variable MLNET_SQL_CONNECTION.
// TrustServerCertificate is on because a local developer instance usually has a self-signed certificate.
string connectionString =
    Environment.GetEnvironmentVariable("MLNET_SQL_CONNECTION")
    ?? "Server=.;Database=AdventureWorks2025;Integrated Security=True;TrustServerCertificate=True;";

// The query returns exactly the columns the OrderRow class expects, in the same order as the
// [LoadColumn] indexes. The casts matter because ML.NET reads columns as float / string / bool:
//   money/decimal -> REAL (float)
//   TerritoryID   -> NVARCHAR, so it is one-hot encoded as a category (the ID number itself has no
//                    meaningful size, territory 9 is not "more" than territory 2)
//   OnlineOrderFlag -> BIT (bool)
const string sql = @"
    SELECT CAST(SubTotal AS REAL)            AS SubTotal,
           CAST(Freight  AS REAL)            AS Freight,
           CAST(TerritoryID AS NVARCHAR(10)) AS TerritoryId,
           CAST(OnlineOrderFlag AS BIT)      AS IsOnline
    FROM Sales.SalesOrderHeader
    WHERE TerritoryID IS NOT NULL";

// MLContext is the entry point to ML.NET. A fixed seed makes the results repeatable.
var mlContext = new MLContext(seed: 0);

try
{
    Console.WriteLine("Loading orders from SQL Server...");

    // DatabaseLoader is the ML.NET way to read from a database. CreateDatabaseLoader<T>() builds the
    // column layout from the [LoadColumn] attributes on OrderRow.
    DatabaseLoader loader = mlContext.Data.CreateDatabaseLoader<OrderRow>();

    // DatabaseSource ties together the provider (SQL Server), the connection string and the query.
    var source = new DatabaseSource(SqlClientFactory.Instance, connectionString, sql);

    // Load() is lazy. Nothing is read until the data is consumed on the next line.
    IDataView sqlView = loader.Load(source);

    // Read the rows into memory once as C# objects. DataTrainer takes a list of objects,
    // and this avoids going back to the database for every pass over the data.
    List<OrderRow> allRows = mlContext.Data.CreateEnumerable<OrderRow>(sqlView, reuseRowObject: false).ToList();
    Console.WriteLine($"{allRows.Count} orders loaded from SQL Server.");

    // Hold back 20% of the rows to test the model on orders it has never seen.
    // A model scored on its own training data would look better than it really is.
    IDataView allData = mlContext.Data.LoadFromEnumerable(allRows);
    var split = mlContext.Data.TrainTestSplit(allData, testFraction: 0.2, seed: 0);
    List<OrderRow> trainRows = mlContext.Data.CreateEnumerable<OrderRow>(split.TrainSet, reuseRowObject: false).ToList();

    // Train with the shared DataTrainer class from the mllib project. IsOnline is a bool label, so it
    // uses binary classification, and it one-hot encodes the text column TerritoryId on its own.
    var trainer = new DataTrainer();
    ITransformer model = trainer.Train<OrderRow>(mlContext, trainRows, "IsOnline", "SubTotal", "Freight", "TerritoryId");

    // Score the model on the held-back test rows.
    IDataView scoredTest = model.Transform(split.TestSet);
    var metrics = mlContext.BinaryClassification.Evaluate(scoredTest, labelColumnName: "IsOnline");

    // Compare with a model that always answers "online". If accuracy is not clearly above this
    // baseline, the model has learned nothing useful.
    double baseline = allRows.Count(r => r.IsOnline) / (double)allRows.Count;
    Console.WriteLine();
    Console.WriteLine($"Accuracy on unseen orders: {metrics.Accuracy:P1}  (always guessing 'online' gives {baseline:P1})");
    Console.WriteLine($"AUC: {metrics.AreaUnderRocCurve:F3}  (1.0 is perfect, 0.5 is a coin flip)");

    // The prediction engine turns one OrderRow into one OrderPrediction. It is not thread safe.
    var engine = mlContext.Model.CreatePredictionEngine<OrderRow, OrderPrediction>(model);

    // Let the user try their own orders until they leave the first question empty.
    while (true)
    {
        Console.WriteLine();
        Console.Write("Order SubTotal (empty to quit): ");
        string? subTotalText = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(subTotalText))
        {
            break;
        }

        Console.Write("Freight: ");
        string? freightText = Console.ReadLine();

        Console.Write("TerritoryID (1 to 10): ");
        string? territory = Console.ReadLine();

        // TryParse avoids a crash on bad input and lets the user try again.
        if (!float.TryParse(subTotalText, out float subTotal) || !float.TryParse(freightText, out float freight)
            || string.IsNullOrWhiteSpace(territory))
        {
            Console.WriteLine("Please enter numbers for SubTotal and Freight, and a TerritoryID.");
            continue;
        }

        // IsOnline is left at its default because it is the value being predicted.
        var prediction = engine.Predict(new OrderRow
        {
            SubTotal = subTotal,
            Freight = freight,
            TerritoryId = territory.Trim()
        });

        // Probability is the model's confidence (0 to 1) that the order is online.
        string channel = prediction.IsOnline ? "an ONLINE order" : "a RESELLER order";
        Console.WriteLine($"Prediction: this looks like {channel} ({prediction.Probability:P0} chance of online).");
    }
}
catch (Exception ex)
{
    // Most likely causes: SQL Server not reachable, wrong database name, or no permission.
    Console.WriteLine($"The Error: {ex.Message}");
}

/// <summary>
/// One order read from Sales.SalesOrderHeader. [LoadColumn(n)] matches each property to the
/// zero-based column position in the SQL query above.
/// </summary>
public class OrderRow
{
    /// <summary>Order value before tax and freight. Feature.</summary>
    [LoadColumn(0)]
    public float SubTotal { get; set; }

    /// <summary>Shipping cost. Feature.</summary>
    [LoadColumn(1)]
    public float Freight { get; set; }

    /// <summary>Sales territory code (1 to 10) kept as text so it is treated as a category. Feature.</summary>
    [LoadColumn(2)]
    public string TerritoryId { get; set; } = string.Empty;

    /// <summary>True if the order was placed online. This is the label the model predicts.</summary>
    [LoadColumn(3)]
    public bool IsOnline { get; set; }
}

/// <summary>
/// The prediction output of the binary classifier.
/// </summary>
public class OrderPrediction
{
    /// <summary>The predicted answer: true if the order is predicted to be online.</summary>
    [ColumnName("PredictedLabel")]
    public bool IsOnline { get; set; }

    /// <summary>How confident the model is that the order is online (0 to 1).</summary>
    public float Probability { get; set; }
}
