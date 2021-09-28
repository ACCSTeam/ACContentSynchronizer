using System;
using System.Threading.Tasks;
using ACContentSynchronizer.ClientGui.ViewModels;
using ACContentSynchronizer.ClientGui.Windows;
using Avalonia.Controls;

namespace ACContentSynchronizer.ClientGui.Models {
  public class Modal : Window {
    protected Modal() {
    }

    public static async Task Open<T, TInput>(TInput? vm = null) where T : Modal, new()
      where TInput : ModalViewModel, new() {
      vm ??= new();

      var modal = (T?) Activator.CreateInstance(typeof(T));

      if (modal != null) {
        modal.DataContext = vm;
        vm.CloseRequest += () => modal.Close();

        await modal.ShowDialog(MainWindow.Instance);
      }
    }

    public static async Task Open<T>() where T : Modal, new() {
      var modal = new T();
      await modal.ShowDialog(MainWindow.Instance);
    }
  }
}
