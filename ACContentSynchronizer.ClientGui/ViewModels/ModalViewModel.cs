namespace ACContentSynchronizer.ClientGui.ViewModels {
  public class ModalViewModel : ViewModelBase {
    public delegate void RequestClose();

    public event RequestClose? CloseRequest;

    protected void Close() {
      CloseRequest?.Invoke();
    }
  }
}
