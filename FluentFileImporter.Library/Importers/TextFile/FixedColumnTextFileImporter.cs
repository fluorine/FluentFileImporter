using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FluentFileImporter.Importers.TextFile
{
    /// <inheritdoc/>
    public class FixedColumnTextFileImporter : IFixedColumnTextFileImporter
    {
        public IEnumerable<Tuple<int, int?>> DefinedColumns { get; protected set; }
        public bool FirstLineIgnored { get; protected set;  }

        private FixedColumnTextFileImporter(IEnumerable<Tuple<int,int?>> definedColumns, bool ignoreFirstLine)
        {
            DefinedColumns = definedColumns ?? new List<Tuple<int,int?>>();
            FirstLineIgnored = ignoreFirstLine;
        }

        public FixedColumnTextFileImporter()
        {
            DefinedColumns = new List<Tuple<int, int?>>();
        }

        public IFixedColumnTextFileImporter HasColumn(int index, int? length = null)
        {
            var column = Tuple.Create(index, length);
            return new FixedColumnTextFileImporter(
                new List<Tuple<int,int?>>(DefinedColumns) { column },
                FirstLineIgnored);
        }

        public IFixedColumnTextFileImporter IgnoringFirstLine(bool ignore = true)
        {
            return new FixedColumnTextFileImporter(DefinedColumns, ignore);
        }

        public IFileImporter<E> AdaptTo<E>(Action<E, IList<string>> entityAdapter) where E : new()
        {
            return new FixedColumnTextFileImporter<E>(this, entityAdapter);
        }
    }

    public class FixedColumnTextFileImporter<E> : FixedColumnTextFileImporter, IFileImporter<E> where E : new()
    {
        public Action<E, IList<string>> EntityAdapter { get; }

        public FixedColumnTextFileImporter(FixedColumnTextFileImporter textFileImporter, Action<E, IList<string>> entityAdapter)
        {
            DefinedColumns = textFileImporter.DefinedColumns;
            FirstLineIgnored = textFileImporter.FirstLineIgnored;
            EntityAdapter = entityAdapter;
        }

        public IEnumerable<E> GenerateEntitiesFromFile(string filePath)
        {
            // Read all lines from file
            IEnumerable<string> lines = File.ReadLines(filePath);
            
            // Ignore first line if required.
            if(FirstLineIgnored)
            {
                lines = lines.Skip(1);
            }

            // Traverse the file's lines
            foreach (var line in lines)
            {
                // Get column values
                var columns = new List<string>(DefinedColumns.Count());
                foreach (var definedColumn in DefinedColumns)
                {
                    columns.Add(line.Substring(
                        definedColumn.Item1,
                        definedColumn.Item2 ?? line.Length - definedColumn.Item1));
                }

                // Create and fill entity
                var entity = new E();
                EntityAdapter(entity, columns);
                yield return entity;
            }
        }
    }
}
