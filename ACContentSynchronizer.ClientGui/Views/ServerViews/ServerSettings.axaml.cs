using System;
using System.Threading.Tasks;
using ACContentSynchronizer.ClientGui.Components;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ReactiveUI;

namespace ACContentSynchronizer.ClientGui.Views.ServerViews {
  public class ServerSettings : DisposableControl {
    public ServerSettings() {
      InitializeComponent();
    }

    public ServerSettings(ServerSettingsViewModel vm) {
      DataContext = vm;
      InitializeComponent();
    }

    private void InitializeComponent() {
      AvaloniaXamlLoader.Load(this);

      this.WhenAnyValue(settings => settings.DataContext).Subscribe(context => {
        if (context == null) {
          return;
        }

        ReactiveCommand.CreateFromTask(() => {
          var serverTabs = this.FindControl<TabControl>("SettingsTabs");
          serverTabs.Items = new TabItem[] {
            new() {
              Header = "Main",
              Content = InitTab<ServerMain>(context),
            },
            new() {
              Header = "Rules",
              Content = InitTab<ServerRules>(context),
            },
            new() {
              Header = "Conditions",
              Content = InitTab<ServerConditions>(context),
            },
            new() {
              Header = "Sessions",
              Content = InitTab<ServerSessions>(context),
            },
          };

          return Task.CompletedTask;
        }).Execute();
      });
    }

    private T InitTab<T>(object dataContext)
      where T : UserControl, new() {
      return new() {
        DataContext = dataContext,
      };
    }
  }
}
