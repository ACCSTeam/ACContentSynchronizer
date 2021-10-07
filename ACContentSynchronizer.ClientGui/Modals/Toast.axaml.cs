﻿using System;
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
    private const int ToastWidth = 300;
    private const int ActiveToastCount = 3;

    public Toast() {
      DataContext = new ToastViewModel();
      InitializeComponent();
    }

    private static List<Toast> ToastsActivated { get; } = new();

    private void InitializeComponent() {
      AvaloniaXamlLoader.Load(this);

      Task.Run(async () => {
        await Task.Delay(TimeSpan.FromSeconds(10));
        await Dispatcher.UIThread.InvokeAsync(CloseInternal);
      });
    }

    public static void Open(string message) {
      try {
        var toast = new Toast {
          DataContext = new ToastViewModel(message),
          Width = ToastWidth,
        };

        if (ToastsActivated.Count > ActiveToastCount) {
          Console.WriteLine("avoided");
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
        Console.WriteLine("showed");
      } catch (Exception ex) {
        Console.WriteLine(ex);
      }
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
