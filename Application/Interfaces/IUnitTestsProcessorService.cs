using Monitor.Application.MonitoringChecks.Models;
using System.Threading.Tasks;

namespace Monitor.Application.Interfaces
{
    public interface IUnitTestsProcessorService
    {
        Task<CheckState> ExecuteMsTestUnitTest(string testName, string pathToProject);
        Task<CheckState> ExecuteNUnitTest(string testName, string pathToDll);
    }
}
