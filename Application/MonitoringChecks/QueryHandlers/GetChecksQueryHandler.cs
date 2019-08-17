using MediatR;
using Monitor.Application.Interfaces;
using Monitor.Application.MonitoringChecks.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Monitor.Application.MonitoringChecks.Queries;
using System;

namespace Monitor.Application.MonitoringChecks.QueryHandlers
{
    public class GetChecksQueryHandler : IRequestHandler<GetChecksQuery, List<CheckWebModel>>
    {
        private IChecksRepository _repository;

        public GetChecksQueryHandler(IChecksRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<List<CheckWebModel>> Handle(GetChecksQuery request, CancellationToken cancellationToken)
        {
            var checks = await _repository.GetCurrentStateForEnvironment(request.EnvironmentId);
            return checks.ToList();
        }
    }
}
