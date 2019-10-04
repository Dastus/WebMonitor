using Monitor.Application.Interfaces;
using Monitor.Application.MonitoringChecks.Models;
using MediatR;
using System;
using System.Threading.Tasks;
using System.Threading;

namespace Monitor.Application.MonitoringChecks
{
    public class CategoryMetatagsCheckBetaCommand : IRequest<CommandResult>,  ICommand<CommandResult>
    {
        public CheckSettings CheckSettings => new CheckSettings
        {
            Priority = PrioritiesEnum.Medium,
            Service = "Meta tags check",
            Type = CheckTypeEnum.CategoryMetaTagsBeta,
            EnvironmentId = (int)EnvironmentsEnum.Beta,
            CheckFullDescription = "Проверка тегов rel, next, canonical, index, follow. Страница: /category/shiny-id49-3"
        };
    }

    public class CategoryMetatagsCheckBetaHandler : IRequestHandler<CategoryMetatagsCheckBetaCommand, CommandResult>
    {
        private IHttpRequestService _httpService;

        public CategoryMetatagsCheckBetaHandler(IHttpRequestService httpService)
        {
            _httpService = httpService ?? throw new ArgumentNullException(nameof(httpService));
        }

        public async Task<CommandResult> Handle(CategoryMetatagsCheckBetaCommand request, CancellationToken cancellationToken)
        {
            var result = new CommandResult();
            result.Success = true;
            var check = new CategoryMetatagsCheck(_httpService);
            result.CheckModel = await check.CheckMetaInfo(request.CheckSettings);

            return result;
        }
    }
}
