using System.Threading.Tasks;
using ACContentSynchronizer.ClientGui.ViewModels;
using Avalonia.Controls;
using ReactiveUI;

namespace ACContentSynchronizer.ClientGui.Windows {
  public class SettingsWindowViewModel : ViewModelBase {
    private string _path = "";

    public string Path {
      get => _path;
      set => this.RaiseAndSetIfChanged(ref _path, value);
    }

    public async Task SelectPath(SettingsWindow parent) {
      Path = await new OpenFolderDialog().ShowAsync(parent);
    }
  }
}
