using System.Threading.Tasks;
using ACContentSynchronizer.ClientGui.ViewModels;

namespace ACContentSynchronizer.ClientGui.Views {
  public class MainViewModel : ViewModelBase {
    public Task AddNewConnection() {
      return Sidebar.Instance.Vm.OpenAddServerDialog();
    }
  }
}
