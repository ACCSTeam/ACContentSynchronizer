using System;
using System.Collections.Generic;
using System.Linq;
using ACContentSynchronizer.ClientGui.Models;
using ACContentSynchronizer.Extensions;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace ACContentSynchronizer.ClientGui.Components.Server {
  public class ServerMain : UserControl, IDisposable {
    private static ServerMain? _instance;
    private readonly ServerMainViewModel _vm;

    public ServerMain() {
      DataContext = _vm = new();
      InitializeComponent();
    }

    public static ServerMain Instance => _instance ??= new();
    public static ServerMainViewModel ViewModel => Instance._vm;

    public void Dispose() {
      _vm.Dispose();
    }

    private void InitializeComponent() {
      AvaloniaXamlLoader.Load(this);
    }

    public void SelectCars(object? sender, RoutedEventArgs e) {
      this.FindControl<Popup>("SelectCars").Open();
    }

    public void SelectTrack(object? sender, RoutedEventArgs e) {
      this.FindControl<Popup>("SelectTrack").Open();
    }
  }
}
