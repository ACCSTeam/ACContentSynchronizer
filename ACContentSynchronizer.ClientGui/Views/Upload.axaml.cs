using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace ACContentSynchronizer.ClientGui.Views {
  public class Upload : UserControl {
    private readonly UploadViewModel _vm;

    public Upload() {
      DataContext = _vm = new();
      InitializeComponent();
    }

    private void InitializeComponent() {
      AvaloniaXamlLoader.Load(this);
    }

    private void OpenPopup(object? sender, RoutedEventArgs e) {
      this.FindControl<Popup>("AddPopup").IsOpen = true;
    }
  }
}
