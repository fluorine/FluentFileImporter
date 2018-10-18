# FluentFileImporter
A small library to briefly create file importers using a fluent syntax.

Currently, these are the only importers available:
 - Fixed-width column text files
 - Pipe-character separated columns text files

# Usage example for text file with fixed-width data columns
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
  .ForTextFile()
  .WithFixedWidthColumns()
  .IgnoringFirstLine(false)
	.HasColumn(0, 5)
	.HasColumn(6, 7)
	.HasColumn(14, 1)
	.HasColumn(16, 60)
	.HasColumn(77)
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
to get the entities using `.GenerateEntitiesFromFile()`. These entities are lazily created
and filled as defined in the `AdaptTo<T>` method.

For instance:

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

# Example for Pipe files
There is another type of importer used in the Example file. This importer support pipe-separated value files.

To define the importer, this syntax is provided:
```C#
var valueSetsTextFileImporter = FileImporter
  .ForTextFile()
  .WithPipeDelimitedColumns()
  .IgnoringFirstLine()
	.HasColumn(0,  "ValueSetName")
	.HasColumn(1,  "ValueSetOid")
	.HasColumn(8,  "Code")
	.HasColumn(9,  "Description")
	.HasColumn(10, "CodeSystem")
   .AdaptTo<ValueSet>((e, columns) =>
   {
	   e.ValueSetName = columns["ValueSetName"];
	   e.ValueSetOid = columns["ValueSetOid"];
	   e.Code = columns["Code"];
	   e.Description = columns["Description"];
	   e.CodeSystem = columns["CodeSystem"];
   });
```

There is also the `.ConsideringMultilineValues()` fluent option, used just as `IgnoringFirstLine()`, to consider values that continue in next line, including the new line character.

The configuration above requires a `ValueSet` class like this:

```C#
public class ValueSet
{
	public string ValueSetName { get; set; }
	public string ValueSetOid { get; set; }
	public string Code { get; set; }
	public string Description { get; set; }
	public string CodeSystem { get; set; }
}
```

The importer defined above can adapt the file below to a collection of `ValueSet` entities. 

```
Value Set Name|Value Set OID|Definition Version|Expansion Version|Purpose: Clinical Focus|Purpose: Data Element Scope|Purpose: Inclusion Criteria|Purpose: Exclusion Criteria|Code|Description|Code System|Code System OID|Code System Version
Goal Achievement|2.16.840.1.113883.11.20.9.55|20170106|C-CDA R2.1 2018-06-15|The Goal Achievement value set contains concepts that describe a patient's progression (or lack thereof) toward a goal.|Goal attribute value in C-CDA template observation: identifier urn:oid:2.16.840.1.113883.10.20.22.4.110|The following concepts from SNOMED CT: Self(  390802008 \| Goal achieved) and DescendentsAndSelf(390801001 \| Goal not achieved).| |390801001|Goal not achieved (finding)|SNOMEDCT|2.16.840.1.113883.6.96|2018-03
Goal Achievement|2.16.840.1.113883.11.20.9.55|20170106|C-CDA R2.1 2018-06-15|The Goal Achievement value set contains concepts that describe a patient's progression (or lack thereof) toward a goal.|Goal attribute value in C-CDA template observation: identifier urn:oid:2.16.840.1.113883.10.20.22.4.110|The following concepts from SNOMED CT: Self(  390802008 \| Goal achieved) and DescendentsAndSelf(390801001 \| Goal not achieved).|only as noted in inclusion criteria|706906006|No progress toward goal (finding)|SNOMEDCT|2.16.840.1.113883.6.96|2018-03
Goal Achievement|2.16.840.1.113883.11.20.9.55|20170106|C-CDA R2.1 2018-06-15|The Goal Achievement value set contains concepts that describe a patient's progression (or lack thereof) toward a goal.|Goal attribute value in C-CDA template observation: identifier urn:oid:2.16.840.1.113883.10.20.22.4.110|The following concepts from SNOMED CT: Self(  390802008 \| Goal achieved) and DescendentsAndSelf(390801001 \| Goal not achieved).|only as noted in inclusion criteria|706905005|Goal not attainable (finding)|SNOMEDCT|2.16.840.1.113883.6.96|2018-03
Goal Achievement|2.16.840.1.113883.11.20.9.55|20170106|C-CDA R2.1 2018-06-15|The Goal Achievement value set contains concepts that describe a patient's progression (or lack thereof) toward a goal.|Goal attribute value in C-CDA template observation: identifier urn:oid:2.16.840.1.113883.10.20.22.4.110|The following concepts from SNOMED CT: Self(  390802008 \| Goal achieved) and DescendentsAndSelf(390801001 \| Goal not achieved).|only as noted in inclusion criteria|390802008|Goal achieved (finding)|SNOMEDCT|2.16.840.1.113883.6.96|2018-03
Clinical Substance|2.16.840.1.113762.1.4.1010.2|20180122|C-CDA R2.1 2018-06-15|Any substance that can be ordered or is included in a clinical record. This is not restricted to medications.|Clinical Substance ManufacturedProduct/manufacturedMaterial/code/translation/|As defined in grouped value sets|No drug classes|1148491|24 HR tramadol hydrochloride 300 MG Extended Release Oral Capsule [ConZip]|RXNORM|2.16.840.1.113883.6.88|2018-04
Clinical Substance|2.16.840.1.113762.1.4.1010.2|20180122|C-CDA R2.1 2018-06-15|Any substance that can be ordered or is included in a clinical record. This is not restricted to medications.|Clinical Substance ManufacturedProduct/manufacturedMaterial/code/translation/|As defined in grouped value sets|No drug classes|1148496|crizotinib 200 MG|RXNORM|2.16.840.1.113883.6.88|2018-04
Clinical Substance|2.16.840.1.113762.1.4.1010.2|20180122|C-CDA R2.1 2018-06-15|Any substance that can be ordered or is included in a clinical record. This is not restricted to medications.|Clinical Substance ManufacturedProduct/manufacturedMaterial/code/translation/|As defined in grouped value sets|No drug classes|1148497|crizotinib Oral Capsule|RXNORM|2.16.840.1.113883.6.88|2018-04
```
