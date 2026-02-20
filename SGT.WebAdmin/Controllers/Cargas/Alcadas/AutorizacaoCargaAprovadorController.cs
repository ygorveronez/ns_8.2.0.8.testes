using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.Alcadas
{
    [CustomAuthorize("Alcadas/AutorizacaoCargaAprovador")]
    public class AutorizacaoCargaAprovadorController : BaseController
    {
		#region Construtores

		public AutorizacaoCargaAprovadorController(Conexao conexao) : base(conexao) { }

		#endregion

        [AllowAnonymous]
        public async Task<IActionResult> Aprovacao(string token)
        {
            string caminhoBaseViews = "~/Views/Cargas/";

            try
            {
                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
                {
                    MontaLayoutBase(unitOfWork);
                    DefineParametrosView(token, unitOfWork);

                    Dominio.ObjetosDeValor.Embarcador.Carga.AutorizacaoCargaAprovadorView dataView = ObtemDadosRenderizacao(token, unitOfWork);

                    if (dataView == null)
                        return View(caminhoBaseViews + "CargaAprovadorErro.cshtml");

                    return View(caminhoBaseViews + "CargaAprovadorDetalhes.cshtml", dataView);
                }
            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e);
                return View(caminhoBaseViews + "CargaAprovadorErro.cshtml");
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

        private Dominio.ObjetosDeValor.Embarcador.Carga.AutorizacaoCargaAprovadorView ObtemDadosRenderizacao(string token, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.AlcadasCarga.AprovacaoAlcadaCarga repositorioAprovacaoAlcadaCarga = new Repositorio.Embarcador.Cargas.AlcadasCarga.AprovacaoAlcadaCarga(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AprovacaoAlcadaCarga aprovacaoAlcadaCarga = repositorioAprovacaoAlcadaCarga.BuscarPorGuid(token);

            if (aprovacaoAlcadaCarga == null)
                return null;

            AutenticarUsuarioNoSistema(aprovacaoAlcadaCarga.Usuario);

            Dominio.ObjetosDeValor.Embarcador.Carga.AutorizacaoCargaAprovadorView dataView = new Dominio.ObjetosDeValor.Embarcador.Carga.AutorizacaoCargaAprovadorView
            {
                SituacaoAlcadaRegra = aprovacaoAlcadaCarga.Situacao,
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
