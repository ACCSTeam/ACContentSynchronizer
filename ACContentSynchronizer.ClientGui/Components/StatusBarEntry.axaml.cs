using System;
using ACContentSynchronizer.ClientGui.ViewModels;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace ACContentSynchronizer.ClientGui.Components {
  public class StatusBarEntry : DisposableControl {
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

    private void Cancel(object? sender, RoutedEventArgs e) {
      ViewModel?.Task.Cancel();
      Source?.Remove(this);
    }
  }
}
