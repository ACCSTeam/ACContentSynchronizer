using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ACContentSynchronizer.ClientGui.Components;
using ACContentSynchronizer.ClientGui.Modals;
using ACContentSynchronizer.ClientGui.Models;
using ACContentSynchronizer.ClientGui.Views;
using Avalonia.Collections;
using ReactiveUI;

namespace ACContentSynchronizer.ClientGui.ViewModels {
  public class ApplicationViewModel : ViewModelBase {
    private ServerEntryViewModel? _selectedServer;

    private Server? _server;
    private AvaloniaList<ServerEntryViewModel> _servers = new();

    private bool _serverSelected;

    public AvaloniaList<ServerEntryViewModel> Servers {
      get => _servers;
      set => this.RaiseAndSetIfChanged(ref _servers, value);
    }

    public ServerEntryViewModel? SelectedServer {
      get => _selectedServer;
      set {
        ServerSelected = value != null;
        Server?.Dispose();
        if (value != null) {
          ReactiveCommand.CreateFromTask(() => {
            Server = new(new (value));
            return Task.CompletedTask;
          }).Execute();
        }

        this.RaiseAndSetIfChanged(ref _selectedServer, value);
        GC.Collect();
      }
    }

    public Server? Server {
      get => _server;
      set => this.RaiseAndSetIfChanged(ref _server, value);
    }

    public AvaloniaList<StatusBarEntry> Tasks { get; set; } = new();

    public SettingsViewModel Settings { get; set; } = new();

    public bool ServerSelected {
      get => _serverSelected;
      set => this.RaiseAndSetIfChanged(ref _serverSelected, value);
    }

    public void UnsetServer() {
      SelectedServer = null;
    }

    public void AddTask(TaskViewModel task) {
      var entry = new StatusBarEntry();
      entry.Run(task, Tasks);
      Tasks.Insert(0, entry);
    }

    public async Task AddNewConnection() {
      await Modal.Open<AddNewServer, AddNewServerViewModel>();
      await SaveServersState();
    }

    public async Task EditServerDialog(ServerEntryViewModel server) {
      await Modal.Open<AddNewServer, AddNewServerViewModel>(new() {
        Server = new() {
          Ip = server.Ip,
          Port = server.Port,
          Password = server.Password,
          DateTime = server.DateTime,
        },
      });
      await SaveServersState();
    }

    public Task Remove(ServerEntryViewModel server) {
      Servers.Remove(server);
      return SaveServersState();
    }

    private Task SaveServersState() {
      return SaveAsync();
    }

    public async Task OpenSettings() {
      await Modal.Open<InitRun>();
    }

    public async Task SaveAsync() {
      var settings = new Settings {
        GamePath = Settings.GamePath,
        PlayerName = Settings.PlayerName,
        SteamId = Settings.SteamId,
        SidebarMinimized = Settings.SidebarMinimized,
        Servers = Servers
          .Select(x => new ServerEntry {
            Ip = x.Ip,
            Name = x.Name,
            Port = x.Port,
            KunosPort = x.KunosPort,
            Password = x.Password,
            DateTime = x.DateTime,
          })
          .ToList(),
      };

      var json = JsonSerializer.Serialize(settings);
      await FileUtils.CreateIfNotExistsAsync(Constants.SettingPath);
      await File.WriteAllTextAsync(Constants.SettingPath, json);
    }

    public void Load() {
      if (!File.Exists(Constants.SettingPath)) {
        return;
      }
      var json = File.ReadAllText(Constants.SettingPath);
      var settings = JsonSerializer.Deserialize<Settings>(json) ?? new();

      Settings.GamePath = settings.GamePath;
      Settings.PlayerName = settings.PlayerName;
      Settings.SteamId = settings.SteamId;
      Settings.SidebarMinimized = settings.SidebarMinimized;
      Servers = new(settings.Servers
        .Select(entry => new ServerEntryViewModel {
          Ip = entry.Ip,
          Name = entry.Name,
          Password = entry.Password,
          Port = entry.Port,
          KunosPort = entry.KunosPort,
          DateTime = entry.DateTime,
        }));
    }
  }
}
