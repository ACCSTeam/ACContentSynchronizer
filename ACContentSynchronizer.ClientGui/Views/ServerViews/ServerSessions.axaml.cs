using System;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ACContentSynchronizer.ClientGui.Views.ServerViews {
  public class ServerSessions : UserControl {
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
