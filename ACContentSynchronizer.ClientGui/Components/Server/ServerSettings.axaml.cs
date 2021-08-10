using ACContentSynchronizer.ClientGui.Models;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ReactiveUI;

namespace ACContentSynchronizer.ClientGui.Components.Server {
  public class ServerSettings : UserControl {
    private static ServerSettings? _instance;
    private readonly ServerSettingsViewModel _vm;

    public ServerSettings() {
      DataContext = _vm = new();
      InitializeComponent();
    }

    public static ServerSettings Instance => _instance ??= new();

    private void InitializeComponent() {
      AvaloniaXamlLoader.Load(this);
    }

    public void Load(ServerEntry server) {
      ReactiveCommand.CreateFromTask(() => _vm.Load(server))
        .Execute();
    }
  }
}
