using MediatR;
using Monitor.Application.Interfaces;
using Monitor.Application.MonitoringChecks.ChecksLogic;
using Monitor.Application.MonitoringChecks.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Monitor.Application.MonitoringChecks.Commands.Prod.API.Delivery
{
    public class AreasCheckCommand : IRequest<CommandResult>, ICommand<CommandResult>
    {
        public CheckSettings CheckSettings => new CheckSettings
        {
            Priority = PrioritiesEnum.Critical,
            Service = "Areas API availability",
            Type = CheckTypeEnum.ApiAreasCheckProd,
            EnvironmentId = (int)EnvironmentsEnum.Prod,
            CheckFullDescription = "Проверка результатов АПИ областей"
        };
    }

    public class ApiNovaPoshtaAreasCheckBetaHandler : IRequestHandler<AreasCheckCommand, CommandResult>
    {
        private readonly IHttpRequestService _httpService;

        public ApiNovaPoshtaAreasCheckBetaHandler(IHttpRequestService httpService)
        {
            _httpService = httpService ?? throw new ArgumentNullException(nameof(httpService));
        }

        public async Task<CommandResult> Handle(AreasCheckCommand request, CancellationToken cancellationToken)
        {
            var result = new CommandResult { Success = true };
            var check = new AreasCheck(_httpService);
            result.CheckModel = await check.CheckApi(request.CheckSettings);
            return result;
        }
    }
}
