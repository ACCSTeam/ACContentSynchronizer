using System.Collections.Generic;
using ACContentSynchronizer.Client.Models;
using ACContentSynchronizer.ClientGui.Tasks;
using ACContentSynchronizer.ClientGui.ViewModels;
using ReactiveUI;

namespace ACContentSynchronizer.ClientGui.Views {
  public class ServerViewModel : ViewModelBase {
    private ServerEntry _serverEntry = new();

    public ServerEntry ServerEntry {
      get => _serverEntry;
      set => this.RaiseAndSetIfChanged(ref _serverEntry, value);
    }

    public List<Entry> Cars { get; set; } = new();
    public Entry Track { get; set; } = new();

    public void ValidateContent() {
      StatusBar.Instance.AddTask(new ValidationTask(ServerEntry));
    }
  }
}
