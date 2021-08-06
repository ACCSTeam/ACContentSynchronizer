using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Markup.Xaml;

namespace ACContentSynchronizer.ClientGui.Components {
  public class FormField : UserControl {
    public static readonly DirectProperty<FormField, string> LabelProperty =
      AvaloniaProperty.RegisterDirect<FormField, string>(
        nameof(Label),
        o => o.Label,
        (o, v) => o.Label = v);

    public static readonly DirectProperty<FormField, Classes?> LabelStyleProperty =
      AvaloniaProperty.RegisterDirect<FormField, Classes?>(
        nameof(LabelStyle),
        o => o.LabelStyle,
        (o, v) => o.LabelStyle = v);

    public static readonly DirectProperty<FormField, GridLength> LabelWidthProperty =
      AvaloniaProperty.RegisterDirect<FormField, GridLength>(
        nameof(LabelWidth),
        o => o.LabelWidth,
        (o, v) => o.LabelWidth = v);

    public static readonly DirectProperty<FormField, string> PostfixProperty =
      AvaloniaProperty.RegisterDirect<FormField, string>(
        nameof(Postfix),
        o => o.Postfix,
        (o, v) => o.Postfix = v);

    public static readonly DirectProperty<FormField, object?> ValueProperty =
      AvaloniaProperty.RegisterDirect<FormField, object?>(
        nameof(Value),
        o => o.Value,
        (o, v) => o.Value = v,
        defaultBindingMode: BindingMode.TwoWay,
        enableDataValidation: true);

    public static readonly DirectProperty<FormField, bool> ValueVisibleProperty =
      AvaloniaProperty.RegisterDirect<FormField, bool>(
        nameof(ValueVisible),
        o => o.ValueVisible,
        (o, v) => o.ValueVisible = v);

    public static readonly DirectProperty<FormField, string?> ValueFormatProperty =
      AvaloniaProperty.RegisterDirect<FormField, string?>(
        nameof(ValueFormat),
        o => o.ValueFormat,
        (o, v) => o.ValueFormat = v);

    private string _label = "";

    private Classes? _labelStyle;

    private GridLength _labelWidth = GridLength.Auto;

    private string _postfix = "";

    private object? _value;

    private string? _valueFormat;

    private bool _valueVisible;

    public FormField() {
      InitializeComponent();
    }

    public Classes? LabelStyle {
      get => _labelStyle;
      set => SetAndRaise(LabelStyleProperty, ref _labelStyle, value);
    }

    public string Label {
      get => $"{_label}:";
      set => SetAndRaise(LabelProperty, ref _label, value);
    }

    public string Postfix {
      get => _postfix;
      set => SetAndRaise(PostfixProperty, ref _postfix, value);
    }

    public GridLength LabelWidth {
      get => _labelWidth;
      set => SetAndRaise(LabelWidthProperty, ref _labelWidth, value);
    }

    public object? Value {
      get => _value;
      set {
        try {
          SetAndRaise(ValueProperty, ref _value, value);
        } finally {
          ValueVisible = value != null;
        }
      }
    }

    public string? ValueFormat {
      get => _valueFormat;
      set => SetAndRaise(ValueFormatProperty, ref _valueFormat, value);
    }

    public bool ValueVisible {
      get => _valueVisible;
      set => SetAndRaise(ValueVisibleProperty, ref _valueVisible, value);
    }

    private void InitializeComponent() {
      AvaloniaXamlLoader.Load(this);
    }
  }
}
