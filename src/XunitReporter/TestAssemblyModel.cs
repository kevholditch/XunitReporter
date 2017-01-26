using System.Collections.Generic;

namespace XunitReporter
{
    public class TestAssemblyModel
    {
        public TestAssemblyModel()
        {
            TestCollections = new List<TestCollectionModel>();
        }

        public string Name { get; set; }
        public int Total { get; set; }
        public int Passed { get; set; }
        public int Failed { get; set; }
        public int Skipped { get; set; }
        public int Errors { get; set; }
        public decimal Time { get; set; }
        public List<TestCollectionModel> TestCollections { get; private set; }
    }
}