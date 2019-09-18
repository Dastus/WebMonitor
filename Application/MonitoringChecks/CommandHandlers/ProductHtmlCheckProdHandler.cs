using MediatR;
using Monitor.Application.Interfaces;
using Monitor.Application.MonitoringChecks.ChecksLogic;
using Monitor.Application.MonitoringChecks.Commands;
using Monitor.Application.MonitoringChecks.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Monitor.Application.MonitoringChecks.CommandHandlers
{
    public class ProductHtmlCheckProdHandler : IRequestHandler<ProductHtmlCheckProdCommand, CommandResult>
    {
        private readonly IHttpRequestService _httpService;

        public ProductHtmlCheckProdHandler(IHttpRequestService httpService)
        {
            _httpService = httpService ?? throw new ArgumentNullException(nameof(httpService));
        }

        public async Task<CommandResult> Handle(ProductHtmlCheckProdCommand request, CancellationToken cancellationToken)
        {
            var result = new CommandResult
            {
                Success = true  ,
                CheckModel = await new ProductHtmlCheck(_httpService).CheckCategoryInfo(request.CheckSettings)
            };

            return result;
        }
    }
}
