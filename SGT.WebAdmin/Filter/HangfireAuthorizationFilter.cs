using Hangfire.Dashboard;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using SGT.WebAdmin.Controllers;
using System.Threading.Tasks;

namespace SGT.WebAdmin.Filter;

public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();

        var user = httpContext.User;
        if (user == null || !user.Identity.IsAuthenticated)
        {
            return false;
        }

        var conexao = httpContext.RequestServices.GetRequiredService<Conexao>();
        using var unitOfWork = new Repositorio.UnitOfWork(conexao.StringConexao);
        var usuario = GetUsuario(httpContext, unitOfWork);

        if(usuario != null && usuario.UsuarioAdministrador)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private Dominio.Entidades.Usuario GetUsuario(HttpContext httpContext, Repositorio.UnitOfWork unitOfWork)
    {
        var repositorioUsuario = new Repositorio.Usuario(unitOfWork);
        int? codigoUsuario = httpContext.User.Claims.FirstOrDefault(o => o.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value.ToInt();

        if (!codigoUsuario.HasValue || codigoUsuario.Value == 0)
            return null;

        return repositorioUsuario.BuscarPorCodigo(codigoUsuario.Value);
    }
}