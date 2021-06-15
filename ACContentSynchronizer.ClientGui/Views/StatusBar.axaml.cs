using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ACContentSynchronizer.ClientGui.Views {
  public class StatusBar : UserControl {
    private static StatusBar? _instance;

    public StatusBar() {
      InitializeComponent();

      _instance = this;
    }

    public static StatusBar Instance => _instance ??= new();

    private void InitializeComponent() {
      AvaloniaXamlLoader.Load(this);
    }

    public void AddTask() {
    }
  }
}
