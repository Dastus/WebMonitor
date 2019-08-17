using MediatR;
using Monitor.Application.Interfaces;
using Monitor.Application.MonitoringChecks.Models;

namespace Monitor.Application.MonitoringChecks.Commands
{
    public class ApiSearchCheckProdCommand : IRequest<CommandResult>, ICommand<CommandResult>
    {
        public CheckSettings CheckSettings => new CheckSettings
        {
            Priority = PrioritiesEnum.Critical,
            Service = "Search API availability",
            Type = CheckTypeEnum.ApiSearchCheckProd,
            EnvironmentId = (int)EnvironmentsEnum.Prod
        };
    }
}
