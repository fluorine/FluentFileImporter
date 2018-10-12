using FluentFileImporter.Example.Icd10Diagnostics;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace FluentFileImporter.Example.Icd10Diagnostics
{
    class Program
    {
        /// <summary>
        /// This example defines a Fluent file importer
        /// to adapt all ICD10 diagnostics from a text file
        /// to a collection of typed entities.
        /// 
        /// See README.md for details.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            var importerHelper = new ImporterHelper();

            // Use importer
            var filename = "icd10cm_order_2019.txt";
            var entities = importerHelper
                .DiagnosticsTextFileImporter
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
