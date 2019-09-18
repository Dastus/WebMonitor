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
    public class MsTestsProcessor
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
                var result = ParseTestResults(Path.Combine(path, "TestResults", testName));
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

        private async Task RunTestAsync(string testName, string path)
        {
            var commandRunner = new ConsoleCommandsRunner();

            string command = "CMD.exe";
            string arguments = String.Format("/C dotnet test {0} --no-build --filter Name={1} --logger \"trx;LogFileName={1}\"", path, testName);

            await commandRunner.RunProcessAsync(command, arguments);
        }

        //private void RunAllTests(string path)
        //{
        //    string strCmdText = String.Format(@"/C dotnet test {0} --logger:trx", path);
        //    var cmd = Process.Start("CMD.exe", strCmdText);
        //    cmd.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

        //    cmd.WaitForExit();
        //}

        private TestRun ParseTestResults(string path)
        {
            var file = new FileInfo(path);

            var deserializer = new XmlSerializer(typeof(TestRun));
            TextReader textReader = new StreamReader(file.FullName);
            var results = (TestRun)deserializer.Deserialize(textReader);
            textReader.Close();

            return results;
        }

        //private TestRun ParseTestResults(string path)
        //{
        //    var file = GetLatestFile(path);

        //    var deserializer = new XmlSerializer(typeof(TestRun));
        //    TextReader textReader = new StreamReader(file.FullName);
        //    var results = (TestRun)deserializer.Deserialize(textReader);
        //    textReader.Close();

        //    return results;
        //}

        //private FileInfo GetLogFile(string path, string testName)
        //{
        //    var files = new DirectoryInfo(Path.Combine(path, "TestResults")).GetFiles();

        //    return files.Where(f => f.Name == testName).LastOrDefault();
        //}

        //private FileInfo GetLatestFile(string path)
        //{
        //    var files = new DirectoryInfo(Path.Combine(path, "TestResults")).GetFiles();
        //    FileInfo file = null;

        //    var lastUpdated = DateTime.MinValue;
        //    foreach (var f in files)
        //    {
        //        if (f.LastWriteTime > lastUpdated)
        //        {
        //            file = f;
        //            lastUpdated = file.LastWriteTime;
        //        }
        //    }

        //    return file;
        //}
    }
}
