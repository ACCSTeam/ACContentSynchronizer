using System;
using System.Threading.Tasks;
using ACContentSynchronizer.ClientGui.Models;
using ACContentSynchronizer.ClientGui.Services;
using ACContentSynchronizer.ClientGui.Views.ServerViews;
using ReactiveUI;

namespace ACContentSynchronizer.ClientGui.ViewModels {
  public class ServerViewModel : ViewModelBase, IDisposable {
    private readonly HubService _hubService;

    public ServerViewModel(ServerEntryViewModel server) {
      _hubService = new(server);
      ReactiveCommand.CreateFromTask(() => {
        Race = new(new(server, _hubService));
        ServerSettings = new(new(server, _hubService));
        return Task.CompletedTask;
      }).Execute();
    }

    private Race? Race { get; set; }
    private ServerSettings? ServerSettings { get; set; }

    public void Dispose() {
      _hubService.Dispose();
      Race?.Dispose();
      ServerSettings?.Dispose();
    }
  }
}
