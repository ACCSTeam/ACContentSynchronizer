using System;
using ACContentSynchronizer.ClientGui.Components;
using ACContentSynchronizer.ClientGui.ViewModels;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ACContentSynchronizer.ClientGui.Views.ServerViews {
  public class Race : UserControl, IDisposable {
    public Race() {
      InitializeComponent();
    }

    public Race(RaceViewModel vm) {
      DataContext = vm;
      InitializeComponent();
    }

    private void InitializeComponent() {
      AvaloniaXamlLoader.Load(this);
    }

    public void Dispose() {
      if (DataContext is IDisposable disposable) {
        disposable.Dispose();
      }

      var carPreview = this.FindControl<Preview>("CarPreview");
      carPreview?.Dispose();

      var trackPreview = this.FindControl<Preview>("TrackPreview");
      trackPreview?.Dispose();
    }
  }
}
