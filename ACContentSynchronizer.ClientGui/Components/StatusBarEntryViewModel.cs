using System;
using ACContentSynchronizer.ClientGui.ViewModels;

namespace ACContentSynchronizer.ClientGui.Components {
  public class StatusBarEntryViewModel : ViewModelBase, IDisposable {
    public StatusBarEntryViewModel(TaskViewModel task) {
      Task = task;
      task.Worker = Task.Run();
    }

    public StatusBarEntryViewModel() {
    }

    internal TaskViewModel Task { get; set; } = null!;

    public void Dispose() {
      Task.Dispose();
    }
  }
}
