using System.Linq;
using ACContentSynchronizer.ClientGui.Components;
using ACContentSynchronizer.ClientGui.Models;
using ACContentSynchronizer.ClientGui.ViewModels;
using Avalonia.Collections;
using ReactiveUI;

namespace ACContentSynchronizer.ClientGui.Views {
  public class StatusBarViewModel : ViewModelBase {
    private double _progress;

    private string _state = "";

    public StatusBarViewModel() {
      Tasks.CollectionChanged += (_, args) => {
        var enumerable = args.NewItems?.Cast<StatusBarEntry>();
        if (enumerable == null) {
          return;
        }

        foreach (var statusBarEntry in enumerable) {
          if (statusBarEntry is { ViewModel: { } }) {
            statusBarEntry.ViewModel.Task.PropertyChanged += (_, _) => {
              var count = Tasks.Count(x => x.ViewModel != null && x.ViewModel.Task.Progress > 0);
              var progress = Tasks.Sum(x => x.ViewModel?.Task.Progress ?? 0);

              Progress = progress > 0 && count > 0
                ? progress / count
                : 0;

              State = statusBarEntry.ViewModel.Task.State;
            };
          }
        }
      };
    }

    public AvaloniaList<StatusBarEntry> Tasks { get; set; } = new();

    public double Progress {
      get => _progress;
      set => this.RaiseAndSetIfChanged(ref _progress, value);
    }

    public string State {
      get => _state;
      set => this.RaiseAndSetIfChanged(ref _state, value);
    }

    public void AddTask(TaskViewModel task) {
      var entry = new StatusBarEntry();
      entry.Run(task, Tasks);
      Tasks.Add(entry);
    }
  }
}
