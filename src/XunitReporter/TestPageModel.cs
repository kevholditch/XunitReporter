using System.Collections.Generic;

namespace XunitReporter
{
    public class TestPageModel
    {
        public string PageTitle { get; set; }
        public List<TestAssemblyModel> TestAssemblyModels { get; set; }


    }
}