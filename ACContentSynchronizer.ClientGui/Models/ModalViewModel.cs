using ACContentSynchronizer.ClientGui.ViewModels;

namespace ACContentSynchronizer.ClientGui.Models {
  public class ModalViewModel<T> : ViewModelBase where T : Modal {
    public T Instance { get; set; } = null!;

    protected void Close() {
      Instance.Close();
    }
  }
}
