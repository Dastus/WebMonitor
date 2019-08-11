using MediatR;
using Monitor.Application.MonitoringChecks.Models;
using System.Threading.Tasks;

namespace Monitor.Application.Interfaces
{
    public interface ICommandsProcessor
    {
        Task ExecuteCommand(CheckTypeEnum checkType);
        Task RegisterCheckProcessor(Check check, IRequest<CommandResult> command);
    }
}
