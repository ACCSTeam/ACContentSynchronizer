using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ACContentSynchronizer.ClientGui.Components.Server {
  public class ServerConditions : UserControl {
    private static ServerConditions? _instance;
    private readonly ServerConditionsViewModel _vm;

    public ServerConditions() {
      DataContext = _vm = new();
      InitializeComponent();
    }

    public static ServerConditions Instance => _instance ??= new();
    public static ServerConditionsViewModel ViewModel => Instance._vm;

    private void InitializeComponent() {
      AvaloniaXamlLoader.Load(this);
    }
  }
}
