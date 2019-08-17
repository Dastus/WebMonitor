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
            var execTime = (DateTime.Now - check.State.LastCheckTime).TotalMilliseconds;
            check.State.ExecutionDuration = Math.Round(execTime, 3);             
            check.State.StatusChangeTime = (check.State.Status != prevCheck?.State.Status) ? DateTime.Now : prevCheck.State.StatusChangeTime;
            return check;
        }
    }
}
