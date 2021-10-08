using System;
using ACContentSynchronizer.ClientGui.Models;
using Avalonia;
using Avalonia.Markup.Xaml;

namespace ACContentSynchronizer.ClientGui.Modals {
  public class AddNewServer : Modal {
    public AddNewServer() {
      InitializeComponent();
    }

    private void InitializeComponent() {
      AvaloniaXamlLoader.Load(this);
    }
  }
}
