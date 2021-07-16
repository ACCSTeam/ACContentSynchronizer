using ACContentSynchronizer.ClientGui.ViewModels;
using ReactiveUI;

namespace ACContentSynchronizer.ClientGui.Views {
  public class LayoutViewModel : ViewModelBase {
    private bool _serverSelected;

    public bool ServerSelected {
      get => _serverSelected;
      set => this.RaiseAndSetIfChanged(ref _serverSelected, value);
    }
  }
}
