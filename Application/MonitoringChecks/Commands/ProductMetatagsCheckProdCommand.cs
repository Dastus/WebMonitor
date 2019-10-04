using Monitor.Application.Interfaces;
using Monitor.Application.MonitoringChecks.Models;
using MediatR;
using System;
using System.Threading.Tasks;
using System.Threading;

namespace Monitor.Application.MonitoringChecks
{
    public class ProductMetatagsCheckProdCommand : IRequest<CommandResult>,  ICommand<CommandResult>
    {
        public CheckSettings CheckSettings => new CheckSettings
        {
            Priority = PrioritiesEnum.Medium,
            Service = "Meta tags check for product",
            Type = CheckTypeEnum.ProductMetaTagsProd,
            EnvironmentId = (int)EnvironmentsEnum.Prod,
            CheckFullDescription = "Проверка тегов rel, next, canonical, index, follow на странице товара oc90"
        };
    }

    public class ProductMetatagsCheckBetaHandler : IRequestHandler<ProductMetatagsCheckBetaCommand, CommandResult>
    {
        private IHttpRequestService _httpService;

        public ProductMetatagsCheckBetaHandler(IHttpRequestService httpService)
        {
            _httpService = httpService ?? throw new ArgumentNullException(nameof(httpService));
        }

        public async Task<CommandResult> Handle(ProductMetatagsCheckBetaCommand request, CancellationToken cancellationToken)
        {
            var result = new CommandResult();
            result.Success = true;
            var check = new ProductMetatagsCheck(_httpService);
            result.CheckModel = await check.CheckMetaInfo(request.CheckSettings);

            return result;
        }
    }
}
