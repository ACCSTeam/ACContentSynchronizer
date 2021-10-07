using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ACContentSynchronizer.Server.Extensions;
using ACContentSynchronizer.Server.Services;
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
    private readonly PasswordType _passwordType;
    private readonly ServerConfigurationService _serverConfiguration;

    public CheckPassword(PasswordType passwordType,
                         ServerConfigurationService serverConfiguration) {
      _passwordType = passwordType;
      _serverConfiguration = serverConfiguration;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next) {
      var hasAccess = _serverConfiguration.HasPrivileges(_passwordType);

      if (hasAccess) {
        await next();
        return;
      }

      context.HttpContext.Response.StatusCode = 403;
    }
  }
}
