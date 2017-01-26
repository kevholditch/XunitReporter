using System.Collections.Generic;

namespace XunitReporter
{
    public class TestModel
    {
        public string Name { get; set; }
        public TestResult TestResult { get; set; }
        public List<TestStep> TestSteps { get; set; }
    }
}