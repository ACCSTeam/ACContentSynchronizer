using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using ACContentSynchronizer.Extensions;
using ACContentSynchronizer.Models;

namespace ACContentSynchronizer {
  public class KunosClient : IDisposable {
    public readonly HttpClient Client;

    public KunosClient(string ip, string port) {
      Client = new() {
        BaseAddress = new($"http://{ip}:{port}/"),
      };
    }

    public void Dispose() {
      Client.Dispose();
    }
  }

  public static class ServerApiProvider {
    public static Task<ServerState?> GetCars(this KunosClient client, string steamId) {
      return client.Client.GetJson<ServerState>($"JSON|{steamId}");
    }

    public static Task<ServerInfo?> GetServerInfo(this KunosClient client) {
      return client.Client.GetJson<ServerInfo>("INFO");
    }

    public static Task<string> Booking(this KunosClient client, params string[] args) {
      var url = $"SUB|{HttpUtility.UrlPathEncode(string.Join('|', args))}";
      return client.Client.GetStringAsync(url);
    }
  }
}
