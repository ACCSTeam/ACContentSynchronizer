using System.Collections.Generic;
using ACContentSynchronizer.Extensions;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ACContentSynchronizer.ClientGui.Components.Server {
  public class ServerSessions : UserControl {
    private static ServerSessions? _instance;
    private readonly ServerSessionsViewModel _vm;

    public ServerSessions() {
      DataContext = _vm = new();
      InitializeComponent();
    }

    public static ServerSessions Instance => _instance ??= new();
    public static ServerSessionsViewModel ViewModel => Instance._vm;

    private void InitializeComponent() {
      AvaloniaXamlLoader.Load(this);
    }
  }
}
