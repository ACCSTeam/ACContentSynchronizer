using System;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace ACContentSynchronizer.ClientGui.Components.Server {
  public class Upload : UserControl, IDisposable {
    private static Upload? _instance;
    private readonly UploadViewModel _vm;

    public Upload() {
      DataContext = _vm = new();
      InitializeComponent();
    }

    public static Upload Instance => _instance ??= new();

    public void Dispose() {
      _vm.Dispose();
    }

    private void InitializeComponent() {
      AvaloniaXamlLoader.Load(this);
    }

    public void OpenPopup(object? sender, RoutedEventArgs e) {
      this.FindControl<Popup>("AddPopup").IsOpen = true;
    }
  }
}
