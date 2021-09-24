using ACContentSynchronizer.ClientGui.ViewModels;
using Splat;

namespace ACContentSynchronizer.ClientGui.Views {
  public class LayoutViewModel : ViewModelBase {
    public LayoutViewModel() {
      Application = Locator.Current.GetService<ApplicationViewModel>();
      Application.Load();
    }

    public ApplicationViewModel Application { get; set; }

    public void GoBack() {
      Application.UnsetServer();
    }
  }
}
