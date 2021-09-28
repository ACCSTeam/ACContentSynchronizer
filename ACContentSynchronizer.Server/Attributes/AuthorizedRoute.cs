using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ACContentSynchronizer.Server.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;

namespace ACContentSynchronizer.Server.Attributes {
  public class AuthorizedRouteAttribute : TypeFilterAttribute {
    public AuthorizedRouteAttribute(PasswordType passwordType) : base(typeof(CheckPassword)) {
      Arguments = new object[] { passwordType };
    }
  }

  public enum PasswordType {
    User,
    Admin,
  }

  public class CheckPassword : IAsyncActionFilter {
    private readonly IConfiguration _configuration;
    private readonly PasswordType _passwordType;

    public CheckPassword(PasswordType passwordType,
                         IConfiguration configuration) {
      _passwordType = passwordType;
      _configuration = configuration;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next) {
      var serverPreset = context.HttpContext.GetServerPreset();
      var gamePath = _configuration.GetValue<string>("GamePath");
      var iniProvider = new IniProvider(Path.Combine(gamePath, Constants.ServerPresetsPath, serverPreset));

      var passwordTypeString = _passwordType switch {
        PasswordType.User => new[] { "PASSWORD", "ADMIN_PASSWORD" },
        PasswordType.Admin => new[] { "ADMIN_PASSWORD" },
        _ => throw new ArgumentOutOfRangeException(),
      };

      var password = context.HttpContext.GetHeader(DefaultHeaders.AccessPassword);
      var hasAccess = passwordTypeString.Any(s => {
        var serverPassword = iniProvider.GetServerConfig().V("SERVER", s, "");
        if (string.IsNullOrEmpty(serverPassword)) {
          return true;
        }

        return serverPassword == password;
      });

      if (hasAccess) {
        await next();
        return;
      }

      context.HttpContext.Response.StatusCode = 403;
    }
  }
}
