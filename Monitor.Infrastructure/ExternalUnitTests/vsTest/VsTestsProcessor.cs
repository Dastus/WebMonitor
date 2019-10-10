using Monitor.Application.MonitoringChecks.Models;
using Monitor.Infrastructure.CommandsRunner;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Monitor.Infrastructure.ExternalUnitTests.MsTest
{
    public class VsTestsProcessor
    {
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
                await RunTestAsync(testName, path);
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
                var dirPath = new FileInfo(path).DirectoryName;
                var result = ParseTestResults(Path.Combine(dirPath, "TestResults", testName + ".trx"));
                testResult = result?.Results.FirstOrDefault();
            }
            catch (Exception ex)
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

        private async Task RunTestAsync(string testName, string dllPath)
        {
            var commandRunner = new ConsoleCommandsRunner();
            var logPath = Path.Combine(new FileInfo(dllPath).DirectoryName, "TestResults", testName + ".trx");

            string command = "CMD.exe";
            string arguments = String.Format("/C dotnet vstest {0} --Tests:\"{1}\" --logger:\"trx;LogFileName={2}\"", dllPath, testName, logPath);

            await commandRunner.RunProcessAsync(command, arguments);
        }

        private TestRun ParseTestResults(string path)
        {
            var file = new FileInfo(path);

            var deserializer = new XmlSerializer(typeof(TestRun));
            TextReader textReader = new StreamReader(file.FullName);
            var results = (TestRun)deserializer.Deserialize(textReader);
            textReader.Close();

            return results;
        }

        //private string ExcludeDllFileFromPath(string path) => new FileInfo(path).DirectoryName;//path.Substring(0, path.LastIndexOf("/"));

    }
}
