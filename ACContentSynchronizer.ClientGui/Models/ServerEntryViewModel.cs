using System;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using ACContentSynchronizer.ClientGui.ViewModels;
using Avalonia.Data;
using ReactiveUI;

namespace ACContentSynchronizer.ClientGui.Models {
  public class ServerEntryViewModel : ViewModelBase {
    private readonly Regex _addressRegex = new($"^{Constants.Pattern}$");
    private string _clientId = "";
    private string _ip = "";
    private string _kunosPort = "";
    private string? _name;

    private string _port = "";

    public DateTime DateTime { get; set; } = DateTime.Now;

    public string Ip {
      get => _ip;
      set {
        if (!_addressRegex.IsMatch(value)) {
          throw new DataValidationException("Incorrect Ip");
        }

        this.RaiseAndSetIfChanged(ref _ip, value);
        this.RaisePropertyChanged(nameof(Http));
      }
    }

    public string Port {
      get => _port;
      set {
        this.RaiseAndSetIfChanged(ref _port, value);
        this.RaisePropertyChanged(nameof(Http));
      }
    }

    [IgnoreDataMember]
    public string Http {
      get {
        if (string.IsNullOrEmpty(Ip) || string.IsNullOrEmpty(Port)) {
          return "";
        }

        return $"http://{Ip}:{Port}";
      }
    }

    public string Name {
      get => _name ?? Ip;
      set => this.RaiseAndSetIfChanged(ref _name, value);
    }

    private string _password="";

    public string Password {
      get => _password;
      set => this.RaiseAndSetIfChanged(ref _password, value);
    }

    public string KunosPort {
      get => _kunosPort;
      set => this.RaiseAndSetIfChanged(ref _kunosPort, value);
    }

    public string ClientId {
      get => _clientId;
      set => this.RaiseAndSetIfChanged(ref _clientId, value);
    }

    private string _serverPreset = "";

    public string ServerPreset {
      get => _serverPreset;
      set => this.RaiseAndSetIfChanged(ref _serverPreset, value);
    }
  }
}
