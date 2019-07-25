using Monitor.Application.MonitoringChecks.Models;
using System.Threading.Tasks;

namespace Monitor.Application.MonitoringChecks
{
    public interface ICommandsProcessor
    {
        Task ExecuteCommand(CheckTypeEnum checkType);
    }
}
