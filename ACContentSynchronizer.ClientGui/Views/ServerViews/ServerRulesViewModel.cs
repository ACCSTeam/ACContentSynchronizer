using System;
using System.Collections.Generic;
using System.Linq;
using ACContentSynchronizer.ClientGui.Models;
using ACContentSynchronizer.Extensions;
using ReactiveUI;

namespace ACContentSynchronizer.ClientGui.Views.ServerViews {
  public partial class ServerSettingsViewModel {
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

    public static int VoteQuorumMinimum => 40;
    public static int VoteQuorumMaximum => 90;
    public static int MaxContactsPerKmMinimum => -1;
    public static int MaxContactsPerKmMaximum => 9;
    public static int RealismMinimum => 0;
    public static int RealismMaximum => 200;
    public static int AllowedTyresOutMinimum => -1;
    public static int AllowedTyresOutMaximum => 3;
    public static TimeSpan VoteDurationMin => TimeSpan.FromSeconds(10);
    public static TimeSpan VoteDurationMax => TimeSpan.FromMinutes(1);

    public IniFile SaveRules(IniFile source) {
      source["SERVER"]["MAX_BALLAST_KG"] = MaxBallast;
      source["SERVER"]["KICK_QUORUM"] = KickVoteQuorum;
      source["SERVER"]["VOTING_QUORUM"] = SessionVoteQuorum;
      source["SERVER"]["VOTE_DURATION"] = VoteDuration.TotalSeconds;
      source["SERVER"]["FUEL_RATE"] = FuelRate;
      source["SERVER"]["DAMAGE_MULTIPLIER"] = DamageRate;
      source["SERVER"]["TYRE_WEAR_RATE"] = TyresWearRate;
      source["SERVER"]["ALLOWED_TYRES_OUT"] = AllowedTyresOut;
      source["SERVER"]["RACE_GAS_PENALTY_DISABLED"] = DisableGasCut.ToInt();
      source["SERVER"]["ABS_ALLOWED"] = (int) AbsMode;
      source["SERVER"]["TC_ALLOWED"] = (int) TcMode;
      source["SERVER"]["START_RULE"] = (int) SpawnType;
      source["SERVER"]["RACE_GAS_PENALTY_DISABLED"] = DisableGasCut.ToInt();
      source["SERVER"]["MAX_CONTACTS_PER_KM"] = MaxContactsPerKm;
      source["SERVER"]["STABILITY_ALLOWED"] = StabilityControl.ToInt();
      source["SERVER"]["AUTOCLUTCH_ALLOWED"] = AutomaticClutch;
      source["SERVER"]["TYRE_BLANKETS_ALLOWED"] = TyreBlankets.ToInt();
      source["SERVER"]["FORCE_VIRTUAL_MIRROR"] = VirtualMirror;

      return source;
    }

    public void LoadRules(IniFile source) {
      MaxBallast = source.V("SERVER", "MAX_BALLAST_KG", MaxBallast);
      KickVoteQuorum = source.V("SERVER", "KICK_QUORUM", KickVoteQuorum);
      SessionVoteQuorum = source.V("SERVER", "VOTING_QUORUM", SessionVoteQuorum);
      VoteDuration = TimeSpan.FromSeconds(source.V("SERVER", "VOTE_DURATION", VoteDuration.TotalSeconds));
      FuelRate = source.V("SERVER", "FUEL_RATE", FuelRate);
      DamageRate = source.V("SERVER", "DAMAGE_MULTIPLIER", DamageRate);
      TyresWearRate = source.V("SERVER", "TYRE_WEAR_RATE", TyresWearRate);
      AllowedTyresOut = source.V("SERVER", "ALLOWED_TYRES_OUT", AllowedTyresOut);
      AbsMode = source.V("SERVER", "ABS_ALLOWED", AbsMode);
      TcMode = source.V("SERVER", "TC_ALLOWED", TcMode);
      SpawnType = source.V("SERVER", "START_RULE", SpawnType);
      DisableGasCut = source.V("SERVER", "RACE_GAS_PENALTY_DISABLED", DisableGasCut);
      MaxContactsPerKm = source.V("SERVER", "MAX_CONTACTS_PER_KM", MaxContactsPerKm);
      StabilityControl = source.V("SERVER", "STABILITY_ALLOWED", StabilityControl);
      AutomaticClutch = source.V("SERVER", "AUTOCLUTCH_ALLOWED", AutomaticClutch);
      TyreBlankets = source.V("SERVER", "TYRE_BLANKETS_ALLOWED", TyreBlankets);
      VirtualMirror = source.V("SERVER", "FORCE_VIRTUAL_MIRROR", VirtualMirror);
    }
  }
}
