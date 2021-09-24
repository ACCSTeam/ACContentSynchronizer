using System.Threading.Tasks;
using ACContentSynchronizer.Server.Hubs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;

namespace ACContentSynchronizer.Server.Services {
  public class SignalRService {
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IHubContext<NotificationHub> _hub;

    public SignalRService(IHubContext<NotificationHub> hub,
                          IHttpContextAccessor httpContextAccessor) {
      _hub = hub;
      _httpContextAccessor = httpContextAccessor;
    }

    private string? GetClientId() {
      return _httpContextAccessor.HttpContext?.Request.Headers[DefaultHeaders.ClientId];
    }

    public Task Send<T>(T method, object arg1) {
      var clientId = GetClientId();
      return string.IsNullOrEmpty(clientId)
        ? Task.CompletedTask
        : _hub.Clients.Client(clientId).SendAsync($"{method}", arg1);
    }

    public Task Send<T>(T method, object arg1, object arg2) {
      var clientId = GetClientId();
      return string.IsNullOrEmpty(clientId)
        ? Task.CompletedTask
        : _hub.Clients.Client(clientId).SendAsync($"{method}", arg1, arg2);
    }

    public Task SendAll<T>(T method, object arg1) {
      return _hub.Clients.All.SendAsync($"{method}", arg1);
    }

    public Task SendAll<T>(T method, object arg1, object arg2) {
      return _hub.Clients.All.SendAsync($"{method}", arg1, arg2);
    }
  }
}
