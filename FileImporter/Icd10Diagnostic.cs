namespace FluentFileImporter.Example.Icd10Diagnostics
{
    public class Icd10Diagnostic
    {
        public int Order { get; set; }

        public string Code { get; set; }

        public bool ValidForSubmission { get; set; }

        public string ShortDescription { get; set; }

        public string LongDescription { get; set; }
    }
}
