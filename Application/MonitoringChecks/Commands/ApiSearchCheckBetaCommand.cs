using MediatR;
using Monitor.Application.Interfaces;
using Monitor.Application.MonitoringChecks.Models;

namespace Monitor.Application.MonitoringChecks.Commands
{
    public class ApiSearchCheckBetaCommand : IRequest<CommandResult>, ICommand<CommandResult>
    {
    }
}
