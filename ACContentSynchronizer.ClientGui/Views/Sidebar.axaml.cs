using ACContentSynchronizer.ClientGui.Components;
using ACContentSynchronizer.ClientGui.Models;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ACContentSynchronizer.ClientGui.Views {
  public class Sidebar : DisposableControl {
    private readonly SideBarViewModel _vm;

    public Sidebar() {
      DataContext = _vm = new();
      InitializeComponent();
    }

    private void InitializeComponent() {
      AvaloniaXamlLoader.Load(this);
    }

    public void Save(ServerEntryViewModel server) {
      // var serverEntry = _vm.Servers.FirstOrDefault(x => x.DateTime == server.DateTime);
      // if (serverEntry != null) {
      //   serverEntry.Ip = server.Ip;
      //   serverEntry.Port = server.Port;
      //   serverEntry.Password = server.Password;
      //   serverEntry.Name = server.Name;
      // } else {
      //   _vm.Servers.Add(server);
      // }
    }

    public void UnsetServer() {
      // _vm.SelectedServer = null;
    }
  }
}
