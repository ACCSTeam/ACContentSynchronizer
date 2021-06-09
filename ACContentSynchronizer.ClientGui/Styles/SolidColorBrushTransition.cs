using Avalonia.Animation;
using Avalonia.Media;
using System;
using System.Reactive.Linq;
using Avalonia.Animation.Animators;

namespace ACContentSynchronizer.ClientGui.Styles {
  public class BrushAnimator : Animator<IBrush> {
    public override SolidColorBrush Interpolate(double progress, IBrush oldValue, IBrush newValue) {
      if (oldValue is not ISolidColorBrush oldBrush) {
        throw new ArgumentException("Only instances of ISolidColorBrush are supported", nameof(oldValue));
      }
      if (newValue is not ISolidColorBrush newBrush) {
        throw new ArgumentException("Only instances of ISolidColorBrush are supported", nameof(newValue));
      }

      var color = new ColorAnimator().Interpolate(progress, oldBrush.Color, newBrush.Color);
      return new(color);
    }
  }

  public class SolidColorBrushTransition : AnimatorDrivenTransition<IBrush, BrushAnimator> {
  }
}
