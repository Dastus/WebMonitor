using Monitor.Application.MonitoringChecks.Commands;
using Monitor.Application.MonitoringChecks.Models;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Linq;
using Monitor.Application.Interfaces;

namespace Monitor.Application.MonitoringChecks.CommandHandlers
{
    public class HomePageCheckBetaHandler : IRequestHandler<HomePageCheckBetaCommand, CommandResult>
    {
        private IHttpRequestService _httpService;

        public HomePageCheckBetaHandler(IHttpRequestService httpService)
        {
            _httpService = httpService ?? throw new ArgumentNullException(nameof(httpService));
        }

        public async Task<CommandResult> Handle(HomePageCheckBetaCommand request, CancellationToken cancellationToken)
        {
            var result = new CommandResult();
            result.Success = true;
            var check = new HomePageCheck(_httpService);
            result.CheckModel = await check.CheckHomePageLoad(EnvironmentsEnum.Beta);
            return result;
        }
    }
}
