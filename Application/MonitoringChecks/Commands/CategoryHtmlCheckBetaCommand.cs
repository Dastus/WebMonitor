using MediatR;
using Monitor.Application.Interfaces;
using Monitor.Application.MonitoringChecks.ChecksLogic;
using Monitor.Application.MonitoringChecks.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Monitor.Application.MonitoringChecks.Commands
{
    public class CategoryHtmlCheckBetaCommand : IRequest<CommandResult>, ICommand<CommandResult>
    {
        public CheckSettings CheckSettings => new CheckSettings
        {
            Priority = PrioritiesEnum.Medium,
            Service = "Category HTML structure check",
            Type = CheckTypeEnum.CategoryHtmlCheckBeta,
            EnvironmentId = (int)EnvironmentsEnum.Beta,
            CheckFullDescription = "Проверка URL крошек, меню подкатегорий, пейджинга. Страница: /category/kolenval-i-komplektuyushhie-id53-3"
        };
    }

    public class CategoryHtmlCheckBetaHandler : IRequestHandler<CategoryHtmlCheckBetaCommand, CommandResult>
    {
        private readonly IHttpRequestService _httpService;

        public CategoryHtmlCheckBetaHandler(IHttpRequestService httpService)
        {
            _httpService = httpService ?? throw new ArgumentNullException(nameof(httpService));
        }

        public async Task<CommandResult> Handle(CategoryHtmlCheckBetaCommand request, CancellationToken cancellationToken)
        {
            var result = new CommandResult
            {
                Success = true,
                CheckModel = await new CategoryHtmlCheck(_httpService).CheckCategoryInfo(request.CheckSettings)
            };

            return result;
        }
    }
}
