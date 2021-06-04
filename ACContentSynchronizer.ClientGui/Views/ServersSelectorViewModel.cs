using System.Threading.Tasks;
using ACContentSynchronizer.Client.Models;
using ACContentSynchronizer.ClientGui.ViewModels;
using ACContentSynchronizer.ClientGui.Windows;
using Avalonia.Collections;
using ReactiveUI;

namespace ACContentSynchronizer.ClientGui.Views {
  public class ServersSelectorViewModel : ViewModelBase {
    private string _server = "";

    public ServersSelectorViewModel() {
      var settings = Settings.Instance();
      Servers.AddRange(settings.Servers);
    }

    public string Server {
      get => _server;
      set => this.RaiseAndSetIfChanged(ref _server, value);
    }

    public AvaloniaList<string> Servers { get; set; } = new();

    public async Task AddServer(string server) {
      var serverUrl = $"http://{server}";
      var settings = Settings.Instance();

      settings.Servers.Add(serverUrl);
      await settings.Save();
      Servers.Add(serverUrl);
    }

    public void GetDataFromServer(string server) {
      DownloadContentWindow.Open(MainWindow.Instance, server);
    }
  }
}
