using Monitor.Application.MonitoringChecks.Models;
using System.Threading.Tasks;

namespace Monitor.Application.Interfaces
{
    public interface IUnitTestsProcessorService
    {
        Task<CheckState> ExecuteUnitTest(string testName, string pathToProject);
    }
}
