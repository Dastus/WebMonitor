using Monitor.Application.MonitoringChecks.Models;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Linq;
using Monitor.Application.Interfaces;


namespace Monitor.Application.MonitoringChecks.CommandHandlers
{
    public class CategoryMetatagsCheckProdHandler: IRequestHandler<CategoryMetatagsCheckProdCommand, CommandResult>
    {
        private IHttpRequestService _httpService;

        public CategoryMetatagsCheckProdHandler(IHttpRequestService httpService)
        {
            _httpService = httpService ?? throw new ArgumentNullException(nameof(httpService));
        }

        public async Task<CommandResult> Handle(CategoryMetatagsCheckProdCommand request, CancellationToken cancellationToken)
        {
            var result = new CommandResult();
            result.Success = true;
            var check = new CategoryMetatagsCheck(_httpService);
            result.CheckModel = await check.CheckMetaInfo(request.CheckSettings);

            return result;
        }
    }
}
