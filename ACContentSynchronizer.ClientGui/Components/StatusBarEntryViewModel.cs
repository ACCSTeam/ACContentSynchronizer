using System;
using ACContentSynchronizer.ClientGui.ViewModels;
using ReactiveUI;

namespace ACContentSynchronizer.ClientGui.Components {
  public class StatusBarEntryViewModel : ViewModelBase, IDisposable {
    public StatusBarEntryViewModel(TaskViewModel task) {
      Task = task;
      ReactiveCommand.CreateFromTask(async () => {
        await task.Initialize();
      }).Execute();
      Task.Run();
    }

    public StatusBarEntryViewModel() {
    }

    internal TaskViewModel Task { get; set; } = null!;

    public void Dispose() {
      Task.Dispose();
    }
  }
}
