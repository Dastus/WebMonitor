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
    public class DeliveryAutoPointsCheckCommand : IRequest<CommandResult>, ICommand<CommandResult>
    {
        public CheckSettings CheckSettings => new CheckSettings
        {
            Priority = PrioritiesEnum.Critical,
            Service = "Delivery Auto Points API availability",
            Type = CheckTypeEnum.ApiDeliveryAutoPointsBeta,
            EnvironmentId = (int)EnvironmentsEnum.Beta,
            CheckFullDescription = "Проверка результатов АПИ точек выдачи Деливери Авто"
        };
    }

    public class DeliveryAutoPointsCheckCommandHandler : IRequestHandler<DeliveryAutoPointsCheckCommand, CommandResult>
    {
        private readonly IHttpRequestService _httpService;

        public DeliveryAutoPointsCheckCommandHandler(IHttpRequestService httpService)
        {
            _httpService = httpService ?? throw new ArgumentNullException(nameof(httpService));
        }

        public async Task<CommandResult> Handle(DeliveryAutoPointsCheckCommand request, CancellationToken cancellationToken)
        {
            var result = new CommandResult { Success = true };
            var check = new DeliveryPointsCheck(_httpService);

            var requestModel = new DeliveryPointsRequest
            {
                CityKey = "4577d856-322b-e311-8b0d-00155d037960",
                DeliveryMethodKey = 4,
                LanguageId = 2,
                PaymentMethodId = 21,
                RegionId = 57
            };

            result.CheckModel = await check.CheckApi(request.CheckSettings, requestModel);
            return result;
        }
    }
}
