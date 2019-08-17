using MediatR;
using Monitor.Application.MonitoringChecks.ChecksLogic;
using Monitor.Application.MonitoringChecks.Commands;
using Monitor.Application.MonitoringChecks.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Monitor.Application.MonitoringChecks.CommandHandlers
{
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
