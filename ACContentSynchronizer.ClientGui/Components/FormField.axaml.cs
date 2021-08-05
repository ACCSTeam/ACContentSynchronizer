using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ACContentSynchronizer.ClientGui.Components {
  public class FormField : UserControl {
    public static readonly DirectProperty<FormField, string> LabelProperty =
      AvaloniaProperty.RegisterDirect<FormField, string>(
        nameof(Label),
        o => o.Label,
        (o, v) => o.Label = v);

    public static readonly DirectProperty<FormField, int> LabelWidthProperty =
      AvaloniaProperty.RegisterDirect<FormField, int>(
        nameof(LabelWidth),
        o => o.LabelWidth,
        (o, v) => o.LabelWidth = v);

    private string _label = "";

    private int _labelWidth = 100;

    public FormField() {
      InitializeComponent();
    }

    public static readonly DirectProperty<FormField, Classes?> LabelStyleProperty =
      AvaloniaProperty.RegisterDirect<FormField, Classes?>(
        nameof(LabelStyle),
        o => o.LabelStyle,
        (o, v) => o.LabelStyle = v);

    public Classes? LabelStyle {
      get => _labelStyle;
      set => SetAndRaise(LabelStyleProperty, ref _labelStyle, value);
    }

    private Classes? _labelStyle;

    public string Label {
      get => _label;
      set => SetAndRaise(LabelProperty, ref _label, value);
    }

    public int LabelWidth {
      get => _labelWidth;
      set => SetAndRaise(LabelWidthProperty, ref _labelWidth, value);
    }

    private void InitializeComponent() {
      AvaloniaXamlLoader.Load(this);
    }
  }
}
