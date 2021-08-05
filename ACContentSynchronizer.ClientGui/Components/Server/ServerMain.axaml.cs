using System.Collections.Generic;
using System.Linq;
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

    public void SelectCars(object? sender, RoutedEventArgs e) {
      this.FindControl<Popup>("SelectCars").Open();
    }

    public void SelectTrack(object? sender, RoutedEventArgs e) {
      this.FindControl<Popup>("SelectTrack").Open();
    }

    public Dictionary<string, Dictionary<string, string>> ToConfig(
      Dictionary<string, Dictionary<string, string>> source) {
      source["SERVER"]["NAME"] = _vm.ServerName;
      source["SERVER"]["CARS"] = string.Join(';', _vm.SelectedCars.Select(x => DirectoryUtils.Name(x.Path)).Distinct());
      source["SERVER"]["TRACK"] = _vm.SelectedTrack != null ? DirectoryUtils.Name(_vm.SelectedTrack.Path) : "";
      source["SERVER"]["CONFIG_TRACK"] = _vm.SelectedTrack?.SelectedVariation ?? "";
      source["SERVER"]["PASSWORD"] = _vm.Password;
      source["SERVER"]["ADMIN_PASSWORD"] = _vm.AdminPassword;
      source["SERVER"]["UDP_PORT"] = _vm.UdpPort.ToString();
      source["SERVER"]["TCP_PORT"] = _vm.TcpPort.ToString();
      source["SERVER"]["HTTP_PORT"] = _vm.HttpPort.ToString();
      source["SERVER"]["MAX_CLIENTS"] = _vm.SelectedCars.Count.ToString();
      source["SERVER"]["NUM_THREADS"] = _vm.Threads.ToString();
      source["SERVER"]["CLIENT_SEND_INTERVAL_HZ"] = _vm.PacketSize.ToString();
      source["SERVER"]["REGISTER_TO_LOBBY"] = _vm.PublicServer ? "1" : "0";

      return source;
    }
  }
}
