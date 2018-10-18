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

        public bool MultilineValuesConsidered { get; protected set; }

        public PipeDelimitedTextFileImporter() => DefinedColumns = new Dictionary<int, string>();

        private PipeDelimitedTextFileImporter(IDictionary<int,string> definedColumns, bool ignoreFirstLine, bool considerMultilineValues)
        {
            DefinedColumns = definedColumns ?? new Dictionary<int, string>();
            FirstLineIgnored = ignoreFirstLine;
            MultilineValuesConsidered = considerMultilineValues;
        }

        public IPipeDelimitedTextFileImporter HasColumn(int index, string named)
        {
            return new PipeDelimitedTextFileImporter(
                new Dictionary<int, string>(DefinedColumns) { { index, named } },
                FirstLineIgnored, MultilineValuesConsidered);
        }

        public IPipeDelimitedTextFileImporter IgnoringFirstLine(bool ignore = true)
        {
            return new PipeDelimitedTextFileImporter(DefinedColumns, ignore, MultilineValuesConsidered);
        }

        public IFileImporter<E> AdaptTo<E>(Action<E, IDictionary<string, string>> entityAdapter) where E : new()
        {
            return new PipeDelimitedTextFileImporter<E>(this, entityAdapter);
        }

        public IPipeDelimitedTextFileImporter ConsideringMultilineValues(bool considerMultilineValues = true)
        {
            return new PipeDelimitedTextFileImporter(DefinedColumns, FirstLineIgnored, considerMultilineValues);
        }
    }

    public class PipeDelimitedTextFileImporter<E> : PipeDelimitedTextFileImporter, IFileImporter<E> where E : new()
    {
        public Action<E, IDictionary<string,string>> EntityAdapter { get; }

        public PipeDelimitedTextFileImporter(PipeDelimitedTextFileImporter textFileImporter, Action<E, IDictionary<string,string>> entityAdapter)
        {
            DefinedColumns = textFileImporter.DefinedColumns;
            FirstLineIgnored = textFileImporter.FirstLineIgnored;
            MultilineValuesConsidered = textFileImporter.MultilineValuesConsidered;
            EntityAdapter = entityAdapter;
        }

        public static Lazy<Regex> SplitRegex =
            new Lazy<Regex>(() => new Regex(@"(?<!(?<!\\)*\\)\|", RegexOptions.Compiled));

        public IEnumerable<E> GenerateEntitiesFromFile(string filePath)
        {
            // Read all lines from file
            var lines = File.ReadLines(filePath);
            string firstLine;

            // Ignore first line if required.
            firstLine = lines?.FirstOrDefault();
            if (FirstLineIgnored)
            {
                lines = lines.Skip(1);
            }

            // Store tokens of broken line
            string[] accumulatedTokens = null;

            // Count columns assuming first line is well formed
            var columnsCount = SplitRegex.Value.Split(firstLine).Count();//DefinedColumns.Count();

            // Traverse the file's lines
            foreach (var line in lines)
            {
                // Split line
                var tokens = SplitRegex.Value.Split(line);

                // If option to glue tokens is on, add tokens here
                if (MultilineValuesConsidered
                    && columnsCount > tokens.Length)
                {
                    if (accumulatedTokens == null || accumulatedTokens.Length == 0)
                    {
                        accumulatedTokens = tokens;
                    }
                    else
                    {
                        // Append last accumulated token to first token
                        accumulatedTokens[accumulatedTokens.Length - 1]
                            = accumulatedTokens[accumulatedTokens.Length - 1] + Environment.NewLine + tokens[0];

                        if (tokens.Length == 1) continue;

                        accumulatedTokens = accumulatedTokens
                            .Concat(tokens.Skip(1).ToArray())
                            .ToArray();
                    }

                    if (accumulatedTokens.Length >= DefinedColumns.Count)
                    {
                        tokens = accumulatedTokens;
                        accumulatedTokens = null;
                    }
                    else
                    {
                        continue;
                    }
                }


                // Store column values in dictionary
                var columns = new Dictionary<string,string>(columnsCount);

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
