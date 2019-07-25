using Monitor.Application.MonitoringChecks.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Monitor.Application.Interfaces
{
    public interface IChecksRepository
    {
        Task Save(Check check);
        Check GetCheck(CheckTypeEnum checkType);
        Task<IEnumerable<Check>> GetCurrentState();
        Task<IEnumerable<Check>> GetCurrentStateForEnvironment(int? environmentId);
    }
}
