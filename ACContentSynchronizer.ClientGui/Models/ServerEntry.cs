using System;
using System.Runtime.Serialization;
using ACContentSynchronizer.ClientGui.ViewModels;
using ReactiveUI;

namespace ACContentSynchronizer.ClientGui.Models {
  public class ServerEntry : ViewModelBase {
    private string _httpPort = "";
    private string _ip = "localhost";
    private string? _name;

    private string _port = "5010";

    public DateTime DateTime { get; set; } = DateTime.Now;

    public string Ip {
      get => _ip;
      set => this.RaiseAndSetIfChanged(ref _ip, value);
    }

    public string Port {
      get => _port;
      set => this.RaiseAndSetIfChanged(ref _port, value);
    }

    [IgnoreDataMember]
    public string Http => $"http://{Ip}:{Port}";

    public string Name {
      get => _name ?? Ip;
      set => this.RaiseAndSetIfChanged(ref _name, value);
    }

    public string Password { get; set; } = "";

    public string HttpPort {
      get => _httpPort;
      set => this.RaiseAndSetIfChanged(ref _httpPort, value);
    }
  }
}
