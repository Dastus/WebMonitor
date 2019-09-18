using Monitor.Application.MonitoringChecks.Models;
using Monitor.Infrastructure.CommandsRunner;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using static Monitor.Infrastructure.ExternalUnitTests.nUnit.NUnitTestModels;

namespace Monitor.Infrastructure.ExternalUnitTests.nUnit
{
    public class NUnitTestsProcessor
    {
        const string nUnitTestResultsStorePath = @"E:\Vodolazkiy\test";

        public async Task<CheckState> ExecuteUnitTest(string testName, string path)
        {
            TestCase testResult;

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
                var results = ParseTestResults(Path.Combine(nUnitTestResultsStorePath, testName + ".xml"));
                testResult = GetTestCasesResult(results)?.FirstOrDefault();
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
                $"Тест {testName} пройден успешно" :
                $"Тест {testName} не пройден";
            checkState.Status = testResult.Success ?
                StatusesEnum.OK : StatusesEnum.CRITICAL;
            checkState.ExecutionDuration = sw.ElapsedMilliseconds;

            return checkState;
        }

        private async Task RunTestAsync(string path, string testName)
        {
            var commandRunner = new ConsoleCommandsRunner();

            string command = "CMD.exe";
            string arguments = string.Format(@"/C nunit3-console.exe {0} --test={1} -result {2}\{1}.xml;format=nunit2 --labels=All", path, testName, nUnitTestResultsStorePath);

            await commandRunner.RunProcessAsync(command, arguments);
        }

        private TestResults ParseTestResults(string path)
        {
            var file = new FileInfo(Path.Combine(path));

            var deserializer = new XmlSerializer(typeof(TestResults));
            TextReader textReader = new StreamReader(file.FullName);
            var results = (TestResults)deserializer.Deserialize(textReader);
            textReader.Close();

            return results;
        }

        private List<TestCase> GetTestCasesResult(TestResults results)
        {
            var testCases = new List<TestCase>();

            if (results.TestSuite == null || results.TestSuite.Results == null)
            {
                return testCases;
            }

            FillTestCasesResultRecursive(results.TestSuite.Results, testCases);

            return testCases;
        }

        private void FillTestCasesResultRecursive(TestBase[] results, List<TestCase> testCases)
        {
            if (results == null || results.Count() == 0)
            {
                return;
            }

            foreach (var res in results)
            {
                if (res.GetType() == typeof(TestCase))
                {
                    testCases.Add(res as TestCase);
                }

                if (res.GetType() == typeof(TestSuite))
                {
                    FillTestCasesResultRecursive((res as TestSuite).Results, testCases);
                }
            }
        }
    }
}
