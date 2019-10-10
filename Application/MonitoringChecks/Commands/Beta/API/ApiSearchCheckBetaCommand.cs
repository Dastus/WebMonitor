using MediatR;
using Monitor.Application.Interfaces;
using Monitor.Application.MonitoringChecks.ChecksLogic;
using Monitor.Application.MonitoringChecks.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Monitor.Application.MonitoringChecks.Commands
{
    public class ApiSearchCheckBetaCommand : IRequest<CommandResult>, ICommand<CommandResult>
    {
        public CheckSettings CheckSettings => new CheckSettings
        {
            Priority = PrioritiesEnum.Critical,
            Service = "Search API availability",
            Type = CheckTypeEnum.ApiSearchCheckBeta,
            EnvironmentId = (int)EnvironmentsEnum.Beta,
            CheckFullDescription = "Проверка результатов АПИ поискового запроса. Поиск ос90"
        };
    }

    public class ApiSearchCheckBetaHandler : IRequestHandler<ApiSearchCheckBetaCommand, CommandResult>
    {
        public async Task<CommandResult> Handle(ApiSearchCheckBetaCommand request, CancellationToken cancellationToken)
        {
            var result = new CommandResult { Success = true };
            var check = new ApiSearchCheck();
            result.CheckModel = await check.CheckApiSearch(request.CheckSettings);
            return result;
        }
    }
}
