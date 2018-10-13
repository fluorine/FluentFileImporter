using FluentFileImporter.Importers.TextFile;

namespace FluentFileImporter
{
    public static class FileImporter
    {
        /// <summary>
        /// Get a new <see cref="IFixedColumnTextFileImporter"/> instance to configure.
        /// </summary>
        /// <returns></returns>
        public static TextFileImporter ForTextFile()
        {
            return new TextFileImporter();
        }
    }

    public class TextFileImporter
    {
        public IFixedColumnTextFileImporter WithFixedWidthColumns()
        {
            return new FixedColumnTextFileImporter();
        }

        public IPipeDelimitedTextFileImporter WithPipeDelimitedColumns()
        {
            return new PipeDelimitedTextFileImporter();
        }
    }
}
