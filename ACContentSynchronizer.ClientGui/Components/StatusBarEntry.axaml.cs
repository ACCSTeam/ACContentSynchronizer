using System;
using ACContentSynchronizer.ClientGui.ViewModels;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace ACContentSynchronizer.ClientGui.Components {
  public class StatusBarEntry : UserControl, IDisposable {
    public StatusBarEntry() {
      InitializeComponent();
    }

    public StatusBarEntryViewModel? ViewModel { get; private set; }

    private AvaloniaList<StatusBarEntry>? Source { get; set; }

    private void InitializeComponent() {
      AvaloniaXamlLoader.Load(this);
    }

    public void Run(TaskViewModel task, AvaloniaList<StatusBarEntry> source) {
      DataContext = ViewModel = new(task);
      Source = source;
    }

    private async void Cancel(object? sender, RoutedEventArgs e) {
      if (ViewModel != null) {
        ViewModel.Task.Cancel();
        await ViewModel.Task.Worker;
      }
      Source?.Remove(this);
    }

    public void Dispose() {
      ViewModel?.Dispose();
    }
  }
}
