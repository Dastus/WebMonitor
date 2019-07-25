using Monitor.Application.Interfaces;
using Monitor.Application.MonitoringChecks.Models;
using MediatR;

namespace Monitor.Application.MonitoringChecks
{
    public class MetatagsCheckCommand : IRequest<CommandResult>,  ICommand<CommandResult>
    {
    }
}
