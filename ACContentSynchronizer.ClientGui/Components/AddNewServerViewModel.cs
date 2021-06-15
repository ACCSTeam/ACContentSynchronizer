using System.Threading.Tasks;
using ACContentSynchronizer.ClientGui.Models;
using ACContentSynchronizer.ClientGui.ViewModels;
using ACContentSynchronizer.ClientGui.Views;
using ReactiveUI;

namespace ACContentSynchronizer.ClientGui.Components {
  public class AddNewServerViewModel : ViewModelBase{
    private string _ip  ="";

    public string Ip {
      get => _ip;
      set => this.RaiseAndSetIfChanged(ref _ip, value);
    }

    private string _password ="";

    public string Password {
      get => _password;
      set => this.RaiseAndSetIfChanged(ref _password, value);
    }

    public void Save() {
      Sidebar.Instance.Vm.Servers.Add(new() {
        Ip = Ip,
        Password =  Password,
      });
    }
  }
}
