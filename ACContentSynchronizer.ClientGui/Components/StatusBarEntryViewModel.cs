using System;
using ACContentSynchronizer.ClientGui.Models;
using ACContentSynchronizer.ClientGui.ViewModels;

namespace ACContentSynchronizer.ClientGui.Components {
  public class StatusBarEntryViewModel : ViewModelBase, IDisposable {
    public StatusBarEntryViewModel(TaskViewModel task) {
      Task = task;
      Task.Run();
    }

    internal TaskViewModel Task { get; set; }

    public void Dispose() {
      Task.Dispose();
    }
  }
}
