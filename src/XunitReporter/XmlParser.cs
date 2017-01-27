using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;

namespace XunitReporter
{
    public class XmlParser
    {
        public static List<TestAssemblyModel> Parse(string xml)
        {
            var doc = XDocument.Load(new StringReader(xml));

            var result = new List<TestAssemblyModel>();

            foreach (var node in doc.XPathSelectElements("/assemblies/assembly"))
            {
                var testAssembly = new TestAssemblyModel
                {
                   Name = node.GetAttributeString("name"),
                   Time = node.GetAttributeDecimal("time")  
                };

                foreach (var collectionNode in node.XPathSelectElements("collection"))
                {
                    var testCollection = new TestCollectionModel
                    {
                        Name = collectionNode.GetAttributeString("name"),
                        Time = collectionNode.GetAttributeDecimal("time")
                    };

                    testCollection.Tests = collectionNode.XPathSelectElements("test")
                        .Select(x => new
                        {
                            Fullname = x.GetAttributeString("name"),
                            Result = x.GetAttributeTestResult("result"),
                            Type = x.GetAttributeString("type")
                        })
                        .Select(x =>
                        {
                            var split = Regex.Split(x.Fullname, @"\[\d+\]");


                            if (Regex.IsMatch(x.Fullname, @"\[(\d+)\]"))
                            {
                                var stepPosition = int.Parse(Regex.Match(x.Fullname, @"\[(\d+)\]").Groups[1].Value);

                                return new
                                {
                                    TestName = split[0].Replace(x.Type, "").Replace("()", "").Replace(".", "").Trim(),
                                    StepName = split[1].Trim(),
                                    StepPosition = stepPosition,
                                    Type = x.Type,
                                    Result = x.Result
                                };
                            }
                            else
                            {
                                return new
                                {
                                    TestName = split[0].Replace(x.Type, "").Replace("()", "").Replace(".", "").Trim(),
                                    StepName = split[0].Trim(),
                                    StepPosition = 0,
                                    Type = x.Type,
                                    Result = x.Result
                                };
                            }

                            
                        })
                        .GroupBy(x => new Tuple<string, string>(x.Type, x.TestName))
                        .Select(x =>
                        {
                            var t = new TestModel
                            {
                                Name = x.Key.Item2,
                                TestSteps = x.OrderBy(s => s.StepPosition)
                                    .Select(s => new TestStepModel {Step = s.StepName, StepResult = s.Result})
                                    .ToList()
                            };

                            t.TestResult = t.TestSteps.Any(s => s.StepResult == TestResult.Fail)
                                ? TestResult.Fail
                                : TestResult.Pass;


                            if (t.TestSteps.All(s => s.StepResult == TestResult.Skip))
                            {
                                t.TestResult = TestResult.Skip;
                                t.TestSteps = new List<TestStepModel>();
                            }

                            return t;
                        })
                        .ToList();

                    testAssembly.TestCollections.Add(testCollection);
                }

                result.Add(testAssembly);

            }

            return WithCorrectTotals(result);
            
        }

        private static List<TestAssemblyModel> WithCorrectTotals(IEnumerable<TestAssemblyModel> testAssemblyModels)
        {
            return testAssemblyModels.Select(assemblyModel =>
            {
                var testCollections = assemblyModel.TestCollections.Select(collection => new TestCollectionModel
                {
                    Name = collection.Name,
                    Total = collection.Tests.Count,
                    Passed = collection.Tests.Count(t => t.TestResult == TestResult.Pass),
                    Failed = collection.Tests.Count(t => t.TestResult == TestResult.Fail),
                    Skipped = collection.Tests.Count(t => t.TestResult == TestResult.Skip),
                    Tests = collection.Tests,
                    Time = collection.Time
                }).ToList();

                return new TestAssemblyModel()
                {
                    Name = assemblyModel.Name,
                    Total = testCollections.Select(c => c.Total).Sum(),
                    Passed = testCollections.Select(c => c.Passed).Sum(),
                    Failed = testCollections.Select(c => c.Failed).Sum(),
                    Skipped = testCollections.Select(c => c.Skipped).Sum(),
                    TestCollections = testCollections,
                    Time = assemblyModel.Time,
                    Errors = assemblyModel.Errors

                };
            }).ToList();
        }
    }
}
