using MediatR;
using Monitor.Application.Interfaces;
using Monitor.Application.MonitoringChecks.Models;

namespace Monitor.Application.MonitoringChecks.Commands
{
    public class ApiSearchCheckProdCommand : IRequest<CommandResult>, ICommand<CommandResult>
    {
    }
}
