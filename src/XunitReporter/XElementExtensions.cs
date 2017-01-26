using System;
using System.Xml.Linq;

namespace XunitReporter
{
    public static class XElementExtensions
    {
        public static string GetAttributeString(this XElement xElement, string attributeName)
        {
            var xAttribute = xElement.Attribute(attributeName);
            if (xAttribute == null)
                throw new ArgumentException(string.Format("attributed named {0} does not exist", attributeName));

            return xAttribute.Value;
        }

        public static int GetAttributeInt(this XElement xElement, string attributeName)
        {
            var xAttribute = xElement.Attribute(attributeName);
            if (xAttribute == null)
                throw new ArgumentException(string.Format("attributed named {0} does not exist", attributeName));

            return int.Parse(xAttribute.Value);
        }

        public static decimal GetAttributeDecimal(this XElement xElement, string attributeName)
        {
            var xAttribute = xElement.Attribute(attributeName);
            if (xAttribute == null)
                throw new ArgumentException(string.Format("attributed named {0} does not exist", attributeName));

            return decimal.Parse(xAttribute.Value);
        }

        public static TestResult GetAttributeTestResult(this XElement xElement, string attributeName)
        {
            var xAttribute = xElement.Attribute(attributeName);
            if (xAttribute == null)
                throw new ArgumentException(string.Format("attributed named {0} does not exist", attributeName));

            return (TestResult)Enum.Parse(typeof(TestResult), xAttribute.Value);
        }
    }
}