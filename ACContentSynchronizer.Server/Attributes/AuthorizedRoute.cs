using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
    private readonly IniProvider _iniProvider;
    private readonly PasswordType _passwordType;

    public CheckPassword(PasswordType passwordType,
                         IConfiguration configuration) {
      _passwordType = passwordType;

      var gamePath = configuration.GetValue<string>("GamePath");
      var preset = configuration.GetValue<string>("Preset");
      _iniProvider = new(Path.Combine(gamePath, Constants.ServerPresetsPath, preset));
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next) {
      var passwordTypeString = _passwordType switch {
        PasswordType.User => new[] { "PASSWORD", "ADMIN_PASSWORD" },
        PasswordType.Admin => new[] { "ADMIN_PASSWORD" },
        _ => throw new ArgumentOutOfRangeException(),
      };

      var password = context.HttpContext.Request.Headers[DefaultHeaders.Password].ToString();
      var hasAccess = passwordTypeString.Any(s => {
        var serverPassword = _iniProvider.GetServerConfig().V("SERVER", s, "");

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
