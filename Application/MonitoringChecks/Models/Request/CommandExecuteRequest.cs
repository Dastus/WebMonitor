using System;
using System.Collections.Generic;
using System.Text;

namespace Monitor.Application.MonitoringChecks.Models.Request
{
    public class CommandExecuteRequest
    {
        public CheckTypeEnum Type { get; set; }
        public int EnvironmentId { get; set; }
    }
}
