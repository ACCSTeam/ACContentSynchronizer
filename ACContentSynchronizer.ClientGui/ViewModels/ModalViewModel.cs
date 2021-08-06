using ACContentSynchronizer.ClientGui.Models;

namespace ACContentSynchronizer.ClientGui.ViewModels {
  public class ModalViewModel<T> : ViewModelBase where T : Modal {
    public T ControlInstance { get; set; } = null!;

    protected void Close() {
      ControlInstance.Close();
    }
  }
}
