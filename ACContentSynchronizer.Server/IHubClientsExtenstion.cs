using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace ACContentSynchronizer.Server {
  public static class HubClientsExtenstion {
    public static Task SendAsync<T>(this IClientProxy clients, T method, params object[] args)
      where T : struct {
      return clients.SendCoreAsync($"{method}", args);
    }
  }
}
