using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ReactiveUI;

namespace ACContentSynchronizer.ClientGui.Modals {
  public class Toast : Window {
    public const int ToastWidth = 300;
    public const int ActiveToastCount = 3;
    public const int ToastLifetime = 5;
    private readonly int _toastLifetime;

    public Toast() {
      DataContext = new ToastViewModel();
      _toastLifetime = ToastLifetime;
      InitializeComponent();
    }

    public Toast(int toastLifetime) {
      DataContext = new ToastViewModel();
      _toastLifetime = toastLifetime;
      InitializeComponent();
    }

    private static List<Toast> ToastsActivated { get; } = new();

    private void InitializeComponent() {
      AvaloniaXamlLoader.Load(this);

      ReactiveCommand.CreateFromTask(async () => {
        await Task.Delay(TimeSpan.FromSeconds(_toastLifetime));
        CloseInternal();
      }).Execute();
    }

    public static void Open(string message,
                            int toastWidth = ToastWidth,
                            int activeToastCount = ActiveToastCount,
                            int toastLifetime = ToastLifetime) {
      var toast = new Toast(toastLifetime) {
        DataContext = new ToastViewModel(message),
        Width = toastWidth,
      };

      if (ToastsActivated.Count > activeToastCount) {
        return;
      }

      var bounds = toast.Screens.Primary.Bounds;
      var x = bounds.Width - ToastWidth - 30;

      var lastToast = ToastsActivated.OrderByDescending(t => t.Position.Y).FirstOrDefault();
      var prevY = (int) (lastToast != null
        ? lastToast.Position.Y + lastToast.Height
        : 0);

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
  }
}
