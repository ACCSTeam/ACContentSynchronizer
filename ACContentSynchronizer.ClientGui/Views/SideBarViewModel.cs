using ACContentSynchronizer.ClientGui.ViewModels;
using ReactiveUI;
using Splat;

namespace ACContentSynchronizer.ClientGui.Views {
  public class SideBarViewModel : ViewModelBase {
    private bool _isMinimized;
    private int _size = 300;

    public SideBarViewModel() {
      Application = Locator.Current.GetService<ApplicationViewModel>();
      // var settings = Settings.Instance;
      //
      // Servers = new(settings.Servers);
      IsMinimized = Application.Settings.SidebarMinimized;
    }

    public ApplicationViewModel Application { get; set; }

    public int Size {
      get => _size;
      set => this.RaiseAndSetIfChanged(ref _size, value);
    }

    public bool IsMinimized {
      get => _isMinimized;
      private set {
        this.RaiseAndSetIfChanged(ref _isMinimized, value);

        Size = value
          ? 60
          : 300;

        Application.Settings.SidebarMinimized = value;
        ReactiveCommand.CreateFromTask(Application.SaveAsync).Execute();
      }
    }

    public void Toggle() {
      IsMinimized = !IsMinimized;
    }
  }
}
