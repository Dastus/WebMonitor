using Monitor.Application.Interfaces;
using Monitor.Application.MonitoringChecks.Models;
using MediatR;

namespace Monitor.Application.MonitoringChecks.Commands
{
    public class HomePageCheckCommand : IRequest<CommandResult>, ICommand<CommandResult>
    {
    }
}
