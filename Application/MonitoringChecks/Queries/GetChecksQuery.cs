using System.Collections.Generic;
using MediatR;
using Monitor.Application.Interfaces;
using Monitor.Application.MonitoringChecks.Models;

namespace Monitor.Application.MonitoringChecks.Queries
{
    public class GetChecksQuery : IQuery<List<Check>>, IRequest<List<Check>>
    {
        public int? EnvironmentId { get; set; }
    }
}
