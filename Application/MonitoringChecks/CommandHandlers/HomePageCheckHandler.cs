using Monitor.Application.MonitoringChecks.Commands;
using Monitor.Application.MonitoringChecks.Models;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Net;
using Monitor.Application.Interfaces;
using Monitor.Application.MonitoringChecks.Helpers;

namespace Monitor.Application.MonitoringChecks.CommandHandlers
{
    public class HomePageCheckHandler : IRequestHandler<HomePageCheckCommand, CommandResult>
    {
        private IHttpRequestService _httpService;

        public HomePageCheckHandler(IHttpRequestService httpService)
        {
            _httpService = httpService ?? throw new ArgumentNullException(nameof(httpService));
        }

        public async Task<CommandResult> Handle(HomePageCheckCommand request, CancellationToken cancellationToken)
        {            
            var result = new CommandResult();
            result.Success = true;
            var check = new HomePageCheck(_httpService);
            result.CheckModel = await check.CheckHomePageLoad(EnvironmentsEnum.Prod);
            return result;
        }
    }
}
