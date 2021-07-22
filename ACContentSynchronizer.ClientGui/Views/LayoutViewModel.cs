using System;
using System.Linq;
using ACContentSynchronizer.ClientGui.Models;
using ACContentSynchronizer.ClientGui.ViewModels;
using ACContentSynchronizer.ClientGui.Windows;
using Avalonia.Collections;
using Avalonia.Markup.Xaml.Styling;
using ReactiveUI;

namespace ACContentSynchronizer.ClientGui.Views {
  public class LayoutViewModel : ViewModelBase {
    private Themes _selectedTheme;
    private bool _serverSelected;

    public bool ServerSelected {
      get => _serverSelected;
      set => this.RaiseAndSetIfChanged(ref _serverSelected, value);
    }

    public AvaloniaList<Themes> AllowedThemes { get; set; } = new() {
      Themes.Default,
      Themes.Compact,
    };

    public Themes SelectedTheme {
      get => _selectedTheme;
      set {
        this.RaiseAndSetIfChanged(ref _selectedTheme, value);
        ChangeLayoutExecute(value);
      }
    }

    public void ChangeLayoutExecute(Themes theme) {
      var style = new Avalonia.Styling.Styles {
        new StyleInclude(new Uri("avares://ACContentSynchronizer.ClientGui/Styles")) {
          Source = new($"avares://ACContentSynchronizer.ClientGui/Styles/{theme}Theme.axaml"),
        },
      };

      if (MainWindow.Instance.Styles.Any()) {
        MainWindow.Instance.Styles[0] = style;
      } else {
        MainWindow.Instance.Styles.Add(style);
      }

      var settings = Settings.Instance;
      settings.Theme = theme;
      settings.Save();
    }

    public void GoBack() {
      ServerSelected = false;
      Sidebar.Instance.UnsetServer();
    }
  }
}
