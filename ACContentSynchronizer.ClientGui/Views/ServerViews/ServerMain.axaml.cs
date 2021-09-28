using ACContentSynchronizer.ClientGui.Components;
using ACContentSynchronizer.ClientGui.Extensions;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace ACContentSynchronizer.ClientGui.Views.ServerViews {
  public class ServerMain : DisposableControl {
    public ServerMain() {
      InitializeComponent();
    }

    private void InitializeComponent() {
      AvaloniaXamlLoader.Load(this);
    }

    public void SelectCarsClick(object? sender, RoutedEventArgs e) {
      this.OpenPopup("SelectCars");
    }

    public void SelectTrackClick(object? sender, RoutedEventArgs e) {
      this.OpenPopup("SelectTrack");
    }
  }
}
