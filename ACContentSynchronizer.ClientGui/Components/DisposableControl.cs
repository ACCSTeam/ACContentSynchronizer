using System;
using Avalonia.Controls;

namespace ACContentSynchronizer.ClientGui.Components {
  public class DisposableControl : UserControl, IDisposable {

    public virtual void Dispose() {
      if (DataContext is IDisposable disposable) {
        disposable.Dispose();
      }
    }
  }
}
