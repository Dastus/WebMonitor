using Monitor.Application.MonitoringChecks.Models;
using System.Threading.Tasks;

namespace Monitor.Infrastructure.Scheduler
{
    public interface ISchedulerService
    {
        Task AddToSchedule(Check check);
        void RemoveFromSchedule(Check check);
        Task StopAll();
    }
}
