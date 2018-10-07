# FluentFileImporter
A small library to briefly create file importers using a fluent syntax.

Currently, only importers for raw text files are imported.

# Usage example
This example is included in the solution as the Example project.

It imports a text file with ICD10 diagnostics, `icd10cm_order_2019.txt`, which can be found [here](https://www.cms.gov/Medicare/Coding/ICD10/2019-ICD-10-CM.html).

This is a sample of its content:

```
00037 A039    1 Shigellosis, unspecified                                     Shigellosis, unspecified
00038 A04     0 Other bacterial intestinal infections                        Other bacterial intestinal infections
00039 A040    1 Enteropathogenic Escherichia coli infection                  Enteropathogenic Escherichia coli infection
00040 A041    1 Enterotoxigenic Escherichia coli infection                   Enterotoxigenic Escherichia coli infection
00041 A042    1 Enteroinvasive Escherichia coli infection                    Enteroinvasive Escherichia coli infection
```

The data from the previous file will be adapted to this entity:

```C#
public class Icd10Diagnostic
{
	public int Order { get; set; }
	public string Code { get; set; }
	public bool ValidForSubmission { get; set; }
	public string ShortDescription { get; set; }
	public string LongDescription { get; set; }
}
```

To adapt the file, the importer must be defined.

For this importer, the columns are defined by position in line and column length. Other file importers (to be implemented) may require to be defined differently. For this particular example, the importer will be defined this way:

```C#
// Create importer
var fileTextImporterForDiagnostics = FileImporter
  .ToImportTextFile()
  .IgnoreFirstLine(false)
	.WithColumn(0, 5)     // Natural number ID for diagnostic
	.WithColumn(6, 7)     // Diagnostic code
	.WithColumn(14, 1)    // Flag; 1 if it's valid for HIPAA transactions.
	.WithColumn(16, 60)   // Short description of the diagnostic
	.WithColumn(77)       // Long description of the diagnostic
   .AdaptTo<Icd10Diagnostic>((e, columns) =>
	  {
		  e.Order = int.Parse(columns[0].Trim());

		  // Get code and add point
		  var code = columns[1].Trim();
		  e.Code = (code.Length > 3 ? code.Insert(3, ".") : code);

		  // Boolean is true if marked as "1", which is billable.
		  e.ValidForSubmission = columns[2].Trim() == "1";

		  // Code Descriptions
		  e.ShortDescription = columns[3].Trim();
		  e.LongDescription = columns[4].Trim();
	  });
```

To get the entities from the importer, the file must be provided as input to the importer
to get the entities using `.GenerateEntitiesFromFile()`. These entities are created and filled as
defined in the `AdaptTo<T>` method. For instance:

```C#
var filename = "icd10cm_order_2019.txt";
var entities = fileTextImporterForDiagnostics
	.GenerateEntitiesFromFile(filename)
	.ToList();
```

These entities can be used as intended.

For example, in this case an entity is being written to console:

```C#
Console.WriteLine($"The {(entity.ValidForSubmission ? "billable" : "non-billable")}" +
                  $" code {entity.Code} is described as \n {entity.LongDescription}.\n");
```

Sample output of the previous:

```
The billable code M67.922 is described as
  Unspecified disorder of synovium and tendon, left upper arm.
```