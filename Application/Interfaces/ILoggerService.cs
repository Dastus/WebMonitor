using Monitor.Application.MonitoringChecks.Models;
using System.Threading.Tasks;

namespace Monitor.Application.Interfaces
{
    public interface ILoggerService
    {
        Task SaveLog(CommandResult result);
    }
}
