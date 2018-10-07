using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace FileImporter
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create importer
            var fileTextImporterForDiagnostics = FileImporter
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

            // Use importer
            var filename = "icd10cm_order_2019.txt";
            var entities = fileTextImporterForDiagnostics
                .GenerateEntitiesFromFile(filename)
                .ToList();

            // Print codes
            for (int i = 0; i < entities.Count(); i++)
            {
                if (i % 10000 != 0) continue;

                var entity = entities[i];
                
                Console.WriteLine($"The {(entity.ValidForSubmission ? "billable" : "non-billable")}" +
                                   $" code {entity.Code} is described as \n  {entity.LongDescription}.\n");
            }

            Console.ReadKey();
        }
    }
}
