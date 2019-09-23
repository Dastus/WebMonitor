using Monitor.Application.MonitoringChecks.Models;
using System;


namespace Monitor.Infrastructure.Logger
{
    public struct LogStashCheckRecord
    {
        public int EnvironmentId { get; set; }
        public CheckTypeEnum Type { get; set; }
        public DateTime LastCheckTime { get; set; }
        public double ExecutionDuration { get; set; }
        public string Service { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
    }
}
