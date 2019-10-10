using MediatR;
using Monitor.Application.Interfaces;
using Monitor.Application.MonitoringChecks.ChecksLogic;
using Monitor.Application.MonitoringChecks.ChecksLogic.API.Delivery.Models;
using Monitor.Application.MonitoringChecks.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Monitor.Application.MonitoringChecks.Commands.Beta.API.Delivery
{
    public class NovaPoshtaPointsCheckCommand : IRequest<CommandResult>, ICommand<CommandResult>
    {
        public CheckSettings CheckSettings => new CheckSettings
        {
            Priority = PrioritiesEnum.Critical,
            Service = "Nova Poshta Points API availability",
            Type = CheckTypeEnum.ApiNovaPoshtaPointsBeta,
            EnvironmentId = (int)EnvironmentsEnum.Beta,
            CheckFullDescription = "Проверка результатов АПИ точек выдачи Новой Почты"
        };
    }

    public class ApiNovaPoshtaWarehousesBetaHandler : IRequestHandler<NovaPoshtaPointsCheckCommand, CommandResult>
    {
        private readonly IHttpRequestService _httpService;

        public ApiNovaPoshtaWarehousesBetaHandler(IHttpRequestService httpService)
        {
            _httpService = httpService ?? throw new ArgumentNullException(nameof(httpService));
        }

        public async Task<CommandResult> Handle(NovaPoshtaPointsCheckCommand request, CancellationToken cancellationToken)
        {
            var result = new CommandResult { Success = true };
            var check = new DeliveryPointsCheck(_httpService);

            var requestModel = new DeliveryPointsRequest
            {
                CityKey = "e71f8842-4b33-11e4-ab6d-005056801329",
                DeliveryMethodKey = 3,
                LanguageId = 2,
                PaymentMethodId = 21,
                RegionId = 57
            };

            result.CheckModel = await check.CheckApi(request.CheckSettings, requestModel);
            return result;
        }
    }
}
