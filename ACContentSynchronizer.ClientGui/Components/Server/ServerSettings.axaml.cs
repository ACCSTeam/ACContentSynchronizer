using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ACContentSynchronizer.ClientGui.Components.Server {
  public class ServerSettings : UserControl {
    private static ServerSettings? _instance;

    public ServerSettings() {
      InitializeComponent();
    }

    public static ServerSettings Instance => _instance ??= new();

    private void InitializeComponent() {
      AvaloniaXamlLoader.Load(this);
    }
  }
}
