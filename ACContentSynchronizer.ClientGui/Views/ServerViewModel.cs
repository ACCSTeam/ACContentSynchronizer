using System.Threading.Tasks;
using ACContentSynchronizer.ClientGui.Modals;
using ACContentSynchronizer.ClientGui.Models;
using ACContentSynchronizer.ClientGui.Tasks;
using ACContentSynchronizer.ClientGui.ViewModels;
using Avalonia.Collections;
using ReactiveUI;

namespace ACContentSynchronizer.ClientGui.Views {
  public class ServerViewModel : ViewModelBase {
    private AvaloniaList<ContentEntry> _cars = new();

    private ContentEntry? _selectedCar;

    private ServerEntry _serverEntry = new();

    private ContentEntry _track = new();

    public ServerEntry ServerEntry {
      get => _serverEntry;
      set => this.RaiseAndSetIfChanged(ref _serverEntry, value);
    }

    public AvaloniaList<ContentEntry> Cars {
      get => _cars;
      set => this.RaiseAndSetIfChanged(ref _cars, value);
    }

    public ContentEntry? SelectedCar {
      get => _selectedCar;
      set => this.RaiseAndSetIfChanged(ref _selectedCar, value);
    }

    public ContentEntry Track {
      get => _track;
      set => this.RaiseAndSetIfChanged(ref _track, value);
    }

    public void Join() {
    }

    public void Refresh() {
    }

    public void ValidateContent() {
      StatusBar.Instance.AddTask(new ValidationTask(ServerEntry));
    }

    public async Task UploadContent() {
      var content = await Modal.Open<UploadModal, UploadViewModel?>();
      if (content != null) {
        StatusBar.Instance.AddTask(new UploadTask(ServerEntry, content));
      }
    }
  }
}
