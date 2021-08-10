using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ACContentSynchronizer.ClientGui.Models;
using ACContentSynchronizer.ClientGui.ViewModels;
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

    public IniFile Save(IniFile source) {
      var cars = string.Join(';', _vm.SelectedCars
        .Select(x => x.EntryName)
        .Distinct());

      source["SERVER"]["NAME"] = _vm.ServerName;
      source["SERVER"]["CARS"] = cars;
      source["SERVER"]["CONFIG_TRACK"] = _vm.SelectedTrack?.SelectedVariation;
      source["SERVER"]["TRACK"] = _vm.SelectedTrack?.EntryName;
      source["SERVER"]["PASSWORD"] = _vm.Password;
      source["SERVER"]["ADMIN_PASSWORD"] = _vm.AdminPassword;
      source["SERVER"]["UDP_PORT"] = _vm.UdpPort;
      source["SERVER"]["TCP_PORT"] = _vm.TcpPort;
      source["SERVER"]["HTTP_PORT"] = _vm.HttpPort;
      source["SERVER"]["CLIENT_SEND_INTERVAL_HZ"] = _vm.PacketSize;
      source["SERVER"]["REGISTER_TO_LOBBY"] = _vm.PublicServer.ToInt();
      source["SERVER"]["MAX_CLIENTS"] = _vm.SelectedCars.Count;
      source["SERVER"]["NUM_THREADS"] = _vm.Threads;
      source["SERVER"]["WELCOME_MESSAGE"] = _vm.WelcomeMessage;

      return source;
    }

    public IniFile SaveEntries() {
      var entryList = new IniFile();
      for (var i = 0; i < _vm.SelectedCars.Count; i++) {
        var car = _vm.SelectedCars[i];
        entryList.Add($"CAR_{i}", new() {
          ["MODEL"] = car.EntryName,
          ["SKIN"] = car.SelectedVariation,
          ["SPECTATOR_MODE"] = 0,
          ["DRIVERNAME"] = "",
          ["TEAM"] = "",
          ["GUID"] = "",
          ["BALLAST"] = 0,
          ["RESTRICTOR"] = 0,
        });
      }

      return entryList;
    }

    public void Load(IniFile serverConfig, IniFile entryList) {

      _vm.ServerName = serverConfig.V("SERVER", "NAME", _vm.ServerName);
      _vm.Password = serverConfig.V("SERVER", "PASSWORD", _vm.Password);
      _vm.AdminPassword = serverConfig.V("SERVER", "ADMIN_PASSWORD", _vm.AdminPassword);
      _vm.PublicServer = serverConfig.V("SERVER", "REGISTER_TO_LOBBY", _vm.PublicServer);
      _vm.UdpPort = serverConfig.V("SERVER", "UDP_PORT", _vm.UdpPort);
      _vm.TcpPort = serverConfig.V("SERVER", "TCP_PORT", _vm.TcpPort);
      _vm.HttpPort = serverConfig.V("SERVER", "HTTP_PORT", _vm.HttpPort);
      _vm.PacketSize = serverConfig.V("SERVER", "CLIENT_SEND_INTERVAL_HZ", _vm.PacketSize);
      _vm.Threads = serverConfig.V("SERVER", "NUM_THREADS", _vm.Threads);
      _vm.WelcomeMessage = serverConfig.V("SERVER", "WELCOME_MESSAGE", _vm.WelcomeMessage);

      _vm.SelectedCars = new();

      foreach (var (_, car) in entryList) {
        var name = car.V("MODEL", "");
        if (string.IsNullOrEmpty(name)) {
          continue;
        }

        var skin = car.V("SKIN", "");
        if (string.IsNullOrEmpty(skin)) {
          continue;
        }

        _vm.SelectedCars.Add(new() {
          Name = ContentUtils.GetCarName(name, Settings.Instance.GamePath) ?? name,
          Preview = Preview.GetCarPreview(name, skin),
          Path = name,
          Variations = new() {
            skin,
          },
          SelectedVariation = skin,
        });
      }

      var track = serverConfig.V("SERVER", "TRACK", string.Empty);

      if (string.IsNullOrEmpty(track)) {
        return;
      }

      var trackVariation = serverConfig.V("SERVER", "CONFIG_TRACK", string.Empty);
      var trackNames = ContentUtils.GetTrackName(track, Settings.Instance.GamePath);

      _vm.SelectedTrack = new() {
        Path = track,
        Name = (string.IsNullOrEmpty(trackVariation)
            ? trackNames.FirstOrDefault()?.Name
            : trackNames.FirstOrDefault(x => x.Variation == trackVariation)?.Name
          ) ?? track,
        Preview = Preview.GetTrackPreview(track),
        Variations = new() { trackVariation },
        SelectedVariation = trackVariation,
      };
    }
  }
}
