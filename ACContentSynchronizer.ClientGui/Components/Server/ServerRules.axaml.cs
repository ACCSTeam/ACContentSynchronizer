using System.Collections.Generic;
using ACContentSynchronizer.Extensions;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ACContentSynchronizer.ClientGui.Components.Server {
  public class ServerRules : UserControl {
    private static ServerRules? _instance;
    private readonly ServerRulesViewModel _vm;

    public ServerRules() {
      DataContext = _vm = new();
      InitializeComponent();
    }

    public static ServerRules Instance => _instance ??= new();
    public static ServerRulesViewModel ViewModel => Instance._vm;

    private void InitializeComponent() {
      AvaloniaXamlLoader.Load(this);
    }
  }
}
