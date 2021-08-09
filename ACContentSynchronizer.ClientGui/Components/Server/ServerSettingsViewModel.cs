using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ACContentSynchronizer.ClientGui.ViewModels;
using ACContentSynchronizer.Extensions;
using ReactiveUI;

namespace ACContentSynchronizer.ClientGui.Components.Server {
  public class ServerSettingsViewModel : ViewModelBase {
    private IniFile? _config;

    public ServerSettingsViewModel() {
      ReactiveCommand.CreateFromTask(Load);
    }

    private async Task Load() {
      var dataReceiver = new DataReceiver(Views.Server.GetServer.Http);
      _config = await dataReceiver.GetServerInfo();
    }

    public void Save() {
      _config ??= new() {
        ["SERVER"] = new(),
        ["FTP"] = new(),
        ["PRACTICE"] = new(),
        ["QUALIFY"] = new(),
        ["RACE"] = new(),
        ["DYNAMIC_TRACK"] = new(),
        ["DATA"] = new(),
      };

      double GetSunAngle(ServerConditionsViewModel conditions) {
        const int middle = 13;
        const double angleInMin = 3.7;
        const double angleInHour = 16;

        var hours = conditions.Time.Hours;
        var hoursPassed = hours - middle;
        var angle = hoursPassed * angleInHour;
        angle += conditions.Time.Minutes / angleInMin;

        return Math.Round(angle);
      }

      var main = ServerMain.ViewModel;
      var conditions = ServerConditions.ViewModel;
      var rules = ServerRules.ViewModel;
      var sessions = ServerSessions.ViewModel;

      var serverConfig = new IniFile {
        ["SERVER"] = new() {
          ["NAME"] = main.ServerName,
          ["CARS"] = string.Join(';', main.SelectedCars.Select(x => DirectoryUtils.Name(x.Path)).Distinct()),
          ["CONFIG_TRACK"] = main.SelectedTrack?.SelectedVariation,
          ["TRACK"] = DirectoryUtils.Name(main.SelectedTrack?.Path),
          ["SUN_ANGLE"] = GetSunAngle(conditions),
          ["PASSWORD"] = main.Password,
          ["ADMIN_PASSWORD"] = main.AdminPassword,
          ["UDP_PORT"] = main.UdpPort,
          ["TCP_PORT"] = main.TcpPort,
          ["HTTP_PORT"] = main.HttpPort,
          ["MAX_BALLAST_KG"] = rules.MaxBallast,
          ["QUALIFY_MAX_WAIT_PERC"] = sessions.QualifyLimit,
          ["RACE_PIT_WINDOW_START"] = sessions.From,
          ["RACE_PIT_WINDOW_END"] = sessions.To,
          ["REVERSED_GRID_RACE_POSITIONS"] = sessions.ReversedGrid,
          ["LOCKED_ENTRY_LIST"] = sessions.LockedEntryList,
          ["PICKUP_MODE_ENABLED"] = sessions.PickupMode.ToInt(),
          ["LOOP_MODE"] = sessions.PickupMode.ToInt(),
          ["SLEEP_TIME"] = "1",
          ["CLIENT_SEND_INTERVAL_HZ"] = main.PacketSize,
          ["SEND_BUFFER_SIZE"] = "0",
          ["RECV_BUFFER_SIZE"] = "0",
          ["RACE_OVER_TIME"] = sessions.RaceOver.Seconds,
          ["KICK_QUORUM"] = rules.KickVoteQuorum,
          ["VOTING_QUORUM"] = rules.SessionVoteQuorum,
          ["VOTE_DURATION"] = rules.VoteDuration.TotalSeconds,
          ["BLACKLIST_MODE"] = "0",
          ["FUEL_RATE"] = rules.FuelRate,
          ["DAMAGE_MULTIPLIER"] = rules.DamageRate,
          ["TYRE_WEAR_RATE"] = rules.TyresWearRate,
          ["ALLOWED_TYRES_OUT"] = rules.AllowedTyresOut,
          ["ABS_ALLOWED"] = (int) rules.AbsMode,
          ["TC_ALLOWED"] = (int) rules.TcMode,
          ["START_RULE"] = (int) rules.SpawnType,
          ["RACE_GAS_PENALTY_DISABLED"] = rules.DisableGasCut.ToInt(),
          ["TIME_OF_DAY_MULT"] = conditions.TimeMultiplier,
          ["RESULT_SCREEN_TIME"] = sessions.ResultScreen.TotalSeconds,
          ["MAX_CONTACTS_PER_KM"] = rules.MaxContactsPerKm,
          ["STABILITY_ALLOWED"] = rules.StabilityControl.ToInt(),
          ["AUTOCLUTCH_ALLOWED"] = rules.AutomaticClutch,
          ["TYRE_BLANKETS_ALLOWED"] = rules.TyreBlankets.ToInt(),
          ["FORCE_VIRTUAL_MIRROR"] = rules.VirtualMirror,
          ["REGISTER_TO_LOBBY"] = main.PublicServer.ToInt(),
          ["MAX_CLIENTS"] = main.SelectedCars.Count,
          ["NUM_THREADS"] = main.Threads,
          ["UDP_PLUGIN_LOCAL_PORT"] = "0",
          ["UDP_PLUGIN_ADDRESS"] = "",
          ["AUTH_PLUGIN_ADDRESS"] = "",
          ["LEGAL_TYRES"] = "",
          ["RACE_EXTRA_LAP"] = sessions.ExtraLap.ToInt(),
          ["WELCOME_MESSAGE"] = "",
        },
        ["DATA"] = new() {
          ["DESCRIPTION"] = "",
          ["EXSERVEREXE"] = "",
          ["EXSERVERBAT"] = "",
          ["EXSERVERHIDEWIN"] = "0",
          ["WEBLINK"] = "",
          ["WELCOME_PATH"] = "",
        },
      };

      if (conditions.DynamicTrack) {
        serverConfig.Add("DYNAMIC_TRACK", new() {
          ["SESSION_START"] = conditions.StartValue,
          ["RANDOMNESS"] = conditions.Randomness,
          ["SESSION_TRANSFER"] = conditions.Transferred,
          ["LAP_GAIN"] = conditions.Laps,
        });
      }

      if (sessions.Booking) {
        serverConfig.Add("BOOKING", new() {
          ["NAME"] = "Booking",
          ["TIME"] = sessions.BookingTime.TotalMinutes,
        });
      }

      if (sessions.Practice) {
        serverConfig.Add("PRACTICE", new() {
          ["NAME"] = "Practice",
          ["TIME"] = sessions.PracticeTime.TotalMinutes,
          ["IS_OPEN"] = sessions.PracticeCanJoin.ToInt(),
        });
      }

      if (sessions.Qualification) {
        serverConfig.Add("QUALIFY", new() {
          ["NAME"] = "Qualify",
          ["TIME"] = sessions.QualificationTime.TotalMinutes,
          ["IS_OPEN"] = sessions.QualificationCanJoin.ToInt(),
        });
      }

      if (sessions.Race) {
        serverConfig.Add("RACE", new() {
          ["NAME"] = "Race",
          ["LAPS"] = sessions.Laps,
          ["TIME"] = sessions.Time,
          ["WAIT_TIME"] = sessions.InitialDelay.TotalSeconds,
          ["IS_OPEN"] = (int) sessions.JoinType,
        });
      }

      for (var i = 0; i < conditions.Weathers.Count; i++) {
        var weather = conditions.Weathers[i].ViewModel;

        serverConfig.Add($"WEATHER_{i}", new() {
          ["GRAPHICS"] = weather.Graphics,
          ["BASE_TEMPERATURE_AMBIENT"] = weather.AmbientTemperature,
          ["BASE_TEMPERATURE_ROAD"] = weather.RoadTemperature,
          ["VARIATION_AMBIENT"] = weather.AmbientVariation,
          ["VARIATION_ROAD"] = weather.RoadVariation,
          ["WIND_BASE_SPEED_MIN"] = weather.WindMin,
          ["WIND_BASE_SPEED_MAX"] = weather.WindMax,
          ["WIND_BASE_DIRECTION"] = weather.WindDirection,
          ["WIND_VARIATION_DIRECTION"] = weather.WindDirectionVariation,
        });
      }

      var entryList = new IniFile {

      };
    }
  }
}
