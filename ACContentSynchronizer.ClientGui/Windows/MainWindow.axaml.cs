using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using ACContentSynchronizer.ClientGui.Modals;
using ACContentSynchronizer.ClientGui.Models;
using ACContentSynchronizer.ClientGui.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;
using Avalonia.Threading;
using ReactiveUI;

namespace ACContentSynchronizer.ClientGui.Windows {
  public class MainWindow : Window {
    private static MainWindow? _instance;

    public MainWindow() {
      InitializeComponent();
#if DEBUG
      this.AttachDevTools();
#endif
    }

    public static MainWindow Instance => _instance ??= new();

    private void InitializeComponent() {
      AvaloniaXamlLoader.Load(this);

      TransparencyLevelHint = RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
        ? WindowTransparencyLevel.Transparent
        : WindowTransparencyLevel.AcrylicBlur;

      this.GetObservable(IsActiveProperty).Subscribe(Console.WriteLine);
    }

    public void DoShowDialogAsync<T, TInput>(TInput? vm = null)
      where T : Modal, new()
      where TInput : ModalViewModel, new() {
      ReactiveCommand.CreateFromTask(async () => {
          var modal = (T?) Activator.CreateInstance(typeof(T));
          vm ??= new();

          if (modal == null) {
            return;
          }

          modal.DataContext = vm;
          vm.CloseRequest += modal.Close;
          await modal.ShowDialog(this);
          vm.CloseRequest -= modal.Close;
        })
        .Execute();
    }
  }
}
