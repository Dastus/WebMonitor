using Monitor.Application.MonitoringChecks.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Monitor.Application.Interfaces
{
    public interface IScheduleRepository
    {
        Task<Dictionary<CheckTypeEnum, CheckSettings>> GetSchedule();
    }
}
