using System.Linq;
using ACContentSynchronizer.ClientGui.Components;
using ACContentSynchronizer.ClientGui.ViewModels;
using Avalonia.Collections;
using ReactiveUI;
using Splat;

namespace ACContentSynchronizer.ClientGui.Views {
  public class StatusBarViewModel : ViewModelBase {
    private double _progress;

    private string _state = "";

    public StatusBarViewModel() {
      Application = Locator.Current.GetService<ApplicationViewModel>();

      Application.Tasks.CollectionChanged += (entry, _) => {
        if (entry is not AvaloniaList<StatusBarEntry> entries) {
          return;
        }

        if (!entries.Any()) {
          Progress = 0;
          State = "";
          return;
        }

        var entriesWithTask = entries.Where(statusBarEntry => statusBarEntry is { ViewModel: { } });
        foreach (var statusBarEntry in entriesWithTask) {
          statusBarEntry.ViewModel!.Task.PropertyChanged += (_, _) => {
            var tasks = Application.Tasks.Where(x => !(x.ViewModel?.Task.Worker.IsCompleted ?? true))
              .ToList();

            var count = tasks.Count(x => x.ViewModel != null && x.ViewModel.Task.Progress > 0);
            var progress = tasks.Sum(x => x.ViewModel?.Task.Progress ?? 0);

            Progress = progress > 0 && count > 0
              ? progress / count
              : 0;

            State = statusBarEntry.ViewModel.Task.Message;
          };
        }
      };
    }

    public ApplicationViewModel Application { get; set; }

    public double Progress {
      get => _progress;
      set => this.RaiseAndSetIfChanged(ref _progress, value);
    }

    public string State {
      get => _state;
      set => this.RaiseAndSetIfChanged(ref _state, value);
    }
  }
}
