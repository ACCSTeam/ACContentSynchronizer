using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;

namespace ACContentSynchronizer.ClientGui.Modals {
  public class Toast : Window {
    private readonly ToastViewModel _vm;

    public Toast() {
      DataContext = _vm = new();
    }

    public Toast(string message) {
      DataContext = _vm = new(message);
      InitializeComponent();
#if DEBUG
      this.AttachDevTools();
#endif
    }

    private void InitializeComponent() {
      AvaloniaXamlLoader.Load(this);

      Task.Factory.StartNew(async () => {
        await Task.Delay(TimeSpan.FromSeconds(10));
        await Dispatcher.UIThread.InvokeAsync(CloseInternal);
      });
    }

    public static void Open(string message) {
      var toast = new Toast(message) {
        Width = ToastWidth,
      };

      if (ToastsActivated.Count > ActiveToastCount) {
        return;
      }

      var bounds = toast.Screens.Primary.Bounds;
      var x = bounds.Width - ToastWidth - 30;
      var lastToast = ToastsActivated.OrderByDescending(t => t.Position.Y).FirstOrDefault();
      var prevY = (int) ((lastToast?.Position.Y + lastToast?.Height) ?? 0);
      var y = prevY + 20;
      toast.Position = new(x, y);

      ToastsActivated.Add(toast);

      toast.Show();
    }

    private void Close(object? sender, RoutedEventArgs e) {
      CloseInternal();
    }

    private void CloseInternal() {
      Close();
      ToastsActivated.Remove(this);
    }

    private const int ToastWidth = 300;
    private const int ActiveToastCount = 3;
    private static List<Toast> ToastsActivated { get; set; } = new();
  }
}
