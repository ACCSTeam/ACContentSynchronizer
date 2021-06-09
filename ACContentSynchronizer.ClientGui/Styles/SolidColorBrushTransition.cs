using Avalonia.Animation;
using Avalonia.Media;
using System;
using System.Reactive.Linq;

namespace ACContentSynchronizer.ClientGui.Styles {
  public class SolidColorBrushTransition : Transition<IBrush> {
    public override IObservable<IBrush> DoTransition(IObservable<double> progress, IBrush oldValue, IBrush newValue) {
      if (oldValue is not ISolidColorBrush oldBrush) {
        throw new ArgumentException("Only instances of ISolidColorBrush are supported", nameof(oldValue));
      }
      if (newValue is not ISolidColorBrush newBrush) {
        throw new ArgumentException("Only instances of ISolidColorBrush are supported", nameof(newValue));
      }

      var oldColor = oldBrush.Color;
      var newColor = newBrush.Color;

      return progress.Select(p => {
        var e = Easing.Ease(p);
        var a = (int) (e * (newColor.A - oldColor.A) + 0.5) + oldColor.A;
        var r = (int) (e * (newColor.R - oldColor.R) + 0.5) + oldColor.R;
        var g = (int) (e * (newColor.G - oldColor.G) + 0.5) + oldColor.G;
        var b = (int) (e * (newColor.B - oldColor.B) + 0.5) + oldColor.B;

        return new SolidColorBrush(new Color((byte) a, (byte) r, (byte) g, (byte) b));
      });
    }
  }
}
