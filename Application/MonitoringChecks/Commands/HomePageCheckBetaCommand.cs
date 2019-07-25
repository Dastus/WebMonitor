using Monitor.Application.Interfaces;
using Monitor.Application.MonitoringChecks.Models;
using MediatR;

namespace Monitor.Application.MonitoringChecks.Commands
{
    public class HomePageCheckBetaCommand : IRequest<CommandResult>, ICommand<CommandResult>
    {
    }
}
