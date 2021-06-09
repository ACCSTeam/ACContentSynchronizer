using Avalonia.Animation;
using Avalonia.Media;
using System;
using System.Reactive.Linq;
using Avalonia.Animation.Animators;

namespace ACContentSynchronizer.ClientGui.Styles {
  public class BrushAnimator : Animator<IBrush> {
    public override SolidColorBrush Interpolate(double progress, IBrush oldIValue, IBrush newIValue) {
      var oldValue = (SolidColorBrush) oldIValue;
      var newValue = (SolidColorBrush) newIValue;
      var color = new ColorAnimator().Interpolate(progress, oldValue.Color, newValue.Color);
      return new(color);
    }
  }

  public class SolidColorBrushTransition : AnimatorDrivenTransition<IBrush, BrushAnimator> {
  }
}
