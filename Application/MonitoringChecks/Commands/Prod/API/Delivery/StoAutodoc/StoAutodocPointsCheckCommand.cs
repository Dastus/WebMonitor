using MediatR;
using Monitor.Application.Interfaces;
using Monitor.Application.MonitoringChecks.ChecksLogic;
using Monitor.Application.MonitoringChecks.ChecksLogic.API.Delivery.Models;
using Monitor.Application.MonitoringChecks.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Monitor.Application.MonitoringChecks.Commands.Prod.API.Delivery
{
    public class StoAutodocPointsCheckCommand : IRequest<CommandResult>, ICommand<CommandResult>
    {
        public CheckSettings CheckSettings => new CheckSettings
        {
            Priority = PrioritiesEnum.Critical,
            Service = "Sto Autodoc Points API availability",
            Type = CheckTypeEnum.ApiStoAutodocPointsProd,
            EnvironmentId = (int)EnvironmentsEnum.Prod,
            CheckFullDescription = "Проверка результатов АПИ точек выдачи СТО Автодок"
        };
    }

    public class StoAutodocPointsCheckCommandHandler : IRequestHandler<StoAutodocPointsCheckCommand, CommandResult>
    {
        private readonly IHttpRequestService _httpService;

        public StoAutodocPointsCheckCommandHandler(IHttpRequestService httpService)
        {
            _httpService = httpService ?? throw new ArgumentNullException(nameof(httpService));
        }

        public async Task<CommandResult> Handle(StoAutodocPointsCheckCommand request, CancellationToken cancellationToken)
        {
            var result = new CommandResult { Success = true };
            var check = new DeliveryPointsCheck(_httpService);

            var requestModel = new DeliveryPointsRequest
            {
                CityKey = "10763",
                DeliveryMethodKey = 5,
                DistrictId = 598,
                LanguageId = 2,
                PaymentMethodId = 21,
                RegionId = 57
            };

            result.CheckModel = await check.CheckApi(request.CheckSettings, requestModel);
            return result;
        }
    }
}
