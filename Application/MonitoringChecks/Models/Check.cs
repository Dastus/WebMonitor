using System;

namespace Monitor.Application.MonitoringChecks.Models
{
    public class Check
    {
        public int Id { get; set; }
        public int EnvironmentId { get; set; }
        public string Host {get; set;}
        public DateTime LastCheckTime {get; set;}
        public DateTime StatusChangeTime {get; set;}
        public double ExecutionDuration { get; set; }
        public StatusesEnum Status {get; set;}
        public string Service {get; set;}
        public string StatusInfo {get; set;}
        public string Description { get; set; }
        public PrioritiesEnum Priority {get; set; }
        public CheckTypeEnum Type { get; set; }
        public string Schedule { get; set; } = "*/10 * * * *"; //UNIX cron format. every 10 minutes by default
    }
}
