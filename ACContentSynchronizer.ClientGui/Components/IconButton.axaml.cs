using System;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Material.Icons;
using ReactiveUI;

namespace ACContentSynchronizer.ClientGui.Components {
  public class IconButton : UserControl {
    public static readonly DirectProperty<IconButton, Classes?> StyleProperty =
      AvaloniaProperty.RegisterDirect<IconButton, Classes?>(
        nameof(Style),
        o => o.Style,
        (o, v) => o.Style = v);

    public static readonly DirectProperty<IconButton, object?> ParameterProperty =
      AvaloniaProperty.RegisterDirect<IconButton, object?>(
        nameof(Parameter),
        o => o.Parameter,
        (o, v) => o.Parameter = v);

    public static readonly DirectProperty<IconButton, string> TextProperty =
      AvaloniaProperty.RegisterDirect<IconButton, string>(
        nameof(Text),
        o => o.Text,
        (o, v) => o.Text = v);

    public static readonly DirectProperty<IconButton, MaterialIconKind?> IconProperty =
      AvaloniaProperty.RegisterDirect<IconButton, MaterialIconKind?>(
        nameof(Icon),
        o => o.Icon,
        (o, v) => o.Icon = v);

    public static readonly DirectProperty<IconButton, double> SpacingProperty =
      AvaloniaProperty.RegisterDirect<IconButton, double>(
        nameof(Spacing),
        o => o.Spacing,
        (o, v) => o.Spacing = v);

    public static readonly DirectProperty<IconButton, ICommand?> CommandProperty =
      AvaloniaProperty.RegisterDirect<IconButton, ICommand?>(
        nameof(Command),
        o => o.Command,
        (o, v) => o.Command = v);

    private ICommand? _command;

    private MaterialIconKind? _icon;

    private object? _parameter;

    private double _spacing;

    private Classes? _style;

    private string _text = "";

    public IconButton() {
      InitializeComponent();
    }

    public Classes? Style {
      get => _style;
      set => SetAndRaise(StyleProperty, ref _style, value);
    }

    public object? Parameter {
      get => _parameter;
      set => SetAndRaise(ParameterProperty, ref _parameter, value);
    }

    public string Text {
      get => _text;
      set => SetAndRaise(TextProperty, ref _text, value);
    }

    public MaterialIconKind? Icon {
      get => _icon;
      set => SetAndRaise(IconProperty, ref _icon, value);
    }

    public double Spacing {
      get => _spacing;
      set => SetAndRaise(SpacingProperty, ref _spacing, value);
    }

    public ICommand? Command {
      get => _command;
      set => SetAndRaise(CommandProperty, ref _command, value);
    }

    public static readonly DirectProperty<IconButton, bool> TextVisibleProperty =
      AvaloniaProperty.RegisterDirect<IconButton, bool>(
        nameof(TextVisible),
        o => o.TextVisible,
        (o, v) => o.TextVisible = v);

    public bool TextVisible {
      get => _textVisible;
      set => SetAndRaise(TextVisibleProperty, ref _textVisible, value);
    }

    private bool _textVisible;

    private void InitializeComponent() {
      AvaloniaXamlLoader.Load(this);

      this.WhenAnyValue(button => button.Width > 100)
        .Subscribe(x => TextVisible = x);
    }
  }
}
