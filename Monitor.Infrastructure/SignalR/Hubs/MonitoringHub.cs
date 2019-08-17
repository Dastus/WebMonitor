using Microsoft.AspNetCore.SignalR;

namespace Monitor.Infrastructure.SignalR
{
    public class MonitoringHub: Hub<ITypedHubClient>
    {
    }
}
