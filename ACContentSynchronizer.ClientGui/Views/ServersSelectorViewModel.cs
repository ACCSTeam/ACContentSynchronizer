using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ACContentSynchronizer.Client.Models;
using ACContentSynchronizer.ClientGui.Models;
using ACContentSynchronizer.ClientGui.ViewModels;
using ACContentSynchronizer.ClientGui.Windows;
using Avalonia.Collections;
using ReactiveUI;

namespace ACContentSynchronizer.ClientGui.Views {
  public class ServersSelectorViewModel : ViewModelBase {
    private const string Pattern = @"(localhost|[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3})\:?([0-9]{1,5})?";

    private ServerEntry? _selectedServer;
    private string _server = "";

    public ServersSelectorViewModel() {
      var settings = Settings.Instance();
      var regex = new Regex($"^{Pattern}$");
      var servers = settings.Servers.Select(ip => new ServerEntry {
        Ip = ip,
      }).ToArray();

      if (servers.Any(x => !regex.IsMatch(x.Ip))) {
        var replaceRegex = new Regex(Pattern, RegexOptions.Compiled);
        servers = servers.Select(x => new ServerEntry {
          Ip = replaceRegex.Match(x.Ip).Value,
        }).ToArray();

        settings.Servers = servers.Select(x => x.Ip).ToList();
        settings.Save();
      }

      ConnectToHubs(servers);
      Servers.AddRange(servers);
    }

    public string Server {
      get => _server;
      set => this.RaiseAndSetIfChanged(ref _server, value);
    }

    public ServerEntry? SelectedServer {
      get => _selectedServer;
      set => this.RaiseAndSetIfChanged(ref _selectedServer, value);
    }

    public AvaloniaList<ServerEntry> Servers { get; set; } = new();

    private async Task ConnectToHubs(IEnumerable<ServerEntry> servers) {
      foreach (var server in servers) {
        await Hubs.NotificationHub<string, HubMethods>(server, HubMethods.Message,
          s => { return Task.CompletedTask; });
      }
    }

    public async Task AddServer(string ip) {
      var regex = new Regex($"^{Pattern}$", RegexOptions.Compiled);
      if (!regex.IsMatch(ip) || Servers.Any(x => x.Ip == ip)) {
        throw new();
      }

      var settings = Settings.Instance();
      settings.Servers.Add(ip);
      await settings.SaveAsync();

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
      await settings.SaveAsync();

      Servers.Remove(server);
    }
  }
}
