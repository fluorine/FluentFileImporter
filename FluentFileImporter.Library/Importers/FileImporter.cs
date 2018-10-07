using FileImporter.TextFile;

namespace FileImporter
{
    public static class FileImporter
    {
        /// <summary>
        /// Get a new <see cref="ITextFileImporter"/> instance to configure.
        /// </summary>
        /// <returns></returns>
        public static ITextFileImporter ToImportTextFile() => new TextFileImporter();
    }
}
