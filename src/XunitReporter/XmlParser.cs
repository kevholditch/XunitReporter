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
                   Total = node.GetAttributeInt("total"),
                   Passed = node.GetAttributeInt("passed"),
                   Failed = node.GetAttributeInt("failed"),
                   Skipped = node.GetAttributeInt("skipped"),
                   Time = node.GetAttributeDecimal("time"),
                   Errors = node.GetAttributeInt("errors")
                };

                foreach (var collectionNode in node.XPathSelectElements("collection"))
                {
                    var testCollection = new TestCollectionModel
                    {
                        Name = collectionNode.GetAttributeString("name"),
                        Total = collectionNode.GetAttributeInt("total"),
                        Passed = collectionNode.GetAttributeInt("passed"),
                        Failed = collectionNode.GetAttributeInt("failed"),
                        Skipped = collectionNode.GetAttributeInt("skipped"),
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
                            var stepPosition = int.Parse(Regex.Match(x.Fullname, @"\[(\d+)\]").Groups[1].Value);
                            
                            return new
                            {
                                TestName = split[0].Replace(x.Type, "").Replace("()", "").Replace(".", "").Trim(),
                                StepName = split[1].Trim(),
                                StepPosition = stepPosition,
                                Type = x.Type,
                                Result = x.Result
                            };
                        })
                        .GroupBy(x => new Tuple<string, string>(x.Type, x.TestName))
                        .Select(x => new TestModel
                        {
                            Name = x.Key.Item2,
                            TestSteps = x.OrderBy(s => s.StepPosition)
                                         .Select(s => new TestStep {Step = s.StepName, StepResult = s.Result})
                                         .ToList()
                        })
                        .ToList();

                    testAssembly.TestCollections.Add(testCollection);
                }

                result.Add(testAssembly);

            }

            return result;
        }
    }
}
