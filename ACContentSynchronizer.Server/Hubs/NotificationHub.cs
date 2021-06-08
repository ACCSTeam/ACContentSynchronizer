using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace ACContentSynchronizer.Server.Hubs {
  public class NotificationHub : Hub {
    public async Task Message(string message) {
      await Clients.All.SendAsync("Message",message);
    }
  }
}
