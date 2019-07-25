using System;
using System.Collections.Generic;
using System.Text;

namespace Monitor.Application.MonitoringChecks.Models
{
    public class CheckSettings
    {
        public CheckTypeEnum CheckType { get; set; }
        public string Schedule { get; set; }
    }
}
