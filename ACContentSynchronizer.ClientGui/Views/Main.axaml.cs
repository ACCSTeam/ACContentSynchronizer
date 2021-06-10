using ACContentSynchronizer.ClientGui.Windows;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ReactiveUI;

namespace ACContentSynchronizer.ClientGui.Views {
  public class Main : UserControl {
    private readonly MainViewModel _vm;

    public Main() {
      DataContext = _vm = new();

      InitializeComponent();
    }

    private void InitializeComponent() {
      AvaloniaXamlLoader.Load(this);
    }
  }
}

