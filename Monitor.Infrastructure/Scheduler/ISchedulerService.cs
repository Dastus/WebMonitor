using Monitor.Application.MonitoringChecks.Models;
using System.Threading.Tasks;

namespace Monitor.Infrastructure.Scheduler
{
    public interface ISchedulerService
    {
        Task AddToSchedule(CheckSettings check);
        void RemoveFromSchedule(CheckSettings check);
        Task StopAll();
    }
}
