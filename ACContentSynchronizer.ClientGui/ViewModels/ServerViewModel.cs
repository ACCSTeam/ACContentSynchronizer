using System.Threading.Tasks;
using ACContentSynchronizer.ClientGui.Models;
using ACContentSynchronizer.ClientGui.Views.ServerViews;
using ReactiveUI;

namespace ACContentSynchronizer.ClientGui.ViewModels {
  public class ServerViewModel : ViewModelBase {
    public ServerViewModel(ServerEntryViewModel server) {
      ReactiveCommand.CreateFromTask(() => {
        Race = new(new(server));
        ServerSettings = new(new(server));
        return Task.CompletedTask;
      }).Execute();
    }

    private Race? Race { get; set; }
    private ServerSettings? ServerSettings { get; set; }
  }
}
