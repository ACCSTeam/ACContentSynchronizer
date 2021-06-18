using System;
using System.Runtime.Serialization;
using ACContentSynchronizer.ClientGui.ViewModels;
using ReactiveUI;

namespace ACContentSynchronizer.ClientGui.Models {
  public class ServerEntry : ViewModelBase {
    private string? _name;

    public DateTime DateTime { get; set; } = DateTime.Now;

    private string _ip = "localhost";

    public string Ip {
      get => _ip;
      set => this.RaiseAndSetIfChanged(ref _ip, value);
    }

    [IgnoreDataMember]
    public string Http => $"http://{Ip}";

    public string Name {
      get => _name ?? Ip;
      set => this.RaiseAndSetIfChanged(ref _name, value);
    }

    public string Password { get; set; } = "";
  }
}
