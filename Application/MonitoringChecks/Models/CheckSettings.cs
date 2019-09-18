using System;
using System.Collections.Generic;
using System.Text;

namespace Monitor.Application.MonitoringChecks.Models
{
    public class CheckSettings
    {        
        public int EnvironmentId { get; set; }
        public CheckTypeEnum Type { get; set; }
        public PrioritiesEnum Priority { get; set; }
        public string Service { get; set; }
        public string CheckFullDescription { get; set; }

        //schedules are in UNIX cron format
        public string NormalSchedule { get; set; } = "*/10 * * * *"; //every 10 minutes;
        public string WarningSchedule { get; set; } = "*/5 * * * *";
        public string CriticalSchedule { get; set; } = "*/2 * * * *";
    }
}
