using Monitor.Application.Interfaces;
using Monitor.Application.MonitoringChecks.Models;
using MediatR;

namespace Monitor.Application.MonitoringChecks
{
    public class MetatagsCheckBetaCommand : IRequest<CommandResult>,  ICommand<CommandResult>
    {
    }
}
