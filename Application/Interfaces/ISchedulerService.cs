using Monitor.Application.MonitoringChecks.Models;
using System.Threading.Tasks;

namespace Monitor.Application.Interfaces
{
    public interface ISchedulerService
    {
        Task AddToSchedule(CheckSettings settings);
        void RemoveFromSchedule(CheckSettings settings);
        Task ReSchedule(CheckSettings settings, StatusesEnum status);
        Task StopAll();
    }
}
