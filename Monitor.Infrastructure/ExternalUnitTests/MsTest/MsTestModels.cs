using System;
using System.Xml.Serialization;

namespace Monitor.Infrastructure.ExternalUnitTests.MsTest
{
    [XmlRoot("TestRun", Namespace = "http://microsoft.com/schemas/VisualStudio/TeamTest/2010")]
    public class TestRun
    {
        [XmlArray("Results")]
        public UnitTestResult[] Results { get; set; }
    }

    public class UnitTestResult
    {
        private string _durationString;

        [XmlAttribute(AttributeName = "testName")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "duration")]
        public string DurationStr
        {
            private get => _durationString;
            set => _durationString = value;
        }

        public TimeSpan Duration
        {
            get => TimeSpan.Parse(_durationString);
        }

        [XmlAttribute(AttributeName = "startTime")]
        public DateTime StartTime { get; set; }

        [XmlAttribute(AttributeName = "outcome")]
        public string Result { get; set; }

        [XmlAttribute(AttributeName = "executionId")]
        public Guid ExecutionId { get; set; }

        public Output Output { get; set; }

        public bool Success { get => Result == "Passed"; }
    }

    public class Output
    {
        public ErrorInfo ErrorInfo { get; set; }
    }

    public class ErrorInfo
    {
        public string Message { get; set; }
        public string StackTrace { get; set; }
    }
}
