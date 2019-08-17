using Monitor.Application.Interfaces;
using Monitor.Application.MonitoringChecks.Models;
using MediatR;

namespace Monitor.Application.MonitoringChecks.Commands
{
    public class HomePageCheckProdCommand : IRequest<CommandResult>, ICommand<CommandResult>
    {
        public CheckSettings CheckSettings => new CheckSettings
        {
            Priority = PrioritiesEnum.Critical,
            Service = "Autodoc site availability",
            Type = CheckTypeEnum.HomePageAvailableProd,
            EnvironmentId = (int)EnvironmentsEnum.Prod
        };
    }
}
