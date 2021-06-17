using ACContentSynchronizer.ClientGui.Models;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ACContentSynchronizer.ClientGui.Views {
  public class StatusBar : UserControl {
    private static StatusBar? _instance;
    private readonly StatusBarViewModel _vm;

    public StatusBar() {
      DataContext = _vm = new();
      InitializeComponent();

      _instance = this;
    }

    public static StatusBar Instance => _instance ??= new();

    private void InitializeComponent() {
      AvaloniaXamlLoader.Load(this);
    }

    public void AddTask(TaskViewModel task) {
      _vm.AddTask(task);
    }
  }
}
