using ACContentSynchronizer.ClientGui.Models;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace ACContentSynchronizer.ClientGui.Modals {
  public class UploadModal : Modal {
    private readonly UploadViewModel _vm;

    public UploadModal() {
      DataContext = _vm = new();

      InitializeComponent();
    }

    private void InitializeComponent() {
      AvaloniaXamlLoader.Load(this);
    }

    private void Upload(object? sender, RoutedEventArgs e) {
      Close(_vm);
    }

    private void Close(object? sender, RoutedEventArgs e) {
      Close();
    }
  }
}
