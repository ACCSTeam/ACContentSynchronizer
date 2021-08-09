using System;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ReactiveUI;

namespace ACContentSynchronizer.ClientGui.Components.Server {
  public class Race : UserControl, IDisposable {
    private static Race? _instance;
    private readonly RaceViewModel _vm;

    public Race() {
      DataContext = _vm = new();
      InitializeComponent();
    }

    public static Race Instance => _instance ??= new();

    public void Dispose() {
      _vm.Dispose();
    }

    private void InitializeComponent() {
      AvaloniaXamlLoader.Load(this);
    }

    private void UpdateCars(object? sender, SelectionChangedEventArgs e) {
      ReactiveCommand.CreateFromTask(_vm.UpdateCars).Execute();
    }

    public void Refresh() {
      ReactiveCommand.CreateFromTask(_vm.Refresh).Execute();
    }
  }
}
