using System;
using System.Collections.Generic;
using System.Text;

namespace Monitor.Application.MonitoringChecks.Models
{
    public class CheckState
    {
        public DateTime LastCheckTime { get; set; }
        public DateTime StatusChangeTime { get; set; }
        public double ExecutionDuration { get; set; }
        public StatusesEnum Status { get; set; }
        public string Description { get; set; }
        public string DiagnosticsInfo { get; set; } = "";//
    }
}
