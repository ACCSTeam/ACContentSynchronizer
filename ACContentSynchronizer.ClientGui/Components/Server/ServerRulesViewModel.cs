using System;
using System.Linq;
using ACContentSynchronizer.ClientGui.Models;
using ACContentSynchronizer.ClientGui.ViewModels;
using Avalonia.Collections;
using ReactiveUI;

namespace ACContentSynchronizer.ClientGui.Components.Server {
  public class ServerRulesViewModel : ViewModelBase {
    private AssistMode _absMode;

    private AssistMode _tcMode;

    private bool _stabilityControl;

    private bool _automaticClutch;

    private bool _tyreBlankets;

    private bool _virtualMirror;

    private short _kickVoteQuorum = 85;

    private short _sessionVoteQuorum = 80;

    private TimeSpan _voteDuration = TimeSpan.FromSeconds(20);

    private bool _kickUntilRestart = true;

    private short _maxContactsPerKm = -1;

    private short _fuelRate = 100;

    private short _damageRate = 100;

    private short _tyresWearRate = 100;

    private short _allowedTyresOut = 2;

    private short _maxBallast;

    private SpawnType _spawnType = SpawnType.Pits;

    private bool _disableGasCut;

    public AvaloniaList<AssistMode> AvailableAssistModes =>
      new(Enum.GetValues(typeof(AssistMode)).Cast<AssistMode>());

    public AvaloniaList<SpawnType> AvailableSpawnTypes =>
      new(Enum.GetValues(typeof(SpawnType)).Cast<SpawnType>());

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
      set {
        this.RaiseAndSetIfChanged(ref _kickVoteQuorum, value);
        this.RaisePropertyChanged(nameof(KickVoteQuorumLabel));
      }
    }

    public string KickVoteQuorumLabel => $"Kick vote quorum: {KickVoteQuorum}%";

    public short SessionVoteQuorum {
      get => _sessionVoteQuorum;
      set {
        this.RaiseAndSetIfChanged(ref _sessionVoteQuorum, value);
        this.RaisePropertyChanged(nameof(SessionVoteQuorumLabel));
      }
    }

    public string SessionVoteQuorumLabel => $"Session vote quorum: {SessionVoteQuorum}%";

    public TimeSpan VoteDuration {
      get => _voteDuration;
      set {
        this.RaiseAndSetIfChanged(ref _voteDuration, value);
        this.RaisePropertyChanged(nameof(VoteDurationLabel));
      }
    }

    public string VoteDurationLabel => $"Vote duration: {VoteDuration}";

    public short MaxContactsPerKm {
      get => _maxContactsPerKm;
      set {
        this.RaiseAndSetIfChanged(ref _maxContactsPerKm, value);
        this.RaisePropertyChanged(nameof(MaxContactsPerKmLabel));
      }
    }

    public string MaxContactsPerKmLabel => $"Max contacts per km: {MaxContactsPerKm}";

    public short FuelRate {
      get => _fuelRate;
      set {
        this.RaiseAndSetIfChanged(ref _fuelRate, value);
        this.RaisePropertyChanged(nameof(FuelRateLabel));
      }
    }

    public string FuelRateLabel => $"Fuel rate: {FuelRate}%";

    public short DamageRate {
      get => _damageRate;
      set {
        this.RaiseAndSetIfChanged(ref _damageRate, value);
        this.RaisePropertyChanged(nameof(DamageRateLabel));
      }
    }

    public string DamageRateLabel => $"Damage rate: {DamageRate}%";

    public short TyresWearRate {
      get => _tyresWearRate;
      set {
        this.RaiseAndSetIfChanged(ref _tyresWearRate, value);
        this.RaisePropertyChanged(nameof(TyresWearRateLabel));
      }
    }

    public string TyresWearRateLabel => $"Tyres wear rate: {TyresWearRate}%";

    public short AllowedTyresOut {
      get => _allowedTyresOut;
      set {
        this.RaiseAndSetIfChanged(ref _allowedTyresOut, value);
        this.RaisePropertyChanged(nameof(AllowedTyresOutLabel));
      }
    }

    public string AllowedTyresOutLabel => $"Allowed tyres out: {AllowedTyresOut} tyres";

    public short MaxBallast {
      get => _maxBallast;
      set {
        this.RaiseAndSetIfChanged(ref _maxBallast, value);
        this.RaisePropertyChanged(nameof(MaxBallastLabel));
      }
    }

    public string MaxBallastLabel => $"Max ballast: {MaxBallast} kg";
    public int VoteQuorumMinimum => 40;
    public int VoteQuorumMaximum => 90;
    public int MaxContactsPerKmMinimum => -1;
    public int MaxContactsPerKmMaximum => 9;
    public int RealismMinimum => 0;
    public int RealismMaximum => 200;
    public int AllowedTyresOutMinimum => -1;
    public int AllowedTyresOutMaximum => 3;
  }
}
