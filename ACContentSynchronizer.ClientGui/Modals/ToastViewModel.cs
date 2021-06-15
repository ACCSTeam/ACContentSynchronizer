using ACContentSynchronizer.ClientGui.ViewModels;

namespace ACContentSynchronizer.ClientGui.Modals {
  public class ToastViewModel : ViewModelBase {
    public ToastViewModel(string message) {
      Message = message;
    }

    public ToastViewModel() {
    }

    public string Message { get; } = "";
  }
}
