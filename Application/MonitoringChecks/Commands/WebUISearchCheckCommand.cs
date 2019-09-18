using Monitor.Application.Interfaces;
using Monitor.Application.MonitoringChecks.Models;
using MediatR;

namespace Monitor.Application.MonitoringChecks.Commands
{
    public class WebUISearchCheckCommand : IRequest<CommandResult>, ICommand<CommandResult>
    {
        public CheckSettings CheckSettings => new CheckSettings
        {
            Priority = PrioritiesEnum.Critical,
            Service = "Web UI search",
            Type = CheckTypeEnum.WebUISearchProd,
            EnvironmentId = (int)EnvironmentsEnum.Prod,
            CheckFullDescription = "Selenium-тест. Поиск 'ос90'"
        };

    }
}
