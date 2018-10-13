using FileImporter.Example.Entities;

namespace FluentFileImporter.Example.FileImporterHelper
{
    class ValueSetsImporterHelper
    {
        public IFileImporter<ValueSet> ValueSetsTextFileImporter { get; }
        
        public ValueSetsImporterHelper()
        {
            // Create importer
            ValueSetsTextFileImporter = FileImporter
              .ForTextFile()
              .WithPipeDelimitedColumns()
              .IgnoringFirstLine()
                .HasColumn(0, "ValueSetName")
                .HasColumn(1, "ValueSetOid")
                .HasColumn(2, "DefinitionVersion")
                .HasColumn(3, "ExpansionVersion")
                .HasColumn(4, "PurposeClinicalFocus")
                .HasColumn(5, "PurposeDataElementScope")
                .HasColumn(6, "PurposeInclusionCriteria")
                .HasColumn(7, "PurposeExclusionCriteria")
                .HasColumn(8, "Code")
                .HasColumn(9, "Description")
                .HasColumn(10, "CodeSystem")
                .HasColumn(11, "CodeSystemOID")
                .HasColumn(12, "CodeSystemVersion")
               .AdaptTo<ValueSet>((e, columns) =>
               {
                   e.ValueSetName = ProcessValue(columns["ValueSetName"]);
                   e.ValueSetOid = ProcessValue(columns["ValueSetOid"]);
                   e.DefinitionVersion = ProcessValue(columns["DefinitionVersion"]);
                   e.ExpansionVersion = ProcessValue(columns["ExpansionVersion"]);
                   e.PurposeClinicalFocus = ProcessValue(columns["PurposeClinicalFocus"]);
                   e.PurposeDataElementScope = ProcessValue(columns["PurposeDataElementScope"]);
                   e.PurposeInclusionCriteria = ProcessValue(columns["PurposeInclusionCriteria"]);
                   e.PurposeExclusionCriteria = ProcessValue(columns["PurposeExclusionCriteria"]);
                   e.Code = ProcessValue(columns["Code"]);
                   e.Description = ProcessValue(columns["Description"]);
                   e.CodeSystem = ProcessValue(columns["CodeSystem"]);
                   e.CodeSystemOID = ProcessValue(columns["CodeSystemOID"]);
                   e.CodeSystemVersion = ProcessValue(columns["CodeSystemVersion"]);
               });
        }

        private string ProcessValue(string input)
        {
            // Remove scaped characters
            var value = input.Replace(@"\|", "|");

            // Return null if string is blank or empty
            return string.IsNullOrWhiteSpace(value) ? null : value;
        }
    }
}
