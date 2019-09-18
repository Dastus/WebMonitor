using System;
using System.Xml.Serialization;

namespace Monitor.Infrastructure.ExternalUnitTests.nUnit
{
    public class NUnitTestModels
    {
        [XmlRoot("test-results")]
        public class TestResults
        {
            [XmlAttribute("name")]
            public string Name { get; set; }

            [XmlAttribute("total")]
            public int Total { get; set; }

            [XmlAttribute("errors")]
            public int Errors { get; set; }

            [XmlAttribute("failures")]
            public int Failures { get; set; }

            [XmlAttribute("not-run")]
            public int NotRun { get; set; }

            [XmlAttribute("inconclusive")]
            public int Inconclusive { get; set; }

            [XmlAttribute("ignored")]
            public int Ignored { get; set; }

            [XmlAttribute("skipped")]
            public int Skipped { get; set; }

            [XmlAttribute("invalid")]
            public int Invalid { get; set; }

            [XmlAttribute("date")]
            public DateTime Date { get; set; }

            [XmlAttribute("time")]
            public DateTime DateTime { get; set; }

            [XmlElement("test-suite")]
            public TestSuite TestSuite { get; set; }
        }

        public class TestSuite : TestBase
        {
            [XmlAttribute("type")]
            public string Type { get; set; }

            [XmlArray("results")]
            [XmlArrayItem("test-suite", Type = typeof(TestSuite))]
            [XmlArrayItem("test-case", Type = typeof(TestCase))]
            public TestBase[] Results { get; set; }
        }

        public class TestCase : TestBase
        {
        }

        public class TestBase
        {
            [XmlAttribute("name")]
            public string Name { get; set; }

            [XmlAttribute("executed")]
            public string Executed { get; set; }

            [XmlAttribute("result")]
            public string Result { get; set; }

            [XmlAttribute("time")]
            public string Time { get; set; }

            [XmlAttribute("asserts")]
            public int Asserts { get; set; }

            public bool Success { get => (Result == "Success"); }
        }
    }
}
