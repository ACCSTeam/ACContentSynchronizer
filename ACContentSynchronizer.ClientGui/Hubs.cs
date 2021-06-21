using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ACContentSynchronizer.ClientGui.Models;
using Microsoft.AspNetCore.SignalR.Client;

namespace ACContentSynchronizer.ClientGui {
  public class Hubs {
    private static readonly Dictionary<string, HubConnection> StoredHubs = new();

    private static HubConnection BuildHub(ServerEntry entry) {
      var hub = new HubConnectionBuilder()
        .WithUrl($"{entry.Http}/notification")
        .WithAutomaticReconnect()
        .Build();

      StoredHubs.Add(entry.Ip, hub);

      return hub;
    }

    public static async Task<string> NotificationHub<T, TMethod>(ServerEntry entry, TMethod method,
                                                                 Func<T, Task> action)
      where TMethod : notnull {
      try {
        var hub = StoredHubs.ContainsKey(entry.Ip)
          ? StoredHubs[entry.Ip]
          : BuildHub(entry);

        hub.Remove(method.ToString());
        hub.On(method.ToString(), action);
        if (hub.State == HubConnectionState.Disconnected) {
          await hub.StartAsync();
        }
        return hub.ConnectionId;
      } catch {
        return string.Empty;
      }
    }

    public static async Task<string> NotificationHub<T1, T2, TMethod>(ServerEntry entry, TMethod method,
                                                                      Func<T1, T2, Task> action)
      where TMethod : notnull {
      try {
        var hub = StoredHubs.ContainsKey(entry.Ip)
          ? StoredHubs[entry.Ip]
          : BuildHub(entry);

        hub.On(method.ToString(), action);
        if (hub.State == HubConnectionState.Disconnected) {
          await hub.StartAsync();
        }
        return hub.ConnectionId;
      } catch {
        return string.Empty;
      }
    }
  }
}
