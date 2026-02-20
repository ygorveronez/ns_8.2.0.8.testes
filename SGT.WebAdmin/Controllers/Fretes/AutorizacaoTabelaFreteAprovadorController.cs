using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Fretes
{
    [CustomAuthorize("Fretes/AutorizacaoTabelaFreteAprovador")]
    public class AutorizacaoTabelaFreteAprovadorController : BaseController
    {
		#region Construtores

		public AutorizacaoTabelaFreteAprovadorController(Conexao conexao) : base(conexao) { }

		#endregion

        [AllowAnonymous]
        public async Task<IActionResult> Aprovacao(string token)
        {
            string caminhoBaseViews = "~/Views/Fretes/";

            try
            {
                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
                {
                    MontaLayoutBase(unitOfWork);
                    DefineParametrosView(token, unitOfWork);

                    Dominio.ObjetosDeValor.Embarcador.Frete.AutorizacaoTabelaFreteAprovadorView dataView = ObtemDadosRenderizacao(token, unitOfWork);

                    if (dataView == null)
                        return View(caminhoBaseViews + "TabelaFreteAprovadorErro.cshtml");

                    return View(caminhoBaseViews + "TabelaFreteAprovadorDetalhes.cshtml", dataView);
                }
            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e);
                return View(caminhoBaseViews + "TabelaFreteAprovadorErro.cshtml");
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

        private Dominio.ObjetosDeValor.Embarcador.Frete.AutorizacaoTabelaFreteAprovadorView ObtemDadosRenderizacao(string token, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frete.AjusteTabelaFreteAutorizacao repositorioAjusteTabelaFreteAutorizacao = new Repositorio.Embarcador.Frete.AjusteTabelaFreteAutorizacao(unitOfWork);
            Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteAutorizacao ajusteTabelaFreteAutorizacao = repositorioAjusteTabelaFreteAutorizacao.BuscarPorGuid(token);

            if (ajusteTabelaFreteAutorizacao == null)
                return null;

            AutenticarUsuarioNoSistema(ajusteTabelaFreteAutorizacao.Usuario);

            Dominio.ObjetosDeValor.Embarcador.Frete.AutorizacaoTabelaFreteAprovadorView dataView = new Dominio.ObjetosDeValor.Embarcador.Frete.AutorizacaoTabelaFreteAprovadorView
            {
                SituacaoAjusteTabelaFreteAutorizacao = ajusteTabelaFreteAutorizacao.Situacao,
                TokenAcesso = token
            };

            return dataView;
        }

        private void AutenticarUsuarioNoSistema(Dominio.Entidades.Usuario usuario)
        {
            Usuario = usuario;

            base.SignIn(Usuario);
        }
    }
}
