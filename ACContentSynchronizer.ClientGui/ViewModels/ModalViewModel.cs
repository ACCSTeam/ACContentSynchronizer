using ACContentSynchronizer.ClientGui.Models;

namespace ACContentSynchronizer.ClientGui.ViewModels {
  public class ModalViewModel<T> : ViewModelBase where T : Modal {
    public T Instance { get; set; } = null!;

    protected void Close() {
      Instance.Close();
    }
  }
}
