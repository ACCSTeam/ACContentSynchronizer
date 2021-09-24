using System;
using System.Runtime.Serialization;
using ACContentSynchronizer.ClientGui.ViewModels;
using ReactiveUI;

namespace ACContentSynchronizer.ClientGui.Models {
  public class ServerEntryViewModel : ViewModelBase {
    private string _clientId = "";
    private string _ip = "";
    private string _kunosPort = "";
    private string? _name;

    private string _port = "";

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

    public string KunosPort {
      get => _kunosPort;
      set => this.RaiseAndSetIfChanged(ref _kunosPort, value);
    }

    public string ClientId {
      get => _clientId;
      set => this.RaiseAndSetIfChanged(ref _clientId, value);
    }
  }
}
