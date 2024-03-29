using System;
using System.Threading.Tasks;
using ACContentSynchronizer.ClientGui.ViewModels;
using ACContentSynchronizer.ClientGui.Windows;
using Avalonia.Controls;

namespace ACContentSynchronizer.ClientGui.Models {
  public class Modal : Window {
    protected Modal() {
    }

    public static async Task Open<T, TInput>(TInput? vm) where T : Modal, new()
      where TInput : ModalViewModel<T> {
      var modal = (T) Activator.CreateInstance(typeof(T), vm)!;
      await modal.ShowDialog(MainWindow.Instance);
    }

    public static async Task Open<T>() where T : Modal, new() {
      var modal = new T();
      await modal.ShowDialog(MainWindow.Instance);
    }

    public static async Task<TResult> Open<T, TResult>() where T : Modal, new() where TResult : new() {
      var modal = new T();
      return await modal.ShowDialog<TResult>(MainWindow.Instance);
    }
  }
}
