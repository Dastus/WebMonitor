using MediatR;
using Monitor.Application.Interfaces;
using Monitor.Application.MonitoringChecks.ChecksLogic;
using Monitor.Application.MonitoringChecks.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Monitor.Application.MonitoringChecks.Commands
{
    public class ProductHtmlCheckBetaCommand : IRequest<CommandResult>, ICommand<CommandResult>
    {
        public CheckSettings CheckSettings => new CheckSettings
        {
            Priority = PrioritiesEnum.Critical,
            Service = "Product HTML structure check",
            Type = CheckTypeEnum.ProductHtmlCheckBeta,
            EnvironmentId = (int)EnvironmentsEnum.Beta,
            CheckFullDescription = "Проверка HTML структуры. Страница: product/filtr-maslyanyj-dvigatelya-lanos-aveo-lacetti-nubira-nexia-pr-vo-knecht-mahle-id-84669-0-170"
        };
    }

    public class ProductHtmlCheckBetaHandler : IRequestHandler<ProductHtmlCheckBetaCommand, CommandResult>
    {
        private readonly IHttpRequestService _httpService;

        public ProductHtmlCheckBetaHandler(IHttpRequestService httpService)
        {
            _httpService = httpService ?? throw new ArgumentNullException(nameof(httpService));
        }

        public async Task<CommandResult> Handle(ProductHtmlCheckBetaCommand request, CancellationToken cancellationToken)
        {
            var result = new CommandResult
            {
                Success = true,
                CheckModel = await new ProductHtmlCheck(_httpService).CheckCategoryInfo(request.CheckSettings)
            };

            return result;
        }
    }
}
