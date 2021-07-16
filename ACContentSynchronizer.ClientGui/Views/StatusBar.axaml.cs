using ACContentSynchronizer.ClientGui.ViewModels;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace ACContentSynchronizer.ClientGui.Views {
  public class StatusBar : UserControl {
    private static StatusBar? _instance;
    private readonly StatusBarViewModel _vm;

    public StatusBar() {
      DataContext = _vm = new();
      InitializeComponent();
    }

    public static StatusBar Instance => _instance ??= new();

    private void InitializeComponent() {
      AvaloniaXamlLoader.Load(this);
    }

    public void AddTask(TaskViewModel task) {
      _vm.AddTask(task);
    }

    private void OpenPopup(object? sender, RoutedEventArgs e) {
      this.FindControl<Popup>("StatusPopup").IsOpen = true;
    }
  }
}
