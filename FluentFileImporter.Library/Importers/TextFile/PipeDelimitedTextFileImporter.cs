using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace FluentFileImporter.Importers.TextFile
{
    /// <inheritdoc/>
    public class PipeDelimitedTextFileImporter : IPipeDelimitedTextFileImporter
    {
        public IDictionary<int, string> DefinedColumns { get; protected set; }

        public bool FirstLineIgnored { get; protected set; }


        public PipeDelimitedTextFileImporter() => DefinedColumns = new Dictionary<int, string>();

        private PipeDelimitedTextFileImporter(IDictionary<int,string> definedColumns, bool ignoreFirstLine)
        {
            DefinedColumns = definedColumns ?? new Dictionary<int, string>();
            FirstLineIgnored = ignoreFirstLine;
        }

        public IPipeDelimitedTextFileImporter HasColumn(int index, string named)
        {
            return new PipeDelimitedTextFileImporter(
                new Dictionary<int, string>(DefinedColumns) { { index, named } },
                FirstLineIgnored);
        }

        public IPipeDelimitedTextFileImporter IgnoringFirstLine(bool ignore = true)
        {
            return new PipeDelimitedTextFileImporter(DefinedColumns, ignore);
        }

        public IFileImporter<E> AdaptTo<E>(Action<E, IDictionary<string, string>> entityAdapter) where E : new()
        {
            return new PipeDelimitedTextFileImporter<E>(this, entityAdapter);
        }
    }

    public class PipeDelimitedTextFileImporter<E> : PipeDelimitedTextFileImporter, IFileImporter<E> where E : new()
    {
        public Action<E, IDictionary<string,string>> EntityAdapter { get; }

        public PipeDelimitedTextFileImporter(PipeDelimitedTextFileImporter textFileImporter, Action<E, IDictionary<string,string>> entityAdapter)
        {
            DefinedColumns = textFileImporter.DefinedColumns;
            FirstLineIgnored = textFileImporter.FirstLineIgnored;
            EntityAdapter = entityAdapter;
        }

        public static Lazy<Regex> SplitRegex =
            new Lazy<Regex>(() => new Regex(@"(?<!(?<!\\)*\\)\|", RegexOptions.Compiled));

        public IEnumerable<E> GenerateEntitiesFromFile(string filePath)
        {
            // Read all lines from file
            var lines = File.ReadLines(filePath);

            // Ignore first line if required.
            if (FirstLineIgnored)
            {
                lines = lines.Skip(1);
            }
            // Traverse the file's lines
            foreach (var line in lines)
            {
                // Split line
                var tokens = SplitRegex.Value.Split(line);

                // Store column values in dictionary
                var columns = new Dictionary<string,string>(DefinedColumns.Count());

                foreach (var definedColumn in DefinedColumns)
                {
                    // Fill dictionary with key-value pairs
                    columns[definedColumn.Value] = tokens[definedColumn.Key];
                }

                // Create and fill entity
                var entity = new E();
                EntityAdapter(entity, columns);

                // Return them as needed
                yield return entity;
            }
        }
    }
}
