namespace FileImporter
{
    public static class FileImporter
    {
        public static ITextFileImporter ToImportTextFile()
        {
            return new TextFileImporter();
        }
    }
}
