using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileImporter
{
    public class TextFileImporter: ITextFileImporter
    {
        public IEnumerable<Tuple<int, int?>> DefinedColumns { get; protected set; }
        public bool FirstLineIgnored { get; protected set;  }

        private TextFileImporter(IEnumerable<Tuple<int,int?>> definedColumns, bool ignoreFirstLine)
        {
            DefinedColumns = definedColumns ?? new List<Tuple<int,int?>>();
            FirstLineIgnored = ignoreFirstLine;
        }

        public TextFileImporter()
        {
            DefinedColumns = new List<Tuple<int, int?>>();
        }

        public ITextFileImporter WithColumn(int index, int? length = null)
        {
            var column = Tuple.Create(index, length);
            return new TextFileImporter(
                new List<Tuple<int,int?>>(DefinedColumns) { column },
                FirstLineIgnored);
        }

        public ITextFileImporter IgnoreFirstLine(bool ignore)
        {
            return new TextFileImporter(DefinedColumns, ignore);
        }

        public IFileImporter<E> AdaptTo<E>(Action<E, IList<string>> entityAdapter) where E : new()
        {
            return new TextFileImporter<E>(this, entityAdapter);
        }
    }

    public class TextFileImporter<E> : TextFileImporter, IFileImporter<E> where E : new()
    {
        public Action<E, IList<string>> EntityAdapter { get; }

        public TextFileImporter(TextFileImporter textFileImporter, Action<E, IList<string>> entityAdapter)
        {
            DefinedColumns = textFileImporter.DefinedColumns;
            FirstLineIgnored = textFileImporter.FirstLineIgnored;
            EntityAdapter = entityAdapter;
        }

        public IEnumerable<E> GenerateEntitiesFromFile(string filePath)
        {
            foreach (var line in File.ReadAllLines(filePath))
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
