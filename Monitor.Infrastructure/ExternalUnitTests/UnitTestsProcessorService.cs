using Monitor.Application.Interfaces;
using Monitor.Application.MonitoringChecks.Models;
using Monitor.Infrastructure.ExternalUnitTests.MsTest;
using Monitor.Infrastructure.ExternalUnitTests.nUnit;
using System.Threading.Tasks;

namespace Monitor.Infrastructure.ExternalUnitTests
{
    public class UnitTestsProcessorService : IUnitTestsProcessorService
    {
        public async Task<CheckState> ExecuteMsTest(string testName, string pathToProject)
        {
            var processor = new MsTestsProcessor();
            return await processor.ExecuteUnitTest(testName, pathToProject);
        }

        public async Task<CheckState> ExecuteNUnitTest(string testName, string pathToDll)
        {
            var processor = new NUnitTestsProcessor();
            return await processor.ExecuteUnitTest(testName, pathToDll);
        }

        public async Task<CheckState> ExecuteVsTest(string testName, string pathToDll)
        {
            var processor = new VsTestsProcessor();
            return await processor.ExecuteUnitTest(testName, pathToDll);
        }
    }
}
