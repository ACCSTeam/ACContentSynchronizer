using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ACContentSynchronizer.ClientGui.Components {
  public class SidebarButton : UserControl {
    public static readonly DirectProperty<SidebarButton, string> LabelProperty =
      AvaloniaProperty.RegisterDirect<SidebarButton, string>(
        nameof(Label),
        o => o.Label,
        (o, v) => o.Label = v);

    public static readonly DirectProperty<SidebarButton, bool> IsMinimizedProperty =
      AvaloniaProperty.RegisterDirect<SidebarButton, bool>(
        nameof(IsMinimized),
        o => o.IsMinimized,
        (o, v) => o.IsMinimized = v);

    public static readonly DirectProperty<SidebarButton, ICommand?> CommandProperty =
      AvaloniaProperty.RegisterDirect<SidebarButton, ICommand?>(
        nameof(Command),
        o => o.Command,
        (o, v) => o.Command = v);

    private ICommand? _command;

    private bool _isMinimized;

    private string _label = "";

    public SidebarButton() {
      InitializeComponent();
    }

    public string Label {
      get => _label;
      set => SetAndRaise(LabelProperty, ref _label, value);
    }

    public bool IsMinimized {
      get => _isMinimized;
      set => SetAndRaise(IsMinimizedProperty, ref _isMinimized, !value);
    }

    public ICommand? Command {
      get => _command;
      set => SetAndRaise(CommandProperty, ref _command, value);
    }

    private void InitializeComponent() {
      AvaloniaXamlLoader.Load(this);
    }
  }
}
