using System;
using System.Linq;
using ACContentSynchronizer.ClientGui.Models;
using ACContentSynchronizer.ClientGui.ViewModels;
using Avalonia.Collections;
using ReactiveUI;

namespace ACContentSynchronizer.ClientGui.Components.Server {
  public class ServerRulesViewModel : ViewModelBase {
    public AvaloniaList<AssistMode> AvailableAssistModes =>
      new(Enum.GetValues(typeof(AssistMode)).Cast<AssistMode>());

    public AvaloniaList<SpawnType> AvailableSpawnTypes =>
      new(Enum.GetValues(typeof(SpawnType)).Cast<SpawnType>());

    private SpawnType _spawnType;

    public SpawnType SpawnType {
      get => _spawnType;
      set => this.RaiseAndSetIfChanged(ref _spawnType, value);
    }


    private AssistMode _absMode;

    public AssistMode AbsMode {
      get => _absMode;
      set => this.RaiseAndSetIfChanged(ref _absMode, value);
    }

    private AssistMode _tcMode;

    public AssistMode TcMode {
      get => _tcMode;
      set => this.RaiseAndSetIfChanged(ref _tcMode, value);
    }

    private bool _stabilityControl;

    public bool StabilityControl {
      get => _stabilityControl;
      set => this.RaiseAndSetIfChanged(ref _stabilityControl, value);
    }

    private bool _automaticClutch;

    public bool AutomaticClutch {
      get => _automaticClutch;
      set => this.RaiseAndSetIfChanged(ref _automaticClutch, value);
    }

    private bool _tyreBlankets;

    public bool TyreBlankets {
      get => _tyreBlankets;
      set => this.RaiseAndSetIfChanged(ref _tyreBlankets, value);
    }

    private bool _virtualMirror;

    public bool VirtualMirror {
      get => _virtualMirror;
      set => this.RaiseAndSetIfChanged(ref _virtualMirror, value);
    }

    private short _kickVoteQuorum;

    public short KickVoteQuorum {
      get => _kickVoteQuorum;
      set => this.RaiseAndSetIfChanged(ref _kickVoteQuorum, value);
    }

    private short _sessionVoteQuorum;

    public short SessionVoteQuorum {
      get => _sessionVoteQuorum;
      set => this.RaiseAndSetIfChanged(ref _sessionVoteQuorum, value);
    }

    private short _voteDuration;

    public short VoteDuration {
      get => _voteDuration;
      set => this.RaiseAndSetIfChanged(ref _voteDuration, value);
    }

    private bool _kickUntilRestart;

    public bool KickUntilRestart {
      get => _kickUntilRestart;
      set => this.RaiseAndSetIfChanged(ref _kickUntilRestart, value);
    }

    private short _maxContacts;

    public short MaxContacts {
      get => _maxContacts;
      set => this.RaiseAndSetIfChanged(ref _maxContacts, value);
    }

    private short _fuelRate;

    public short FuelRate {
      get => _fuelRate;
      set => this.RaiseAndSetIfChanged(ref _fuelRate, value);
    }

    private short _damageRate;

    public short DamageRate {
      get => _damageRate;
      set => this.RaiseAndSetIfChanged(ref _damageRate, value);
    }

    private short _tyresWearRate;

    public short TyresWearRate {
      get => _tyresWearRate;
      set => this.RaiseAndSetIfChanged(ref _tyresWearRate, value);
    }

    private short _tyresOut;

    public short TyresOut {
      get => _tyresOut;
      set => this.RaiseAndSetIfChanged(ref _tyresOut, value);
    }

    private short _maxBallast;

    public short MaxBallast {
      get => _maxBallast;
      set => this.RaiseAndSetIfChanged(ref _maxBallast, value);
    }

    private bool _disableGasCut;

    public bool DisableGasCut {
      get => _disableGasCut;
      set => this.RaiseAndSetIfChanged(ref _disableGasCut, value);
    }

  }
}
