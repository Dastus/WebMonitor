using System.Collections.Generic;
using MediatR;
using Monitor.Application.Interfaces;
using Monitor.Application.MonitoringChecks.Models;

namespace Monitor.Application.MonitoringChecks.Queries
{
    public class GetChecksQuery : IQuery<List<CheckWebModel>>, IRequest<List<CheckWebModel>>
    {
        public int? EnvironmentId { get; set; }
    }
}
