using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

namespace ACContentSynchronizer.ClientGui.Behaviors {
  public class AddClassBehavior : AvaloniaObject, IBehavior {
    public static readonly DirectProperty<AddClassBehavior, Classes?> ClassesProperty =
      AvaloniaProperty.RegisterDirect<AddClassBehavior, Classes?>(
        nameof(Classes),
        o => o.Classes,
        (o, v) => o.Classes = v);

    private Classes? _classes;

    public Classes? Classes {
      get => _classes;
      set => SetAndRaise(ClassesProperty, ref _classes, value);
    }

    public IAvaloniaObject? AssociatedObject { get; private set; }

    public void Attach(IAvaloniaObject? associatedObject) {
      if (!(associatedObject is IStyledElement styledElement)) {
        throw new ArgumentException($"{nameof(AddClassBehavior)} supports only IStyledElement");
      }

      AssociatedObject = associatedObject;

      if (Classes == null || !Classes.Any()) {
        return;
      }

      styledElement.Classes = Classes;
    }

    public void Detach() {
      AssociatedObject = null;
    }

    protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> e) {
      base.OnPropertyChanged(e);

      if (!(AssociatedObject is IStyledElement styledElement)) {
        return;
      }

      if (e.NewValue.GetValueOrDefault<Classes>() is { } newClassName) {
        styledElement.Classes = newClassName;
      }
    }
  }
}
