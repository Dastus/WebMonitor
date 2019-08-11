using MediatR;
using Monitor.Application.MonitoringChecks.ChecksLogic;
using Monitor.Application.MonitoringChecks.Commands;
using Monitor.Application.MonitoringChecks.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Monitor.Application.MonitoringChecks.CommandHandlers
{
    public class ApiSearchCheckProdHandler : IRequestHandler<ApiSearchCheckProdCommand, CommandResult>
    {
        public async Task<CommandResult> Handle(ApiSearchCheckProdCommand request, CancellationToken cancellationToken)
        {
            var result = new CommandResult { Success = true };
            var check = new ApiSearchCheck();
            result.CheckModel = await check.CheckApiSearch(EnvironmentsEnum.Prod);
            return result;
        }
    }
}
