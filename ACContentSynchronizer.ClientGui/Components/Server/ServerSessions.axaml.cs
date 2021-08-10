using ACContentSynchronizer.Extensions;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ACContentSynchronizer.ClientGui.Components.Server {
  public class ServerSessions : UserControl {
    private static ServerSessions? _instance;
    private readonly ServerSessionsViewModel _vm;

    public ServerSessions() {
      DataContext = _vm = new();
      InitializeComponent();
    }

    public static ServerSessions Instance => _instance ??= new();
    public static ServerSessionsViewModel ViewModel => Instance._vm;

    private void InitializeComponent() {
      AvaloniaXamlLoader.Load(this);
    }

    public void Load(IniFile serverConfig) {
      throw new System.NotImplementedException();
    }

    public IniFile Save(IniFile source) {
      source["SERVER"]["QUALIFY_MAX_WAIT_PERC"] = _vm.QualifyLimit;
      source["SERVER"]["RACE_PIT_WINDOW_START"] = _vm.From;
      source["SERVER"]["RACE_PIT_WINDOW_END"] = _vm.To;
      source["SERVER"]["REVERSED_GRID_RACE_POSITIONS"] = _vm.ReversedGrid;
      source["SERVER"]["LOCKED_ENTRY_LIST"] = _vm.LockedEntryList;
      source["SERVER"]["PICKUP_MODE_ENABLED"] = _vm.PickupMode.ToInt();
      source["SERVER"]["LOOP_MODE"] = _vm.PickupMode.ToInt();
      source["SERVER"]["RACE_OVER_TIME"] = _vm.RaceOver.Seconds;
      source["SERVER"]["RESULT_SCREEN_TIME"] = _vm.ResultScreen.TotalSeconds;
      source["SERVER"]["RACE_EXTRA_LAP"] = _vm.ExtraLap.ToInt();

      if (_vm.Booking) {
        source.Add("BOOKING", new() {
          ["NAME"] = "Booking",
          ["TIME"] = _vm.BookingTime.TotalMinutes,
        });
      }

      if (_vm.Practice) {
        source.Add("PRACTICE", new() {
          ["NAME"] = "Practice",
          ["TIME"] = _vm.PracticeTime.TotalMinutes,
          ["IS_OPEN"] = _vm.PracticeCanJoin.ToInt(),
        });
      }

      if (_vm.Qualification) {
        source.Add("QUALIFY", new() {
          ["NAME"] = "Qualify",
          ["TIME"] = _vm.QualificationTime.TotalMinutes,
          ["IS_OPEN"] = _vm.QualificationCanJoin.ToInt(),
        });
      }

      if (_vm.Race) {
        source.Add("RACE", new() {
          ["NAME"] = "Race",
          ["LAPS"] = _vm.Laps,
          ["TIME"] = _vm.Time,
          ["WAIT_TIME"] = _vm.InitialDelay.TotalSeconds,
          ["IS_OPEN"] = (int) _vm.JoinType,
        });
      }

      return source;
    }
  }
}
