using Monitor.Application.Interfaces;
using Monitor.Application.MonitoringChecks.Models;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Monitor.Application.MonitoringChecks.Commands
{
    public class HomePageCheckProdCommand : IRequest<CommandResult>, ICommand<CommandResult>
    {
        public CheckSettings CheckSettings => new CheckSettings
        {
            Priority = PrioritiesEnum.Critical,
            Service = "Autodoc site availability",
            Type = CheckTypeEnum.HomePageAvailableProd,
            EnvironmentId = (int)EnvironmentsEnum.Prod,
            CheckFullDescription = "Проверка доступности и времени загрузки html-кода главной страницы"
        };
    }

    public class HomePageCheckHandler : IRequestHandler<HomePageCheckProdCommand, CommandResult>
    {
        private IHttpRequestService _httpService;

        public HomePageCheckHandler(IHttpRequestService httpService)
        {
            _httpService = httpService ?? throw new ArgumentNullException(nameof(httpService));
        }

        public async Task<CommandResult> Handle(HomePageCheckProdCommand request, CancellationToken cancellationToken)
        {
            var result = new CommandResult();
            result.Success = true;
            var check = new HomePageCheck(_httpService);
            result.CheckModel = await check.CheckHomePageLoad(request.CheckSettings);
            return result;
        }
    }
}
