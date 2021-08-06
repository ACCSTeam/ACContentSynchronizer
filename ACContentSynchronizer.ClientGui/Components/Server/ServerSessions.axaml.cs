using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ACContentSynchronizer.ClientGui.Components.Server {
  public class ServerSessions : UserControl {
    private readonly ServerSessionsViewModel _vm;
    private static ServerSessions? _instance;

    public ServerSessions() {
      DataContext = _vm = new();
      InitializeComponent();
    }

    public static ServerSessions Instance => _instance ??= new();

    private void InitializeComponent() {
      AvaloniaXamlLoader.Load(this);
    }
  }
}
