using MediatR;
using Monitor.Application.Interfaces;
using Monitor.Application.MonitoringChecks.ChecksLogic;
using Monitor.Application.MonitoringChecks.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Monitor.Application.MonitoringChecks.Commands
{
    public class CategoryHtmlCheckProdCommand : IRequest<CommandResult>, ICommand<CommandResult>
    {
        public CheckSettings CheckSettings => new CheckSettings
        {
            Priority = PrioritiesEnum.Medium,
            Service = "Category HTML structure check",
            Type = CheckTypeEnum.CategoryHtmlCheckProd,
            EnvironmentId = (int)EnvironmentsEnum.Prod,
            CheckFullDescription = "Проверка URL крошек, меню подкатегорий, пейджинга. Страница: /category/kolenval-i-komplektuyushhie-id53-3"
        };
    }

    public class CategoryHtmlCheckProdHandler : IRequestHandler<CategoryHtmlCheckProdCommand, CommandResult>
    {
        private readonly IHttpRequestService _httpService;

        public CategoryHtmlCheckProdHandler(IHttpRequestService httpService)
        {
            _httpService = httpService ?? throw new ArgumentNullException(nameof(httpService));
        }

        public async Task<CommandResult> Handle(CategoryHtmlCheckProdCommand request, CancellationToken cancellationToken)
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
