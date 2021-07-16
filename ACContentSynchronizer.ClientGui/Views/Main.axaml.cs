using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ACContentSynchronizer.ClientGui.Views {
  public class Main : UserControl {
    private static Main? _instance;
    private readonly MainViewModel _vm;

    public Main() {
      DataContext = _vm = new();
      InitializeComponent();
    }

    public static Main Instance => _instance ??= new();

    private void InitializeComponent() {
      AvaloniaXamlLoader.Load(this);
    }
  }
}
