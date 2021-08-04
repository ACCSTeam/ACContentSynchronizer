using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ACContentSynchronizer.ClientGui.Components.Server {
  public class ServerRules : UserControl {
    private static ServerRules? _instance;

    public ServerRules() {
      InitializeComponent();
    }

    public static ServerRules Instance => _instance ??= new();

    private void InitializeComponent() {
      AvaloniaXamlLoader.Load(this);
    }
  }
}
