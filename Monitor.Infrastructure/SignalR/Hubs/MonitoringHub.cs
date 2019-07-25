using Microsoft.AspNetCore.SignalR;

namespace Monitor.Infrastructure.SignalR
{
    public class MonitoringHub: Hub<ITypedHubClient>
    {
        //public async Task Send(Check check)
        //{
        //    await this.Clients.All.SendAsync("Send", check);
        //}
    }
}
