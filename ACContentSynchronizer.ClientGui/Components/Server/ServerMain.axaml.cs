using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace ACContentSynchronizer.ClientGui.Components.Server {
  public class ServerMain : UserControl {
    private static ServerMain? _instance;
    private readonly ServerMainViewModel _vm;

    public ServerMain() {
      DataContext = _vm = new();
      InitializeComponent();
    }

    public static ServerMain Instance => _instance ??= new();

    private void InitializeComponent() {
      AvaloniaXamlLoader.Load(this);
    }

    private void SelectCars(object? sender, RoutedEventArgs e) {
      this.FindControl<Popup>("SelectCars").Open();
    }

    private void SelectTrack(object? sender, RoutedEventArgs e) {
      this.FindControl<Popup>("SelectTrack").Open();
    }
  }
}
