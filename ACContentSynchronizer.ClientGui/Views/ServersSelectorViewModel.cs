using System;
using System.Linq;
using System.Threading.Tasks;
using ACContentSynchronizer.Client.Models;
using ACContentSynchronizer.ClientGui.Models;
using ACContentSynchronizer.ClientGui.ViewModels;
using ACContentSynchronizer.ClientGui.Windows;
using Avalonia.Collections;
using Avalonia.Interactivity;
using ReactiveUI;

namespace ACContentSynchronizer.ClientGui.Views {
  public class ServersSelectorViewModel : ViewModelBase {
    private string _server = "";

    public ServersSelectorViewModel() {
      var settings = Settings.Instance();
      Servers.AddRange(settings.Servers.Select(ip => new ServerEntry {
        Ip = ip,
      }));
    }

    public string Server {
      get => _server;
      set => this.RaiseAndSetIfChanged(ref _server, value);
    }

    private ServerEntry? _selectedServer;

    public ServerEntry? SelectedServer {
      get => _selectedServer;
      set => this.RaiseAndSetIfChanged(ref _selectedServer, value);
    }

    public AvaloniaList<ServerEntry> Servers { get; set; } = new();

    public async Task AddServer(string ip) {
      var settings = Settings.Instance();
      settings.Servers.Add(ip);
      await settings.Save();

      Servers.Add(new() {
        Ip = ip,
      });
    }

    public void DownloadContent(string http) {
      DownloadContentWindow.Open(MainWindow.Instance, http);
    }

    public void UploadContent(string http) {
      UploadContentWindow.Open(MainWindow.Instance, http);
    }

    public async Task RemoveServer(ServerEntry server) {
      var settings = Settings.Instance();
      settings.Servers.Remove(server.Ip);
      await settings.Save();

      Servers.Remove(server);
    }
  }
}
