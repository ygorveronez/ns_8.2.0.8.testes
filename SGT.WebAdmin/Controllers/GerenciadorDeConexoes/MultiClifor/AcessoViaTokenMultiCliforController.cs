using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.GerenciadorDeConexoes.MultiClifor
{
    [AllowAnonymous]
    public class AcessoViaTokenMultiCliforController : BaseController
    {
		#region Construtores

		public AcessoViaTokenMultiCliforController(Conexao conexao) : base(conexao) { }

		#endregion

        [AllowAnonymous]
        public async Task<IActionResult> Acessar(string token)
        {
            string caminhoBaseViews = "~/Views/GerenciadorDeConexoes/MultiClifor/";

            try
            {
                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
                {
                    MontaLayoutBase(unitOfWork);
                    DefineParametrosView(token, unitOfWork);

                    Dominio.ObjetosDeValor.Embarcador.GerenciadorDeConexoes.MultiClifor.AcessoViaTokenMultiCliforView dataView = ObtemDadosRenderizacao(token, unitOfWork);

                    if (dataView == null)
                        return View(caminhoBaseViews + "AcessoViaTokenErroMultiClifor.cshtml");

                    return View(caminhoBaseViews + "AcessoViaTokenDetalhesMultiClifor.cshtml", dataView);
                }
            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e);
                return View(caminhoBaseViews + "AcessoViaTokenErroMultiClifor.cshtml");
            }
        }

        private void DefineParametrosView(string token, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoAmbiente repConfiguracaoAmbiente = new Repositorio.Embarcador.Configuracoes.ConfiguracaoAmbiente(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAmbiente configuracaoAmbiente = repConfiguracaoAmbiente.BuscarPrimeiroRegistro();
            string protocolo = (Request.IsHttps ? "https" : "http");
            if (configuracaoAmbiente?.TipoProtocolo != null && configuracaoAmbiente?.TipoProtocolo.ObterProtocolo() != "")
                protocolo = configuracaoAmbiente?.TipoProtocolo.ObterProtocolo();
            ViewBag.HTTPConnection = protocolo;
            ViewBag.Token = token;
        }

        private void MontaLayoutBase(Repositorio.UnitOfWork unitOfWork)
        {
            AdminMultisoftware.Repositorio.UnitOfWork adminMultisoftwareUnitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(_conexao.AdminStringConexao);

            try
            {
                AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso repClienteURLAcesso = new AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso(adminMultisoftwareUnitOfWork);
                AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURLAcesso = repClienteURLAcesso.BuscarPorURL(_conexao.ObterHost);

                Servicos.Embarcador.Configuracoes.Layout.MontarLayout(ViewBag, clienteURLAcesso, unitOfWork);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
            finally
            {
                adminMultisoftwareUnitOfWork.Dispose();
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.GerenciadorDeConexoes.MultiClifor.AcessoViaTokenMultiCliforView ObtemDadosRenderizacao(string token, Repositorio.UnitOfWork unitOfWork)
        {

            Servicos.Login.AcessoViaToken servicoAcessoViaToken = new Servicos.Login.AcessoViaToken(unitOfWork, TipoServicoMultisoftware);
            
            Dominio.Entidades.Usuario usuario = servicoAcessoViaToken.Autenticar(token);

            if (usuario == null || (!ClienteAcesso.URLAcesso.Contains("clifor")) || !(Cliente.URLAutenticadaViaCodigoDeIntegracaoDoUsuarioParaPortalMultiClifor))
                return null;

            Dominio.ObjetosDeValor.Embarcador.GerenciadorDeConexoes.MultiClifor.AcessoViaTokenMultiCliforView dataView = new Dominio.ObjetosDeValor.Embarcador.GerenciadorDeConexoes.MultiClifor.AcessoViaTokenMultiCliforView
            {
                TokenAcesso = token
            };

            dataView.UsuarioAutenticado = AutenticarUsuarioNoSistema(usuario);

            return dataView;
        }

        private bool AutenticarUsuarioNoSistema(Dominio.Entidades.Usuario usuario)
        {
            Usuario = usuario;

            base.SignIn(Usuario);

            return true;
        }
    }
}
