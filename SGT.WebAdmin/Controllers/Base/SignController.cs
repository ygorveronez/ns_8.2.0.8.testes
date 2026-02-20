using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Principal;

namespace SGT.WebAdmin.Controllers
{
    public class SignController : Controller
    {
        #region Propriedades

        public Conexao _conexao;

        #endregion

        #region Construtores

        public SignController(Conexao conexao)
        {
            _conexao = conexao;
        }

        #endregion

        #region Métodos Protegidos

        protected void SignIn(Dominio.Entidades.Usuario usuario, bool gerenciarTransportadores = false, bool loginSSO = false, Dominio.ObjetosDeValor.UsuarioInterno usuarioInterno = null)
        {
            // Autenticação Owin            
            List<System.Security.Claims.Claim> claims = new List<System.Security.Claims.Claim>
            {
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, usuario.Codigo.ToString()),
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, usuario.Codigo.ToString()),
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Email, usuario.Login),
                new System.Security.Claims.Claim("LoginSSO", loginSSO.ToString())
            };

            if (usuarioInterno != null)
                claims.Add(new System.Security.Claims.Claim("InternalUser", JsonConvert.SerializeObject(usuarioInterno)));

            if (!gerenciarTransportadores)//Acesso a empresa Filho pela tela Transportadores/GerenciarTransportadores não irá atualizar usuário e nem mudar session (EncodedLoginController)
            {
#if !DEBUG
                using Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

                usuario.Session = Guid.NewGuid().ToString();
                usuario.UltimoAcesso = DateTime.Now;

                repUsuario.Atualizar(usuario);

                new Servicos.Embarcador.Login.Login(unitOfWork).SalvarLogAcesso(usuario, HttpContext.Connection.RemoteIpAddress.ToString(), Dominio.Enumeradores.TipoLogAcesso.Entrada, usuarioInterno, unitOfWork);
#endif
                if (usuario.Session is not null)
                    claims.Add(new System.Security.Claims.Claim("GuidUser", usuario.Session));
            }

            System.Security.Claims.ClaimsIdentity identity = new System.Security.Claims.ClaimsIdentity(claims, Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ApplicationCookie);

            AuthenticationProperties authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                IsPersistent = true
            };

            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new System.Security.Claims.ClaimsPrincipal(identity), authProperties).Wait();

            HttpContext.User = new GenericPrincipal(new GenericIdentity(usuario.Codigo.ToString()), null);
            System.Threading.Thread.CurrentPrincipal = HttpContext.User;
        }

        protected void SignOut()
        {
#if !DEBUG
            using Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova);

            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

            Dominio.Entidades.Usuario usuario = repUsuario.BuscarPorCodigo(int.Parse(User.GetCodigoUsuario()));

            if (usuario != null)
            {
                string internalUser = User.GetInternalUser();
                Dominio.ObjetosDeValor.UsuarioInterno usuarioInterno = !string.IsNullOrWhiteSpace(internalUser) ? JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.UsuarioInterno>(internalUser) : null;

                new Servicos.Embarcador.Login.Login(unitOfWork).SalvarLogAcesso(usuario, HttpContext.Connection.RemoteIpAddress.ToString(), Dominio.Enumeradores.TipoLogAcesso.Saída, usuarioInterno, unitOfWork);
            }
#endif

            SignOutExternal();
        }

        protected void SignOutExternal()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).Wait();
            HttpContext.User = new GenericPrincipal(new GenericIdentity(string.Empty), null);
        }

        #endregion
    }
}