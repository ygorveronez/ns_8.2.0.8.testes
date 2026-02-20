using System.Security.Claims;
using System.Security.Principal;

namespace SGT.WebAdmin.Controllers;

public static class HttpContextUserExtensions
{
    public static string GetCodigoUsuario(this IPrincipal principal)
    {
        var claims = principal.Identity as ClaimsIdentity;
        return claims?.FindFirst(ClaimTypes.Name)?.Value;
    }
    
    public static bool GetLoginSSO(this IPrincipal principal)
    {
        var claims = principal.Identity as ClaimsIdentity;
        var value = claims?.FindFirst("LoginSSO")?.Value;
        return value != null && bool.Parse(value);
    }

    public static string GetInternalUser(this IPrincipal principal)
    {
        var claims = principal.Identity as ClaimsIdentity;
        var value = claims?.FindFirst("InternalUser")?.Value;
        return value;
    }
}
