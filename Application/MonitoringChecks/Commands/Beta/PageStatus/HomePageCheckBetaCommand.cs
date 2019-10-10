using Monitor.Application.Interfaces;
using Monitor.Application.MonitoringChecks.Models;
using MediatR;
using System;
using System.Threading.Tasks;
using System.Threading;

namespace Monitor.Application.MonitoringChecks.Commands
{
    public class HomePageCheckBetaCommand : IRequest<CommandResult>, ICommand<CommandResult>
    {
        public CheckSettings CheckSettings => new CheckSettings
        {
            Priority = PrioritiesEnum.Critical,
            Service = "Autodoc beta site availability",
            Type = CheckTypeEnum.HomePageAvailableBeta,
            EnvironmentId = (int)EnvironmentsEnum.Beta,
            CheckFullDescription = "Проверка доступности и времени загрузки html-кода главной страницы"
        };
    }

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
            result.CheckModel = await check.CheckHomePageLoad(request.CheckSettings);
            return result;
        }
    }
}
