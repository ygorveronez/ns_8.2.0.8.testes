using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Notificacoes
{
    [CustomAuthorize("Notificacoes/Dashboard")]
    public class DashboardController : BaseController
    {
		#region Construtores

		public DashboardController(Conexao conexao) : base(conexao) { }

		#endregion

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarDashboardDocumentacao()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("HomeCabotagem/Index");

                if (permissoesPersonalizadas != null && permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Home_VisualizarDashDoc))
                {
                    Repositorio.Embarcador.BI.PermissaoAcessoUsuarioBI repPermissaoAcessoUsuarioBI = new Repositorio.Embarcador.BI.PermissaoAcessoUsuarioBI(unidadeTrabalho);
                    Dominio.Entidades.Embarcador.BI.PermissaoAcessoUsuarioBI permissaoAcessoUsuarioBI = repPermissaoAcessoUsuarioBI.BuscarPorPermissao(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Home_VisualizarDashDoc);

                    IList<Dominio.ObjetosDeValor.Embarcador.Dashboard.DashboardDocumentacao> dashboardDocumentacaos = null;
                    if (permissaoAcessoUsuarioBI != null && (permissaoAcessoUsuarioBI.CodigoFormularioBI > 0 || !string.IsNullOrWhiteSpace(permissaoAcessoUsuarioBI.CaminhoFormulario)))
                        dashboardDocumentacaos = repPermissaoAcessoUsuarioBI.BuscarDashboardDocumentacao();

                    var retorno = new
                    {
                        PossuiAcessoAoDashboardDocumentacao = permissaoAcessoUsuarioBI != null && (permissaoAcessoUsuarioBI.CodigoFormularioBI > 0 || !string.IsNullOrWhiteSpace(permissaoAcessoUsuarioBI.CaminhoFormulario)),
                        QuantidadeNavioFechado = dashboardDocumentacaos.Count(item => item.NavioAberto == 0),
                        QuantidadeNavioAberto = dashboardDocumentacaos.Count(item => item.NavioAberto == 1),
                        DataInicial = DateTime.Now.Date.AddDays(-15).ToString("dd/MM/yyyy"),
                        DataFinal = DateTime.Now.Date.ToString("dd/MM/yyyy"),
                        IDDashboardDocumentacao = permissaoAcessoUsuarioBI != null ? permissaoAcessoUsuarioBI.CodigoFormularioBI : 0,
                        CaminhoDashboardDocumentacao = permissaoAcessoUsuarioBI != null ? permissaoAcessoUsuarioBI.CaminhoFormulario : ""
                    };

                    return new JsonpResult(retorno);
                }
                else
                {
                    var retorno = new
                    {
                        PossuiAcessoAoDashboardDocumentacao = false,
                        QuantidadeNavioFechado = "",
                        QuantidadeNavioAberto = "",
                        DataInicial = "",
                        DataFinal = "",
                        IDDashboardDocumentacao = 0,
                        CaminhoDashboardDocumentacao = ""
                    };

                    return new JsonpResult(retorno);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao obter o dash de documentação.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }


        [AllowAuthenticate]
        public async Task<IActionResult> CriarLinkAcessoDashboard()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);
            AdminMultisoftware.Repositorio.UnitOfWork adminUnitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(_conexao.AdminStringConexao);

            try
            {
                int idDashboardDocumentacao = Request.GetIntParam("idDashboardDocumentacao");
                string caminhoDashboardDocumentacao = Request.GetStringParam("CaminhoDashboardDocumentacao");
                string urlBase = Servicos.Embarcador.GestaoEntregas.NotificaoEntrega.ObterURLBase(Cliente.Codigo, Cliente.ClienteConfiguracao.TipoServicoMultisoftware, adminUnitOfWork, _conexao.AdminStringConexao, unidadeTrabalho);

                var retorno = new
                {
                    URL = !string.IsNullOrWhiteSpace(caminhoDashboardDocumentacao) ? urlBase + "/#" + caminhoDashboardDocumentacao : idDashboardDocumentacao <= 0 ? "" : urlBase + "/#BusinessIntelligence/BI/Report?ID=" + idDashboardDocumentacao.ToString("D")
                };

                return new JsonpResult(retorno);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao gerar a URL para acesso ao dashboard.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
                adminUnitOfWork.Dispose();
            }
        }

    }
}
