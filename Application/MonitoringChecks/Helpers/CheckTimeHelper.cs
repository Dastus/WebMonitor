using Monitor.Application.MonitoringChecks.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Monitor.Application.MonitoringChecks.Helpers
{
    public class CheckTimeHelper
    {
        public Check SetDurations(Check check, Check prevCheck)
        {
            var execTime = (DateTime.Now - check.LastCheckTime).TotalMilliseconds;
            check.ExecutionDuration = Math.Round(execTime, 3);             
            check.StatusChangeTime = (check.Status != prevCheck?.Status) ? DateTime.Now : prevCheck.StatusChangeTime;
            return check;
        }
    }
}
