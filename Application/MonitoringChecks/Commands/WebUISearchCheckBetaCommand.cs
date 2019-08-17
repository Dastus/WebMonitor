using Monitor.Application.Interfaces;
using Monitor.Application.MonitoringChecks.Models;
using MediatR;

namespace Monitor.Application.MonitoringChecks.Commands
{
    public class WebUISearchCheckBetaCommand : IRequest<CommandResult>, ICommand<CommandResult>
    {
        public CheckSettings CheckSettings => new CheckSettings
        {
            Priority = PrioritiesEnum.Critical,
            Service = "Web UI search",
            Type = CheckTypeEnum.WebUISearchBeta,
            EnvironmentId = (int)EnvironmentsEnum.Beta
        };
    }
}
