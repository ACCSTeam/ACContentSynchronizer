using System;
using System.Threading.Tasks;
using ACContentSynchronizer.ClientGui.Models;
using Microsoft.AspNetCore.SignalR.Client;
using ReactiveUI;

namespace ACContentSynchronizer.ClientGui.Services {
  public class HubService : IDisposable {
    private readonly ServerEntryViewModel _serverEntry;
    private HubConnection? _hub;

    public HubService(ServerEntryViewModel serverEntry) {
      _serverEntry = serverEntry;
      _hub ??= BuildHub(_serverEntry);

      ReactiveCommand.CreateFromTask(async () => {
        if (_hub.State == HubConnectionState.Disconnected) {
          await _hub.StartAsync();
        }

        _serverEntry.ClientId = _hub.ConnectionId;
      });
    }

    public void Dispose() {
        _hub?.StopAsync();
        _hub?.DisposeAsync();
      }

      private static HubConnection BuildHub(ServerEntryViewModel entry) {
        var hub = new HubConnectionBuilder()
          .WithUrl($"{entry.Http}{Constants.HubEndpoint}")
          .WithAutomaticReconnect()
          .Build();

        return hub;
      }

      public async Task NotificationHub<T, TMethod>(TMethod method,
                                                    Func<T, Task> action)
        where TMethod : notnull {
        try {
          _hub ??= BuildHub(_serverEntry);

          _hub.On(method.ToString(), action);

          if (_hub.State == HubConnectionState.Disconnected) {
            await _hub.StartAsync();
          }

          _serverEntry.ClientId = _hub.ConnectionId;
        } catch {
          _serverEntry.ClientId = "";
        }
      }

      public async Task NotificationHub<T1, T2, TMethod>(TMethod method,
                                                         Func<T1, T2, Task> action)
        where TMethod : notnull {
        try {
          _hub ??= BuildHub(_serverEntry);

          _hub.On(method.ToString(), action);

          if (_hub.State == HubConnectionState.Disconnected) {
            await _hub.StartAsync();
          }

          _serverEntry.ClientId = _hub.ConnectionId;
        } catch {
          _serverEntry.ClientId = "";
        }
      }
    }
  }
