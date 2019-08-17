using Monitor.Application.MonitoringChecks.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Monitor.Infrastructure.SignalR
{
    public interface ITypedHubClient
    {
        Task BroadcastChecks(IEnumerable<CheckWebModel> check);
    }
}
