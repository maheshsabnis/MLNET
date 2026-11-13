Yes. In C#, you can create a class-like type dynamically at runtime, but for ML.NET it's usually better to use one of these approaches:

Option 1: Use LoadFromTextFile<T>() with a Generated Type (Recommended)

If the CSV schema is known only at runtime, you can generate a type using:

System.Reflection.Emit
Roslyn (Microsoft.CodeAnalysis)
Source Generators (compile-time)

Example CSV:

Age,Salary,Experience,Label
25,50000,2,0
30,75000,5,1


Generate:

public class EmployeeData
{
    public float Age { get; set; }
    public float Salary { get; set; }
    public float Experience { get; set; }
    public bool Label { get; set; }
}


Then:

var data = mlContext.Data.LoadFromTextFile<EmployeeData>(
    "data.csv",
    hasHeader: true,
    separatorChar: ',');


This is the fastest approach.

Option 2: Use TextLoader Dynamically (Best for Unknown CSV Structures)

ML.NET allows you to define columns at runtime without creating a class.

Read the CSV header:

var headers = File.ReadLines("data.csv")
                  .First()
                  .Split(',');


Build columns dynamically:

var columns = headers
    .Select((h, i) =>
        new TextLoader.Column(
            name: h,
            dataKind: DataKind.Single,
            source: new[] { new TextLoader.Range(i) }))
    .ToArray();


Create loader:

var loader = mlContext.Data.CreateTextLoader(
    new TextLoader.Options
    {
        HasHeader = true,
        Separators = new[] { ',' },
        Columns = columns
    });

IDataView data = loader.Load("data.csv");


This avoids generating classes entirely.

Why this is ideal for ML.NET

ML.NET internally works on:

IDataView


not directly on classes.

So once you have an IDataView, you can do:

var pipeline =
    mlContext.Transforms.Concatenate(
        "Features",
        headers.Where(h => h != "Label").ToArray())
    .Append(
        mlContext.BinaryClassification.Trainers
            .SdcaLogisticRegression());


No class is needed.

Option 3: Dynamic Record Using Reflection.Emit

You can create records/classes at runtime.

AssemblyBuilder assemblyBuilder =
    AssemblyBuilder.DefineDynamicAssembly(
        new AssemblyName("DynamicAssembly"),
        AssemblyBuilderAccess.Run);

ModuleBuilder moduleBuilder =
    assemblyBuilder.DefineDynamicModule("MainModule");

TypeBuilder typeBuilder =
    moduleBuilder.DefineType(
        "CsvRecord",
        TypeAttributes.Public);


Loop through CSV columns:

foreach(var column in headers)
{
    var field = typeBuilder.DefineField(
        "_" + column,
        typeof(float),
        FieldAttributes.Private);

    var property = typeBuilder.DefineProperty(
        column,
        PropertyAttributes.None,
        typeof(float),
        null);
}


Create type:

Type dynamicType = typeBuilder.CreateType();


Then:

var listType = typeof(List<>).MakeGenericType(dynamicType);

Option 4: Use DataFrame (My Preferred Choice for Generic ML.NET Solutions)

ML.NET has:

Microsoft.Data.Analysis.DataFrame


Load CSV:

DataFrame df = DataFrame.LoadCsv("data.csv");


Inspect:

foreach(var column in df.Columns)
{
    Console.WriteLine($"{column.Name}");
}


Convert to IDataView:

IDataView dataView =
    mlContext.Data.LoadFromEnumerable(
        df.Rows.Select(row => new DynamicRow(row)));


This approach is excellent when:

The CSV schema changes frequently.
You're building AutoML tools.
You're building generic training platforms.
Features are unknown beforehand.
What I would recommend for your scenario

Since you are learning and using ML.NET, and your CSV structure is discovered at runtime:

Scenario	RecommendationFixed CSV schema	Strongly Typed Class/Record
CSV columns change frequently	TextLoader
AutoML / Generic Training Platform	TextLoader + IDataView
Need strongly typed objects generated dynamically	Reflection.Emit or Roslyn
Enterprise ML Pipeline	IDataView only

For most real-world ML.NET applications, including healthcare insurance analytics, fraud detection, claims prediction, and customer churn models, developers typically do not generate classes dynamically. They use a dynamic TextLoader to produce an IDataView and build pipelines from column names discovered at runtime.

A useful pattern is:

CSV -> TextLoader -> IDataView
                     ↓
              Feature Detection
                     ↓
                Pipeline
                     ↓
                 Training


This is simpler, faster, and aligns with how ML.NET is designed to work.