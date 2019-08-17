using System;

namespace Monitor.Application.MonitoringChecks.Models
{
    public class Check
    {
        public CheckState State { get; set; } = new CheckState();
        public CheckSettings Settings { get; set; } = new CheckSettings();
    }
}
