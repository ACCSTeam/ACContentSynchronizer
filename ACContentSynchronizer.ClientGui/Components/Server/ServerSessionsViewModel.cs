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

    private TimeSpan _initialDelay;

    private JoinTypes _joinType = JoinTypes.CloseAsStart;

    private short _laps;

    private bool _lockedEntryListInPickupMode;

    private bool _loopMode;

    private bool _mandatoryPits;

    private bool _pickupMode;

    private bool _practice;

    private bool _practiceCanJoin;

    private TimeSpan _practiceTime;

    private bool _qualification;

    private bool _qualificationCanJoin;

    private TimeSpan _qualificationTime;

    private double _qualifyLimit;

    private bool _race;

    private RaceLimits _raceLimit;

    private TimeSpan _raceOver;

    private TimeSpan _resultScreen;

    private short _reversedGrid;

    private TimeSpan _time;

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
      set => this.RaiseAndSetIfChanged(ref _raceLimit, value);
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
  }
}
