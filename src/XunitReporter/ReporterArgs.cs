using PowerArgs;

namespace XunitReporter
{
    public class ReporterArgs
    {
        [ArgRequired(PromptIfMissing = true)]
        public string Xml { get; set; }

        [ArgRequired(PromptIfMissing = true)]
        public string Html { get; set; }

        public string PageTitle { get; set; }
    }
}