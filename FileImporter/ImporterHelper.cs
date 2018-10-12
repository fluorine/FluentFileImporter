using System;
using System.Collections.Generic;
using System.Text;

namespace FluentFileImporter.Example.Icd10Diagnostics
{
    class ImporterHelper
    {
        public IFileImporter<Icd10Diagnostic> DiagnosticsTextFileImporter { get; }
        
        public ImporterHelper()
        {
            // Create importer
            DiagnosticsTextFileImporter = FileImporter
              .ToImportTextFile()
              .IgnoreFirstLine(false)
                .WithColumn(0, 5)
                .WithColumn(6, 7)
                .WithColumn(14, 1)
                .WithColumn(16, 60)
                .WithColumn(77)
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
