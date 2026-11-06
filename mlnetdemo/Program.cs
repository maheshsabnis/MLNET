
using Microsoft.ML;
using Microsoft.ML.Data;

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
do
{
    
var mlc = new MLContext(seed: 0);

// the Data
var database = new HouseDatabase();

// load the data as an IDataView object
IDataView data = mlc.Data.LoadFromEnumerable(database);

// Transform the data by concatenating the Size column into a Features column

var pipeline = mlc.Transforms.Concatenate("Features", "Size")
    .Append(mlc.Regression.Trainers.Sdca(labelColumnName: "Price", maximumNumberOfIterations: 100));
// Train the data
ITransformer model = pipeline.Fit(data);
// Create a prediction engine from the trained model
var engine = mlc.Model.CreatePredictionEngine<HouseData, PricePrediction>(model);

// Enter the size of the house to predict the price
Console.WriteLine("Enter the size of the house to predict the price:");
float size = float.Parse(Console.ReadLine());

// Make a prediction
var prediction = engine.Predict(new HouseData() { Size = size });

Console.WriteLine($"The Predicated Price for size {size} is: {prediction.Price:F2}");

//Console.ReadLine();

Console.WriteLine("Press any key to run the ML.NET demo, or 'q' to quit.");
} while (Console.ReadKey().KeyChar != 'q');

/*
The Model Classes for definig the Data Sehema 
*/

public class  HouseData
{
    public float Size { get; set; }
    public float Price { get; set; }
}

public class PricePrediction
{
    [ColumnName("Score")]
    public float Price { get; set; }
}   

public class HouseDatabase : List<HouseData>
{
    public HouseDatabase()
    {
       Add( new HouseData() { Size = 1.1F, Price = 1.2F });
       Add( new HouseData() { Size = 1.9F, Price = 2.3F });
       Add( new HouseData() { Size = 2.8F, Price = 3.0F });
       Add( new HouseData() { Size = 3.4F, Price = 3.7F });
       Add( new HouseData() { Size = 4.0F, Price = 4.5F });
       Add( new HouseData() { Size = 5.0F, Price = 5.0F });
       Add( new HouseData() { Size = 4.2F, Price = 4.7F });
    }
}