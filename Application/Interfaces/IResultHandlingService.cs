using Monitor.Application.MonitoringChecks.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Monitor.Application.Interfaces
{
    public interface IResultHandlingService
    {
        Task HandleResult(CommandResult result);
    }
}
