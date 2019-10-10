using Monitor.Application.Interfaces;
using Monitor.Application.MonitoringChecks.Models;
using MediatR;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace Monitor.Application.MonitoringChecks.Commands
{
    public class WebUISearchCheckBetaCommand : IRequest<CommandResult>, ICommand<CommandResult>
    {
        public CheckSettings CheckSettings => new CheckSettings
        {
            Priority = PrioritiesEnum.Critical,
            Service = "Web UI search",
            Type = CheckTypeEnum.WebUISearchBeta,
            EnvironmentId = (int)EnvironmentsEnum.Beta,
            CheckFullDescription = "Selenium-тест. Поиск 'ос90'"
        };
    }

    public class WebUISearchCheckBetaHandler : IRequestHandler<WebUISearchCheckBetaCommand, CommandResult>
    {
        private IWebDriversFactory _driversFactory;

        public WebUISearchCheckBetaHandler(IWebDriversFactory driversFactory)
        {
            _driversFactory = driversFactory ?? throw new ArgumentNullException(nameof(driversFactory));
        }

        public async Task<CommandResult> Handle(WebUISearchCheckBetaCommand request, CancellationToken cancellationToken)
        {
            var result = new CommandResult();
            result.Success = true;
            var check = new WebUISearchCheck(_driversFactory);
            result.CheckModel = await check.CheckWebUISearch(request.CheckSettings);

            return result;
        }
    }
}
