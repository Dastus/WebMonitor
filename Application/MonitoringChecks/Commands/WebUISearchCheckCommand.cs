using Monitor.Application.Interfaces;
using Monitor.Application.MonitoringChecks.Models;
using MediatR;

namespace Monitor.Application.MonitoringChecks.Commands
{
    public class WebUISearchCheckCommand : IRequest<CommandResult>, ICommand<CommandResult>
    {
    }
}
