using ACContentSynchronizer.ClientGui.Components;
using ACContentSynchronizer.ClientGui.Models;
using ACContentSynchronizer.ClientGui.ViewModels;
using Avalonia.Collections;
using Material.Icons;
using Material.Icons.Avalonia;
using ReactiveUI;

namespace ACContentSynchronizer.ClientGui.Views {
  public class SideBarViewModel : ViewModelBase {
    private bool _addServerDialog;

    private bool _isMinimized;

    private object _newContent = "Add new";

    private object _settingsContent = "Settings";

    private int _size = 300;

    public int Size {
      get => _size;
      set => this.RaiseAndSetIfChanged(ref _size, value);
    }

    public bool IsMinimized {
      get => _isMinimized;
      set {
        Size = value ? 60 : 300;
        NewContent = value ? new MaterialIcon { Kind = MaterialIconKind.Plus } : "Add new";
        SettingsContent = value ? new MaterialIcon { Kind = MaterialIconKind.Cog } : "Settings";
        this.RaiseAndSetIfChanged(ref _isMinimized, value);
      }
    }

    public object NewContent {
      get => _newContent;
      set => this.RaiseAndSetIfChanged(ref _newContent, value);
    }

    public object SettingsContent {
      get => _settingsContent;
      set => this.RaiseAndSetIfChanged(ref _settingsContent, value);
    }

    public AvaloniaList<ServerEntry> Servers { get; set; } = new();

    public bool AddServerDialog {
      get => _addServerDialog;
      set => this.RaiseAndSetIfChanged(ref _addServerDialog, value);
    }

    public void Toggle() {
      IsMinimized = !IsMinimized;
    }

    public void OpenAddServerDialog() {
      Modal.Open<AddNewServer>();
    }

    public void AddServer() {
      Servers.Add(new() {
        Ip =
          $"{Servers.Count}.{Servers.Count}.{Servers.Count}.{Servers.Count}:{Servers.Count}{Servers.Count}{Servers.Count}{Servers.Count}",
      });
    }
  }
}
