using Monitor.Application.MonitoringChecks.Models;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Linq;
using Monitor.Application.Interfaces;


namespace Monitor.Application.MonitoringChecks.CommandHandlers
{
    public class MetatagsCheckHandler: IRequestHandler<MetatagsCheckCommand, CommandResult>
    {
        private IHttpRequestService _httpService;

        public MetatagsCheckHandler(IHttpRequestService httpService)
        {
            _httpService = httpService ?? throw new ArgumentNullException(nameof(httpService));
        }

        public async Task<CommandResult> Handle(MetatagsCheckCommand request, CancellationToken cancellationToken)
        {
            var result = new CommandResult();
            result.Success = true;
            var check = new MetatagsCheck(_httpService);
            result.CheckModel = await check.CheckMetaInfo(EnvironmentsEnum.Prod);

            return result;
        }
    }
}
