using Monitor.Application.Interfaces;
using Monitor.Application.MonitoringChecks.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Monitor.Infrastructure.ExternalUnitTests
{    
    public class UnitTestsProcessorService : IUnitTestsProcessorService
    {
        //TODO: make async
        public async Task<CheckState> ExecuteUnitTest(string testName, string path)
        {
            UnitTestResult testResult;

            var checkState = new CheckState
            {
                Status = StatusesEnum.CRITICAL,
                LastCheckTime = DateTime.Now
            };

            var sw = new Stopwatch();
            sw.Start();

            try
            {
                await RunTestAsync(path, testName);
            }
            catch (Exception ex)
            {
                sw.Stop();
                checkState.Description = "Проблема во время запуска теста: " + ex.Message;
                checkState.ExecutionDuration = sw.ElapsedMilliseconds;
                return checkState;
            }

            try
            {
                var result = ParseTestResults(path, testName);
                testResult = result?.Results.FirstOrDefault();
            }
            catch(Exception ex)
            {
                sw.Stop();
                checkState.Description = "Проблема во время обработки результатов теста: " + ex.Message;
                checkState.ExecutionDuration = sw.ElapsedMilliseconds;
                return checkState;
            }

            sw.Stop();
            checkState.Description = testResult.Success ? 
                $"Тест {testName} выполнен успешно" : 
                testResult.Output.ErrorInfo.Message;
            checkState.Status = testResult.Success ? 
                StatusesEnum.OK : StatusesEnum.CRITICAL;
            checkState.ExecutionDuration = sw.ElapsedMilliseconds;

            return checkState;
        }

        public async Task RunTestAsync(string testName, string path)
        {
            string strCmdText = String.Format("/C dotnet test {0} --no-build --filter Name={1} --logger \"trx;LogFileName={1}\"", path, testName);

            var process = new Process
            {
                EnableRaisingEvents = true,
                StartInfo =
                {
                    FileName = "CMD.exe",
                    Arguments = strCmdText,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                },
            };

            using (process)
            {
                var tsc = new TaskCompletionSource<object>();

                process.Exited += (sender, args) =>
                {
                    tsc.SetResult(process.ExitCode);
                };

                process.Start();

                await Task.Run(() => tsc.Task);
            }
        }

        private void RunTest(string path, string testName)
        {
            string strCmdText = String.Format("/C dotnet test {0} --no-build --filter Name={1} --logger \"trx;LogFileName={1}\"", path, testName);
            var cmd = Process.Start("CMD.exe", strCmdText);
            cmd.WaitForExit();
        }

        private void RunAllTests(string path)
        {
            string strCmdText = String.Format(@"/C dotnet test {0} --logger:trx", path);
            var cmd = Process.Start("CMD.exe", strCmdText);
            cmd.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

            cmd.WaitForExit();
        }

        private TestRun ParseTestResults(string path, string testName)
        {
            var file = GetLogFile(path, testName);

            var deserializer = new XmlSerializer(typeof(TestRun));
            TextReader textReader = new StreamReader(file.FullName);
            var results = (TestRun)deserializer.Deserialize(textReader);
            textReader.Close();

            return results;
        }

        private TestRun ParseTestResults(string path)
        {
            var file = GetLatestFile(path);

            var deserializer = new XmlSerializer(typeof(TestRun));
            TextReader textReader = new StreamReader(file.FullName);
            var results = (TestRun)deserializer.Deserialize(textReader);
            textReader.Close();

            return results;
        }

        private FileInfo GetLogFile(string path, string testName)
        {
            var files = new DirectoryInfo(Path.Combine(path, "TestResults")).GetFiles();

            return files.Where(f => f.Name == testName).LastOrDefault();
        }

        private FileInfo GetLatestFile(string path)
        {
            var files = new DirectoryInfo(Path.Combine(path, "TestResults")).GetFiles();
            FileInfo file = null;

            var lastUpdated = DateTime.MinValue;
            foreach (var f in files)
            {
                if (f.LastWriteTime > lastUpdated)
                {
                    file = f;
                    lastUpdated = file.LastWriteTime;
                }
            }

            return file;
        }
    }

    #region models
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
    #endregion
}
