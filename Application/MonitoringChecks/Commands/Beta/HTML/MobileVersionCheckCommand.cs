using MediatR;
using Monitor.Application.Interfaces;
using Monitor.Application.MonitoringChecks.ChecksLogic.HTML;
using Monitor.Application.MonitoringChecks.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Monitor.Application.MonitoringChecks.Commands.Beta.HTML
{
    public class MobileVersionCheckCommand : IRequest<CommandResult>, ICommand<CommandResult>
    {
        public CheckSettings CheckSettings => new CheckSettings
        {
            Priority = PrioritiesEnum.High,
            Service = "Mobile Markup Check",
            Type = CheckTypeEnum.HtmlMobileBeta,
            EnvironmentId = (int)EnvironmentsEnum.Beta,
            CheckFullDescription = "Проверяет возврат мобильной версии сайта"
        };
    }

    public class MobileVersionCheckCommandHandler : IRequestHandler<MobileVersionCheckCommand, CommandResult>
    {
        private readonly IHttpRequestService _httpService;

        public MobileVersionCheckCommandHandler(IHttpRequestService httpService)
        {
            _httpService = httpService ?? throw new ArgumentNullException(nameof(httpService));
        }

        public async Task<CommandResult> Handle(MobileVersionCheckCommand request, CancellationToken cancellationToken)
        {
            var result = new CommandResult
            {
                Success = true,
                CheckModel = await new LayoutVariantCheck(_httpService).PerformCheck(request.CheckSettings, false)
            };

            return result;
        }
    }
}
