using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ACContentSynchronizer.ClientGui.Components.Server {
  public class ServerConditions : UserControl {
    private static ServerConditions? _instance;

    public ServerConditions() {
      InitializeComponent();
    }

    public static ServerConditions Instance => _instance ??= new();

    private void InitializeComponent() {
      AvaloniaXamlLoader.Load(this);
    }
  }
}
