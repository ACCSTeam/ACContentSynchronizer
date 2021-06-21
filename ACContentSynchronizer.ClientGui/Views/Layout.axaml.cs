using ACContentSynchronizer.ClientGui.Models;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ACContentSynchronizer.ClientGui.Views {
  public class Layout : UserControl {
    private static Layout? _instance;

    public Layout() {
      InitializeComponent();

      _instance = this;
    }

    public static Layout Instance => _instance ??= new();

    private void InitializeComponent() {
      AvaloniaXamlLoader.Load(this);
    }

    public void SelectServer(ServerEntry serverEntry) {
      var carousel = this.FindControl<Carousel>("Carousel");
      carousel.SelectedIndex = 1;
      Server.Instance.SetServer(serverEntry);
    }
  }
}
