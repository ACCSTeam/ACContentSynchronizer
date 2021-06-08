using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ACContentSynchronizer.ClientGui.Models;
using Microsoft.AspNetCore.SignalR.Client;

namespace ACContentSynchronizer.ClientGui {
  public class Hubs {
    private static readonly Dictionary<string, HubConnection> StoredHubs = new();

    public static async Task NotificationHub<T, TMethod>(ServerEntry entry, TMethod method, Func<T, Task> action)
      where TMethod : notnull {
      try {
        if (StoredHubs.ContainsKey(entry.Ip)) {
          return;
        }

        var hub = new HubConnectionBuilder()
          .WithUrl($"{entry.Http}/notification")
          .WithAutomaticReconnect()
          .Build();

        StoredHubs.Add(entry.Ip, hub);

        hub.On(method.ToString(), action);

        await hub.StartAsync();
      } catch {
      }
    }
  }
}
