using Monitor.Application.Interfaces;
using Monitor.Application.MonitoringChecks.Models;
using MediatR;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace Monitor.Application.MonitoringChecks.Commands
{
    public class WebUISearchCheckProdCommand : IRequest<CommandResult>, ICommand<CommandResult>
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

    public class WebUISearchCheckHandler : IRequestHandler<WebUISearchCheckProdCommand, CommandResult>
    {
        private IWebDriversFactory _driversFactory;

        public WebUISearchCheckHandler(IWebDriversFactory driversFactory)
        {
            _driversFactory = driversFactory ?? throw new ArgumentNullException(nameof(driversFactory));
        }

        public async Task<CommandResult> Handle(WebUISearchCheckProdCommand request, CancellationToken cancellationToken)
        {
            var result = new CommandResult();
            result.Success = true;
            var check = new WebUISearchCheck(_driversFactory);
            result.CheckModel = await check.CheckWebUISearch(request.CheckSettings);

            return result;
        }
    }
}
