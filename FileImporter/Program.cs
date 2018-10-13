using FluentFileImporter.Example.FileImporterHelper;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace FluentFileImporter.Example
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
            PrintICD10Codes();

            PrintValueSetCodes();

            Console.ReadKey();
        }

        private static void PrintICD10Codes()
        {
            // Title
            Console.WriteLine("##### ICD10 Codes ####");

            // Use importer for ICD10 Codes, 
            // which imports them from a fixed-width
            // column file with diagnostics.
            var importerHelper = new DiagnosticsImporterHelper();
            var filename = "Data/icd10cm_order_2019.txt";
            var entities = importerHelper
                .DiagnosticsTextFileImporter
                .GenerateEntitiesFromFile(filename)
                .ToList();

            // Print ICD10 codes
            for (int i = 0; i < entities.Count(); i++)
            {
                if (i % 10000 != 0) continue;

                var entity = entities[i];

                Console.WriteLine($"The {(entity.ValidForSubmission ? "billable" : "non-billable")}" +
                                   $" code {entity.Code} is described as \n  {entity.LongDescription}.\n");
            }
        }

        private static void PrintValueSetCodes()
        {
            // Title
            Console.WriteLine("\n##### Value Set Codes ####");

            // Use importer for ICD10 Codes, 
            // which imports them from a fixed-width
            // column file with diagnostics.
            var importerHelper = new ValueSetsImporterHelper();
            var filename = "Data/valuesets.txt";
            var entities = importerHelper
                .ValueSetsTextFileImporter
                .GenerateEntitiesFromFile(filename)
                .ToList();

            // Print value sets
            foreach(var entity in entities)
            {
                Console.WriteLine(entity.ToString());
            }
        }
    }
}
