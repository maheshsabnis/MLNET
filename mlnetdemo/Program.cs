
using Microsoft.ML;
using Microsoft.ML.Data;
using mllib;
using System.Text.Json;

Console.WriteLine("The smallest complete ML.NET program. It loads objects, builds a Features column, trains a linear regression, and predicts. With so little data the numbers only show the shape of the API.");

/*
 MLContext initializes the ML.NET environment and is the first object you create in any ML.NET application.

It provides access to all ML.NET components, such as data loading, transformations, training algorithms, evaluation, and model management.

It manages reproducibility by allowing you to specify a random seed:

It is used throughout the complete ML workflow, including loading data, building pipelines, training models, evaluating accuracy, and making predictions.

It acts like a factory class, exposing namespaces such as:

    mlContext.Data → Data operations
    mlContext.Transforms → Feature engineering
    mlContext.BinaryClassification → Binary ML models
    mlContext.MulticlassClassification → Multi-class models
    mlContext.Model → Save/Load models
 */
// The demo runs in a loop so several house sizes can be tried. It exits when 'q' is pressed.
do
{
    #region Commented Code
    // Earlier version of this demo that built and trained the pipeline directly in this file.
    // The same work is now done by DataTrainer.Train in the mllib project.

    // Create the ML.NET context. A fixed seed makes the training results repeatable.
    var mlc = new MLContext(seed: 0);

// the Data
var database = new HouseDatabase();
    
    // load the data as an IDataView object
    IDataView data = mlc.Data.LoadFromEnumerable(database);

    var vrdata = data.Preview();
    Console.WriteLine($"Data: {JsonSerializer.Serialize(vrdata.RowView)}");
    // Print The Data   
    foreach (var column in data.Schema)
    {
        Console.WriteLine($"Column Name: {column.Name}, Column Type: {column.Type}");
    }


    // Transform the data by concatenating the Size column into a Features column

    var pipeline = mlc.Transforms.Concatenate("Features", "Size")
    .Append(mlc.Regression.Trainers.Sdca(labelColumnName: "Price", maximumNumberOfIterations: 100));
// Train the data
ITransformer model = pipeline.Fit(data);

    #endregion

/*

    // Helper from the mllib project that prepares the features, picks the trainer and trains.
  //  DataTrainer dataTrainer = new DataTrainer();
*/

    // Enter the size of the house to predict the price
    Console.WriteLine("Enter the size of the house to predict the price:");
    float size = float.Parse(Console.ReadLine());

    /*
    // Train on the sample houses. Price is a float label, so DataTrainer picks a regression
    // trainer, and Size is the only feature.
   // var model = dataTrainer.Train<HouseData>(mlc, new HouseDatabase(), "Price", "Size");
    */

    // Create a prediction engine from the trained model. It maps one input object (HouseData)
    // to one output object (PricePrediction).
    var engine = mlc.Model.CreatePredictionEngine<HouseData, PricePrediction>(model);



// Make a prediction for the size entered by the user. Price is not set because it is
// the value being predicted.
var prediction = engine.Predict(new HouseData() { Size = size });

// Show the predicted price with two decimal places.
Console.WriteLine($"The Predicated Price for size {size} is: {prediction.Price:F2}");

//Console.ReadLine();

Console.WriteLine("Press any key to run the ML.NET demo, or 'q' to quit.");
// Loop again unless the user pressed 'q'.
} while (Console.ReadKey().KeyChar != 'q');

/*
The Model Classes for defining the Data Schema
*/

/// <summary>
/// One house in the training data (and the input to the prediction engine).
/// </summary>
public class  HouseData
{
    /// <summary>The size of the house. Used as the feature.</summary>
    public float Size { get; set; }

    /// <summary>The price of the house. This is the label the model predicts.</summary>
    public float Price { get; set; }
}

/// <summary>
/// The prediction output. A regression trainer writes its result to the Score column,
/// so the property is mapped to that name.
/// </summary>
public class PricePrediction
{
    /// <summary>The predicted price.</summary>
    [ColumnName("Score")]
    public float Price { get; set; }
}

/// <summary>
/// A small in-memory training set of house sizes and prices. With so few rows the
/// predictions only show how the ML.NET API works.
/// </summary>
public class HouseDatabase : List<HouseData>
{
    public HouseDatabase()
    {
       // Each row is one training example: the size is the input and the price is the known answer.
       Add( new HouseData() { Size = 1.1F, Price = 1.2F });
       Add( new HouseData() { Size = 1.9F, Price = 2.3F });
       Add( new HouseData() { Size = 2.8F, Price = 3.0F });
       Add( new HouseData() { Size = 3.4F, Price = 3.7F });
       Add( new HouseData() { Size = 4.0F, Price = 4.5F });
       Add( new HouseData() { Size = 5.0F, Price = 5.0F });
       Add( new HouseData() { Size = 4.2F, Price = 4.7F });
    }
}