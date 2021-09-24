using System;
using System.Collections.Generic;
using System.Linq;
using ACContentSynchronizer.ClientGui.Models;
using ACContentSynchronizer.Extensions;
using ReactiveUI;

namespace ACContentSynchronizer.ClientGui.Views.ServerViews {
  public partial class ServerSettingsViewModel {
    private bool _booking;

    private TimeSpan _bookingTime = TimeSpan.FromMinutes(10);

    private bool _extraLap;

    private short _from;

    private TimeSpan _initialDelay = TimeSpan.FromMinutes(1);

    private JoinTypes _joinType = JoinTypes.CloseAsStart;

    private short _lapsSession = 5;

    private bool _lockedEntryList;

    private bool _loopMode = true;

    private bool _pickupMode = true;

    private bool _practice = true;

    private bool _practiceCanJoin = true;

    private TimeSpan _practiceTime = TimeSpan.FromMinutes(10);

    private bool _qualification = true;

    private bool _qualificationCanJoin = true;

    private TimeSpan _qualificationTime = TimeSpan.FromMinutes(10);

    private short _qualifyLimit = 120;

    private bool _race = true;

    private RaceLimits _raceLimit = RaceLimits.Laps;

    private TimeSpan _raceOver = TimeSpan.FromMinutes(1);

    private TimeSpan _resultScreen = TimeSpan.FromMinutes(1);

    private short _reversedGrid;

    private TimeSpan _timeSession = TimeSpan.FromMinutes(10);

    private short _to;

    public bool PickupMode {
      get => _pickupMode;
      set => this.RaiseAndSetIfChanged(ref _pickupMode, value);
    }

    public bool LockedEntryList {
      get => _lockedEntryList;
      set => this.RaiseAndSetIfChanged(ref _lockedEntryList, value);
    }

    public bool LoopMode {
      get => _loopMode;
      set => this.RaiseAndSetIfChanged(ref _loopMode, value);
    }

    public bool Booking {
      get => _booking;
      set => this.RaiseAndSetIfChanged(ref _booking, value);
    }

    public TimeSpan BookingTime {
      get => _bookingTime;
      set => this.RaiseAndSetIfChanged(ref _bookingTime, value);
    }

    public bool Practice {
      get => _practice;
      set => this.RaiseAndSetIfChanged(ref _practice, value);
    }

    public TimeSpan PracticeTime {
      get => _practiceTime;
      set => this.RaiseAndSetIfChanged(ref _practiceTime, value);
    }

    public bool PracticeCanJoin {
      get => _practiceCanJoin;
      set => this.RaiseAndSetIfChanged(ref _practiceCanJoin, value);
    }

    public bool Qualification {
      get => _qualification;
      set => this.RaiseAndSetIfChanged(ref _qualification, value);
    }

    public TimeSpan QualificationTime {
      get => _qualificationTime;
      set => this.RaiseAndSetIfChanged(ref _qualificationTime, value);
    }

    public bool QualificationCanJoin {
      get => _qualificationCanJoin;
      set => this.RaiseAndSetIfChanged(ref _qualificationCanJoin, value);
    }

    public short QualifyLimit {
      get => _qualifyLimit;
      set => this.RaiseAndSetIfChanged(ref _qualifyLimit, value);
    }

    public bool Race {
      get => _race;
      set => this.RaiseAndSetIfChanged(ref _race, value);
    }

    public RaceLimits RaceLimit {
      get => _raceLimit;
      set {
        this.RaiseAndSetIfChanged(ref _raceLimit, value);
        this.RaisePropertyChanged(nameof(IsLimitByLaps));
      }
    }

    public short LapsSession {
      get => _lapsSession;
      set => this.RaiseAndSetIfChanged(ref _lapsSession, value);
    }

    public TimeSpan TimeSession {
      get => _timeSession;
      set => this.RaiseAndSetIfChanged(ref _timeSession, value);
    }

    public bool ExtraLap {
      get => _extraLap;
      set => this.RaiseAndSetIfChanged(ref _extraLap, value);
    }

    public TimeSpan InitialDelay {
      get => _initialDelay;
      set => this.RaiseAndSetIfChanged(ref _initialDelay, value);
    }

    public TimeSpan RaceOver {
      get => _raceOver;
      set => this.RaiseAndSetIfChanged(ref _raceOver, value);
    }

    public TimeSpan ResultScreen {
      get => _resultScreen;
      set => this.RaiseAndSetIfChanged(ref _resultScreen, value);
    }

    public JoinTypes JoinType {
      get => _joinType;
      set => this.RaiseAndSetIfChanged(ref _joinType, value);
    }

    public short From {
      get => _from;
      set => this.RaiseAndSetIfChanged(ref _from, value);
    }

    public short To {
      get => _to;
      set => this.RaiseAndSetIfChanged(ref _to, value);
    }

    public short ReversedGrid {
      get => _reversedGrid;
      set => this.RaiseAndSetIfChanged(ref _reversedGrid, value);
    }

    public static IEnumerable<JoinTypes> AvailableJoinTypes =>
      Enum.GetValues(typeof(JoinTypes)).Cast<JoinTypes>();

    public static IEnumerable<RaceLimits> AvailableRaceLimits =>
      Enum.GetValues(typeof(RaceLimits)).Cast<RaceLimits>();

    public bool IsLimitByLaps => RaceLimit == RaceLimits.Laps;
    public static TimeSpan TimeMin => TimeSpan.FromMinutes(1);
    public static TimeSpan TimeMax => TimeSpan.FromHours(1.5);
    public static short QualifyLimitMin => 100;
    public static short QualifyLimitMax => 200;
    public static short LapsMin => 1;
    public static short LapsMax => 120;
    public static TimeSpan InitialDelayMin => TimeSpan.FromSeconds(1);
    public static TimeSpan InitialDelayMax => TimeSpan.FromMinutes(2);
    public static TimeSpan RaceOverMin => TimeSpan.Zero;
    public static TimeSpan RaceOverMax => TimeSpan.FromMinutes(5);
    public static TimeSpan ResultScreenMin => TimeSpan.Zero;
    public static TimeSpan ResultScreenMax => TimeSpan.FromMinutes(2);

    public IniFile SaveSession(IniFile source) {
      source["SERVER"]["QUALIFY_MAX_WAIT_PERC"] = QualifyLimit;
      source["SERVER"]["RACE_PIT_WINDOW_START"] = From;
      source["SERVER"]["RACE_PIT_WINDOW_END"] = To;
      source["SERVER"]["REVERSED_GRID_RACE_POSITIONS"] = ReversedGrid;
      source["SERVER"]["LOCKED_ENTRY_LIST"] = LockedEntryList;
      source["SERVER"]["PICKUP_MODE_ENABLED"] = PickupMode.ToInt();
      source["SERVER"]["LOOP_MODE"] = PickupMode.ToInt();
      source["SERVER"]["RACE_OVER_TIME"] = RaceOver.Seconds;
      source["SERVER"]["RESULT_SCREEN_TIME"] = ResultScreen.TotalSeconds;
      source["SERVER"]["RACE_EXTRA_LAP"] = ExtraLap.ToInt();

      if (Booking) {
        source.Add("BOOKING", new() {
          ["NAME"] = "Booking",
          ["TIME"] = BookingTime.TotalMinutes,
        });
      }

      if (Practice) {
        source.Add("PRACTICE", new() {
          ["NAME"] = "Practice",
          ["TIME"] = PracticeTime.TotalMinutes,
          ["IS_OPEN"] = PracticeCanJoin.ToInt(),
        });
      }

      if (Qualification) {
        source.Add("QUALIFY", new() {
          ["NAME"] = "Qualify",
          ["TIME"] = QualificationTime.TotalMinutes,
          ["IS_OPEN"] = QualificationCanJoin.ToInt(),
        });
      }

      if (Race) {
        source.Add("RACE", new() {
          ["NAME"] = "Race",
          ["LAPS"] = LapsSession,
          ["TIME"] = TimeSession,
          ["WAIT_TIME"] = InitialDelay.TotalSeconds,
          ["IS_OPEN"] = (int) JoinType,
        });
      }

      return source;
    }
  }
}
