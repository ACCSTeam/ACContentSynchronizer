using System;
using ACContentSynchronizer.ClientGui.Models;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ReactiveUI;

namespace ACContentSynchronizer.ClientGui.Views {
  public class Server : UserControl, IDisposable {
    private static Server? _instance;
    private readonly ServerViewModel _vm;

    public Server() {
      DataContext = _vm = new();
      InitializeComponent();
    }

    public static Server Instance => _instance ??= new();

    public ServerEntry GetServer => _vm.ServerEntry;

    public void Dispose() {
      _vm.Dispose();
    }

    private void InitializeComponent() {
      AvaloniaXamlLoader.Load(this);
    }

    public void SetServer(ServerEntry serverEntry) {
      _vm.ServerEntry = serverEntry;
      ReactiveCommand.CreateFromTask(_vm.Refresh).Execute();
    }

    private void UpdateCars(object? sender, SelectionChangedEventArgs e) {
      ReactiveCommand.CreateFromTask(_vm.UpdateCars).Execute();
    }
  }
}
