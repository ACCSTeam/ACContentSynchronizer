using System;
using ACContentSynchronizer.ClientGui.Components;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ACContentSynchronizer.ClientGui.Views.ServerViews {
  public class ServerSessions : DisposableControl {
    public ServerSessions() {
      InitializeComponent();
    }

    private void InitializeComponent() {
      AvaloniaXamlLoader.Load(this);
    }

    public void Load(IniFile serverConfig) {
      throw new NotImplementedException();
    }
  }
}
