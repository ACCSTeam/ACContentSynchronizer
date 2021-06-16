using ACContentSynchronizer.Client.Models;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ACContentSynchronizer.ClientGui.Views {
  public class Layout : UserControl {
    private static Layout? _instance;

    public static Layout Instance => _instance ??= new();

    public Layout() {
      InitializeComponent();

      _instance = this;
    }

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
