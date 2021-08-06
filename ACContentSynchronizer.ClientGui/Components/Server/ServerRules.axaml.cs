using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ACContentSynchronizer.ClientGui.Components.Server {
  public class ServerRules : UserControl {
    private readonly ServerRulesViewModel _vm;
    private static ServerRules? _instance;

    public ServerRules() {
      DataContext = _vm = new();
      InitializeComponent();
    }

    public static ServerRules Instance => _instance ??= new();

    private void InitializeComponent() {
      AvaloniaXamlLoader.Load(this);
    }

    public Dictionary<string, Dictionary<string, object>> ToConfig(
      Dictionary<string, Dictionary<string, object>> source) {
      source["SERVER"]["KICK_QUORUM"] = _vm.KickVoteQuorum;
      source["SERVER"]["VOTING_QUORUM"] = _vm.SessionVoteQuorum;
      source["SERVER"]["VOTE_DURATION"] = _vm.VoteDuration;
      source["SERVER"]["FUEL_RATE"] = _vm.FuelRate;
      source["SERVER"]["DAMAGE_MULTIPLIER"] = _vm.DamageRate;
      source["SERVER"]["TYRE_WEAR_RATE"] = _vm.TyresWearRate;
      source["SERVER"]["ALLOWED_TYRES_OUT"] = _vm.AllowedTyresOut;
      source["SERVER"]["ABS_ALLOWED"] = _vm.AbsMode;
      source["SERVER"]["TC_ALLOWED"] = _vm.TcMode;
      source["SERVER"]["START_RULE"] = _vm.SpawnType;
      source["SERVER"]["STABILITY_ALLOWED"] = _vm.StabilityControl ? "1" : "0";
      source["SERVER"]["AUTOCLUTCH_ALLOWED"] = _vm.AutomaticClutch ? "1" : "0";
      source["SERVER"]["TYRE_BLANKETS_ALLOWED"] = _vm.TyreBlankets ? "1" : "0";
      source["SERVER"]["FORCE_VIRTUAL_MIRROR"] = _vm.VirtualMirror ? "1" : "0";
      source["SERVER"]["MAX_BALLAST_KG"] = _vm.MaxBallast;

      return source;
    }
  }
}
