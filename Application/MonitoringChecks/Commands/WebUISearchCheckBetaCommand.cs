using Monitor.Application.Interfaces;
using Monitor.Application.MonitoringChecks.Models;
using MediatR;

namespace Monitor.Application.MonitoringChecks.Commands
{
    public class WebUISearchCheckBetaCommand : IRequest<CommandResult>, ICommand<CommandResult>
    {
    }
}
