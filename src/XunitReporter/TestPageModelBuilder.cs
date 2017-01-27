using System.IO;

namespace XunitReporter
{
    public class TestPageModelBuilder
    {
        private string _xmlPath;
        private string _pageTitle;
        private TestPageModelBuilder()
        {
            
        }

        public static TestPageModelBuilder Create()
        {
            return new TestPageModelBuilder();
        }

        public TestPageModelBuilder WithTestXmlFromPath(string value)
        {
            _xmlPath = value;
            return this;
        }

        public TestPageModelBuilder WithPageTitle(string value)
        {
            _pageTitle = value;
            return this;
        }

        public TestPageModel Build()
        {
            return new TestPageModel
            {
                PageTitle = _pageTitle,
                TestAssemblyModels = XmlParser.Parse(File.ReadAllText(_xmlPath))
            };
        }


    }
}
