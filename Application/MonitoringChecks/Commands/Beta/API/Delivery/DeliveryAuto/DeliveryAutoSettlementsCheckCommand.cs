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
    public class DeliveryAutoSettlementsCheckCommand : IRequest<CommandResult>, ICommand<CommandResult>
    {
        public CheckSettings CheckSettings => new CheckSettings
        {
            Priority = PrioritiesEnum.Critical,
            Service = "Delivery Auto Settlements API availability",
            Type = CheckTypeEnum.ApiDeliveryAutoSettlementsBeta,
            EnvironmentId = (int)EnvironmentsEnum.Beta,
            CheckFullDescription = "Проверка результатов АПИ населенных пунктов Новой Почты"
        };
    }

    public class DeliveryAutoSettlementsCheckCommandHandler : IRequestHandler<DeliveryAutoSettlementsCheckCommand, CommandResult>
    {
        private readonly IHttpRequestService _httpService;

        public DeliveryAutoSettlementsCheckCommandHandler(IHttpRequestService httpService)
        {
            _httpService = httpService ?? throw new ArgumentNullException(nameof(httpService));
        }

        public async Task<CommandResult> Handle(DeliveryAutoSettlementsCheckCommand request, CancellationToken cancellationToken)
        {
            var result = new CommandResult { Success = true };
            var check = new SettlementsCheck(_httpService);

            var requestModel = new CityRequest
            {
                AreaId = 57,
                DeliveryMethodKey = 3,
                DistrictId = 598,
                LanguageId = 2,
                SearchString = ""
            };

            result.CheckModel = await check.CheckApi(request.CheckSettings, requestModel);
            return result;
        }
    }
}
