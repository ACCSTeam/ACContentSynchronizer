using System;
using ACContentSynchronizer.ClientGui.Models;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace ACContentSynchronizer.ClientGui.Components {
  public class StatusBarEntry : UserControl, IDisposable {
    private StatusBarEntryViewModel? _vm;

    public StatusBarEntry() {
      InitializeComponent();
    }

    public AvaloniaList<StatusBarEntry>? Source { get; set; }

    public void Dispose() {
      _vm?.Dispose();
    }

    private void InitializeComponent() {
      AvaloniaXamlLoader.Load(this);
    }

    public void Run(TaskViewModel task, AvaloniaList<StatusBarEntry> source) {
      DataContext = _vm = new(task);
      Source = source;
    }

    private void Cancel(object? sender, RoutedEventArgs e) {
      _vm?.Task.Cancel();
      Source?.Remove(this);
    }
  }
}
