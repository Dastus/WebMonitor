using System;
using System.Net;

namespace Monitor.Application.MonitoringChecks.Models
{
    public class WebPageLoadResult
    {
        public TimeSpan LoadTime { get; set; }
        public HttpStatusCode ResponseStatus { get; set; }
    }
}
