using System;
using System.Collections.Generic;
using ACContentSynchronizer.Extensions;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ACContentSynchronizer.ClientGui.Components.Server {
  public class ServerRules : UserControl {
    private static ServerRules? _instance;
    private readonly ServerRulesViewModel _vm;

    public ServerRules() {
      DataContext = _vm = new();
      InitializeComponent();
    }

    public static ServerRules Instance => _instance ??= new();
    public static ServerRulesViewModel ViewModel => Instance._vm;

    private void InitializeComponent() {
      AvaloniaXamlLoader.Load(this);
    }

    /*
          ["MAX_BALLAST_KG"] = rules.MaxBallast,
          ["KICK_QUORUM"] = rules.KickVoteQuorum,
          ["VOTING_QUORUM"] = rules.SessionVoteQuorum,
          ["VOTE_DURATION"] = rules.VoteDuration.TotalSeconds,
          ["FUEL_RATE"] = rules.FuelRate,
          ["DAMAGE_MULTIPLIER"] = rules.DamageRate,
          ["TYRE_WEAR_RATE"] = rules.TyresWearRate,
          ["ALLOWED_TYRES_OUT"] = rules.AllowedTyresOut,
          ["ABS_ALLOWED"] = (int) rules.AbsMode,
          ["TC_ALLOWED"] = (int) rules.TcMode,
          ["START_RULE"] = (int) rules.SpawnType,
          ["RACE_GAS_PENALTY_DISABLED"] = rules.DisableGasCut.ToInt(),
          ["MAX_CONTACTS_PER_KM"] = rules.MaxContactsPerKm,
          ["STABILITY_ALLOWED"] = rules.StabilityControl.ToInt(),
          ["AUTOCLUTCH_ALLOWED"] = rules.AutomaticClutch,
          ["TYRE_BLANKETS_ALLOWED"] = rules.TyreBlankets.ToInt(),
          ["FORCE_VIRTUAL_MIRROR"] = rules.VirtualMirror,
     */

    public IniFile Save(IniFile source) {
      source["SERVER"]["MAX_BALLAST_KG"] = _vm.MaxBallast;
      source["SERVER"]["KICK_QUORUM"] = _vm.KickVoteQuorum;
      source["SERVER"]["VOTING_QUORUM"] = _vm.SessionVoteQuorum;
      source["SERVER"]["VOTE_DURATION"] = _vm.VoteDuration.TotalSeconds;
      source["SERVER"]["FUEL_RATE"] = _vm.FuelRate;
      source["SERVER"]["DAMAGE_MULTIPLIER"] = _vm.DamageRate;
      source["SERVER"]["TYRE_WEAR_RATE"] = _vm.TyresWearRate;
      source["SERVER"]["ALLOWED_TYRES_OUT"] = _vm.AllowedTyresOut;
      source["SERVER"]["RACE_GAS_PENALTY_DISABLED"] = _vm.DisableGasCut.ToInt();
      source["SERVER"]["ABS_ALLOWED"] = (int) _vm.AbsMode;
      source["SERVER"]["TC_ALLOWED"] = (int) _vm.TcMode;
      source["SERVER"]["START_RULE"] = (int) _vm.SpawnType;
      source["SERVER"]["RACE_GAS_PENALTY_DISABLED"] = _vm.DisableGasCut.ToInt();
      source["SERVER"]["MAX_CONTACTS_PER_KM"] = _vm.MaxContactsPerKm;
      source["SERVER"]["STABILITY_ALLOWED"] = _vm.StabilityControl.ToInt();
      source["SERVER"]["AUTOCLUTCH_ALLOWED"] = _vm.AutomaticClutch;
      source["SERVER"]["TYRE_BLANKETS_ALLOWED"] = _vm.TyreBlankets.ToInt();
      source["SERVER"]["FORCE_VIRTUAL_MIRROR"] = _vm.VirtualMirror;

      return source;
    }

    public void Load(IniFile source) {
      _vm.MaxBallast = source.V("SERVER", "MAX_BALLAST_KG", _vm.MaxBallast);
      _vm.KickVoteQuorum = source.V("SERVER", "KICK_QUORUM", _vm.KickVoteQuorum);
      _vm.SessionVoteQuorum = source.V("SERVER", "VOTING_QUORUM", _vm.SessionVoteQuorum);
      _vm.VoteDuration = TimeSpan.FromSeconds(source.V("SERVER", "VOTE_DURATION", _vm.VoteDuration.TotalSeconds));
      _vm.FuelRate = source.V("SERVER", "FUEL_RATE", _vm.FuelRate);
      _vm.DamageRate = source.V("SERVER", "DAMAGE_MULTIPLIER", _vm.DamageRate);
      _vm.TyresWearRate = source.V("SERVER", "TYRE_WEAR_RATE", _vm.TyresWearRate);
      _vm.AllowedTyresOut = source.V("SERVER", "ALLOWED_TYRES_OUT", _vm.AllowedTyresOut);
      _vm.AbsMode = source.V("SERVER", "ABS_ALLOWED", _vm.AbsMode);
      _vm.TcMode = source.V("SERVER", "TC_ALLOWED", _vm.TcMode);
      _vm.SpawnType = source.V("SERVER", "START_RULE", _vm.SpawnType);
      _vm.DisableGasCut = source.V("SERVER", "RACE_GAS_PENALTY_DISABLED", _vm.DisableGasCut);
      _vm.MaxContactsPerKm = source.V("SERVER", "MAX_CONTACTS_PER_KM", _vm.MaxContactsPerKm);
      _vm.StabilityControl = source.V("SERVER", "STABILITY_ALLOWED", _vm.StabilityControl);
      _vm.AutomaticClutch = source.V("SERVER", "AUTOCLUTCH_ALLOWED", _vm.AutomaticClutch);
      _vm.TyreBlankets = source.V("SERVER", "TYRE_BLANKETS_ALLOWED", _vm.TyreBlankets);
      _vm.VirtualMirror = source.V("SERVER", "FORCE_VIRTUAL_MIRROR", _vm.VirtualMirror);
    }
  }
}
