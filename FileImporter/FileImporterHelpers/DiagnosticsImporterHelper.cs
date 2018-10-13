using FluentFileImporter.Example.Entities;

namespace FluentFileImporter.Example.FileImporterHelper
{
    class DiagnosticsImporterHelper
    {
        public IFileImporter<Icd10Diagnostic> DiagnosticsTextFileImporter { get; }
        
        public DiagnosticsImporterHelper()
        {
            // Create importer
            DiagnosticsTextFileImporter = FileImporter
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
        }
    }
}
