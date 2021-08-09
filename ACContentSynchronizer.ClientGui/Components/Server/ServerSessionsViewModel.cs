using System;
using System.Collections.Generic;
using System.Linq;
using ACContentSynchronizer.ClientGui.Models;
using ACContentSynchronizer.ClientGui.ViewModels;
using ReactiveUI;

namespace ACContentSynchronizer.ClientGui.Components.Server {
  public class ServerSessionsViewModel : ViewModelBase {
    private bool _booking;

    private TimeSpan _bookingTime = TimeSpan.FromMinutes(10);

    private bool _extraLap;

    private short _from;

    private TimeSpan _initialDelay = TimeSpan.FromMinutes(1);

    private JoinTypes _joinType = JoinTypes.CloseAsStart;

    private short _laps = 5;

    private bool _lockedEntryListInPickupMode;

    private bool _loopMode = true;

    private bool _mandatoryPits;

    private bool _pickupMode = true;

    private bool _practice = true;

    private bool _practiceCanJoin = true;

    private TimeSpan _practiceTime = TimeSpan.FromMinutes(10);

    private bool _qualification = true;

    private bool _qualificationCanJoin = true;

    private TimeSpan _qualificationTime = TimeSpan.FromMinutes(10);

    private double _qualifyLimit = 120;

    private bool _race = true;

    private RaceLimits _raceLimit = RaceLimits.Laps;

    private TimeSpan _raceOver = TimeSpan.FromMinutes(1);

    private TimeSpan _resultScreen = TimeSpan.FromMinutes(1);

    private short _reversedGrid;

    private TimeSpan _time = TimeSpan.FromMinutes(10);

    private short _to;

    public bool PickupMode {
      get => _pickupMode;
      set => this.RaiseAndSetIfChanged(ref _pickupMode, value);
    }

    public bool LockedEntryListInPickupMode {
      get => _lockedEntryListInPickupMode;
      set => this.RaiseAndSetIfChanged(ref _lockedEntryListInPickupMode, value);
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

    public double QualifyLimit {
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

    public short Laps {
      get => _laps;
      set => this.RaiseAndSetIfChanged(ref _laps, value);
    }

    public TimeSpan Time {
      get => _time;
      set => this.RaiseAndSetIfChanged(ref _time, value);
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

    public bool MandatoryPits {
      get => _mandatoryPits;
      set => this.RaiseAndSetIfChanged(ref _mandatoryPits, value);
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

    public IEnumerable<JoinTypes> AvailableJoinTypes =>
      Enum.GetValues(typeof(JoinTypes)).Cast<JoinTypes>();

    public IEnumerable<RaceLimits> AvailableRaceLimits =>
      Enum.GetValues(typeof(RaceLimits)).Cast<RaceLimits>();

    public bool IsLimitByLaps => RaceLimit == RaceLimits.Laps;
    public TimeSpan TimeMin=>TimeSpan.FromMinutes(1);
    public TimeSpan TimeMax=>TimeSpan.FromHours(1.5);
    public short QualifyLimitMin => 100;
    public short QualifyLimitMax => 200;
    public short LapsMin => 1;
    public short LapsMax => 120;
    public TimeSpan InitialDelayMin => TimeSpan.FromSeconds(1);
    public TimeSpan InitialDelayMax => TimeSpan.FromMinutes(2);
    public TimeSpan RaceOverMin => TimeSpan.Zero;
    public TimeSpan RaceOverMax => TimeSpan.FromMinutes(5);
    public TimeSpan ResultScreenMin => TimeSpan.Zero;
    public TimeSpan ResultScreenMax => TimeSpan.FromMinutes(2);
  }
}
