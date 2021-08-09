using System;
using System.Collections.Generic;
using System.Linq;
using ACContentSynchronizer.ClientGui.Models;
using ACContentSynchronizer.ClientGui.ViewModels;
using ReactiveUI;

namespace ACContentSynchronizer.ClientGui.Components.Server {
  public class ServerRulesViewModel : ViewModelBase {
    private AssistMode _absMode;

    private short _allowedTyresOut = 2;

    private bool _automaticClutch;

    private short _damageRate = 100;

    private bool _disableGasCut;

    private short _fuelRate = 100;

    private bool _kickUntilRestart = true;

    private short _kickVoteQuorum = 85;

    private short _maxBallast;

    private short _maxContactsPerKm = -1;

    private short _sessionVoteQuorum = 80;

    private SpawnType _spawnType = SpawnType.Pits;

    private bool _stabilityControl;

    private AssistMode _tcMode;

    private bool _tyreBlankets;

    private short _tyresWearRate = 100;

    private bool _virtualMirror;

    private TimeSpan _voteDuration = TimeSpan.FromSeconds(20);

    public IEnumerable<AssistMode> AvailableAssistModes =>
      Enum.GetValues(typeof(AssistMode)).Cast<AssistMode>();
    public IEnumerable<SpawnType> AvailableSpawnTypes =>
      Enum.GetValues(typeof(SpawnType)).Cast<SpawnType>();

    public SpawnType SpawnType {
      get => _spawnType;
      set => this.RaiseAndSetIfChanged(ref _spawnType, value);
    }

    public AssistMode AbsMode {
      get => _absMode;
      set => this.RaiseAndSetIfChanged(ref _absMode, value);
    }

    public AssistMode TcMode {
      get => _tcMode;
      set => this.RaiseAndSetIfChanged(ref _tcMode, value);
    }

    public bool StabilityControl {
      get => _stabilityControl;
      set => this.RaiseAndSetIfChanged(ref _stabilityControl, value);
    }

    public bool AutomaticClutch {
      get => _automaticClutch;
      set => this.RaiseAndSetIfChanged(ref _automaticClutch, value);
    }

    public bool TyreBlankets {
      get => _tyreBlankets;
      set => this.RaiseAndSetIfChanged(ref _tyreBlankets, value);
    }

    public bool VirtualMirror {
      get => _virtualMirror;
      set => this.RaiseAndSetIfChanged(ref _virtualMirror, value);
    }

    public bool KickUntilRestart {
      get => _kickUntilRestart;
      set => this.RaiseAndSetIfChanged(ref _kickUntilRestart, value);
    }

    public bool DisableGasCut {
      get => _disableGasCut;
      set => this.RaiseAndSetIfChanged(ref _disableGasCut, value);
    }

    public short KickVoteQuorum {
      get => _kickVoteQuorum;
      set => this.RaiseAndSetIfChanged(ref _kickVoteQuorum, value);
    }

    public short SessionVoteQuorum {
      get => _sessionVoteQuorum;
      set => this.RaiseAndSetIfChanged(ref _sessionVoteQuorum, value);
    }

    public TimeSpan VoteDuration {
      get => _voteDuration;
      set => this.RaiseAndSetIfChanged(ref _voteDuration, value);
    }

    public short MaxContactsPerKm {
      get => _maxContactsPerKm;
      set => this.RaiseAndSetIfChanged(ref _maxContactsPerKm, value);
    }

    public short FuelRate {
      get => _fuelRate;
      set => this.RaiseAndSetIfChanged(ref _fuelRate, value);
    }

    public short DamageRate {
      get => _damageRate;
      set => this.RaiseAndSetIfChanged(ref _damageRate, value);
    }

    public short TyresWearRate {
      get => _tyresWearRate;
      set => this.RaiseAndSetIfChanged(ref _tyresWearRate, value);
    }

    public short AllowedTyresOut {
      get => _allowedTyresOut;
      set => this.RaiseAndSetIfChanged(ref _allowedTyresOut, value);
    }

    public short MaxBallast {
      get => _maxBallast;
      set => this.RaiseAndSetIfChanged(ref _maxBallast, value);
    }

    public int VoteQuorumMinimum => 40;
    public int VoteQuorumMaximum => 90;
    public int MaxContactsPerKmMinimum => -1;
    public int MaxContactsPerKmMaximum => 9;
    public int RealismMinimum => 0;
    public int RealismMaximum => 200;
    public int AllowedTyresOutMinimum => -1;
    public int AllowedTyresOutMaximum => 3;
    public TimeSpan VoteDurationMin => TimeSpan.FromSeconds(10);
    public TimeSpan VoteDurationMax => TimeSpan.FromMinutes(1);
  }
}
