using Monitor.Application.MonitoringChecks.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Monitor.Application.Interfaces
{
    public interface INotificationsService
    {
        Task Notify(List<Check> checkResults);
    }
}
