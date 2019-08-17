using Monitor.Application.MonitoringChecks.Commands;
using Monitor.Application.MonitoringChecks.Models;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System;
using Monitor.Application.Interfaces;

namespace Monitor.Application.MonitoringChecks.CommandHandlers
{
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
