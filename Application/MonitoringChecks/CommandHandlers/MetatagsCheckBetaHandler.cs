using Monitor.Application.MonitoringChecks.Models;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Linq;
using Monitor.Application.Interfaces;


namespace Monitor.Application.MonitoringChecks.CommandHandlers
{
    public class MetatagsCheckBetaHandler: IRequestHandler<MetatagsCheckBetaCommand, CommandResult>
    {
        private IHttpRequestService _httpService;

        public MetatagsCheckBetaHandler(IHttpRequestService httpService)
        {
            _httpService = httpService ?? throw new ArgumentNullException(nameof(httpService));
        }

        public async Task<CommandResult> Handle(MetatagsCheckBetaCommand request, CancellationToken cancellationToken)
        {
            var result = new CommandResult();
            result.Success = true;
            var check = new MetatagsCheck(_httpService);
            result.CheckModel = await check.CheckMetaInfo(EnvironmentsEnum.Beta);

            return result;
        }
    }
}
