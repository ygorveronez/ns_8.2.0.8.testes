using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Dash
{
    [CustomAuthorize("Dash/Documentacao")]
    public class DocumentacaoController : BaseController
    {
		#region Construtores

		public DocumentacaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.BI.PermissaoAcessoUsuarioBI repositorioPermissaoAcessoUsuarioBI = new Repositorio.Embarcador.BI.PermissaoAcessoUsuarioBI(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Dashboard.FiltroPesquisaDashboardDocumentacao filtrosPesquisa = ObterFiltrosPesquisa();

                IList<Dominio.ObjetosDeValor.Embarcador.Dashboard.DashboardDocumentacao> dashboardDocumentacoes = null;
                dashboardDocumentacoes = repositorioPermissaoAcessoUsuarioBI.PesquisarDashboardDocumentacao(filtrosPesquisa);

                IList<Dominio.ObjetosDeValor.Embarcador.Dashboard.Embarques> portos = repositorioPermissaoAcessoUsuarioBI.ObterPortos(filtrosPesquisa);
                ObterTotais(dashboardDocumentacoes);

                var retorno = new
                {
                    QuantidadeNavioFechado = filtrosPesquisa.NavioFechado.GetValueOrDefault() ? dashboardDocumentacoes.Count(item => item.NavioAberto == 0) : 0,
                    QuantidadeNavioAberto = filtrosPesquisa.NavioAberto.GetValueOrDefault() ? dashboardDocumentacoes.Count(item => item.NavioAberto == 1) : 0,
                    DadosDocumentacaoDash = dashboardDocumentacoes?.ToArray(),
                    Embarques = (from obj in portos
                                select new { 
                                    obj.Codigo, 
                                    obj.Descricao 
                                }).ToList(),
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaDetalhesModal()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.BI.PermissaoAcessoUsuarioBI repositorioPermissaoAcessoUsuarioBI = new Repositorio.Embarcador.BI.PermissaoAcessoUsuarioBI(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Dashboard.FiltroPesquisaDetalhesDashboardDocumentacao filtrosPesquisa = ObterFiltrosPesquisaDetalhes();

                IList<Dominio.ObjetosDeValor.Embarcador.Dashboard.DetalhesDashboardDocumentacao> detalhesDashboardDocumentacoes = null;
                detalhesDashboardDocumentacoes = repositorioPermissaoAcessoUsuarioBI.PesquisarDetalhesDashboardDocumentacao(filtrosPesquisa);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("LinkCarga", false);
                grid.AdicionarCabecalho("Nº da Carga", "Carga", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Nº do Booking", "Booking", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tomador", "NomeNavio", 20, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho("Tipo Tomador", "TipoTomador", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Remetente", "Remetente", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Destinatário", "Destinatario", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Nº do Conteiner", "Containeres", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Qtd. Cntr na Carga", "QtdCntrCarga", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Modal", "Modal", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Porto Origem", "PortoOrigem", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Porto Destino", "PortoDestino", 20, Models.Grid.Align.left, false);
                
                grid.setarQuantidadeTotal(detalhesDashboardDocumentacoes.Count());

                var retorno = (from obj in detalhesDashboardDocumentacoes
                               select new
                               {
                                   obj.LinkCarga,
                                   obj.CodigoCarga,
                                   obj.Booking,
                                   obj.NomeNavio,
                                   obj.Containeres,
                                   obj.Carga,
                                   obj.PortoOrigem,
                                   obj.PortoDestino,
                                   obj.QtdCntrCarga,
                                   obj.Modal,
                                   obj.TipoTomador,
                                   obj.Remetente,
                                   obj.Destinatario
                               }).ToList();

                grid.AdicionaRows(retorno);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterUsuarioLogado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.Entidades.Usuario usuario = this.Usuario;

                var retorno = new
                {
                    UsuarioLogadoOperadorNorte = usuario.RegiaoAcessoDashOperadorNorte,
                    UsuarioLogadoOperadorSul = usuario.RegiaoAcessoDashOperadorSul,
                    UsuarioLogadoOperadorNordeste = usuario.RegiaoAcessoDashOperadorNordeste,
                    UsuarioLogadoOperadorCentroOeste = usuario.RegiaoAcessoDashOperadorCentroOeste,
                    UsuarioLogadoOperadorSudeste = usuario.RegiaoAcessoDashOperadorSudeste,
                    NomeUsuario = usuario.Nome
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar usuário.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarDashboardDocumentacao()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.BI.PermissaoAcessoUsuarioBI repositorioPermissaoAcessoUsuarioBI = new Repositorio.Embarcador.BI.PermissaoAcessoUsuarioBI(unidadeTrabalho);

                IList<Dominio.ObjetosDeValor.Embarcador.Dashboard.DashboardDocumentacao> dashboardDocumentacaos = null;
                dashboardDocumentacaos = repositorioPermissaoAcessoUsuarioBI.BuscarDashboardDocumentacao();

                //var totais = ObterTotais(dashboardDocumentacaos);

                //var retorno = new
                //{
                //    //QuantidadeNavioFechado = dashboardDocumentacaos.Count(item => !item.NavioAberto),
                //    //QuantidadeNavioAberto = dashboardDocumentacaos.Count(item => item.NavioAberto),
                //    TotalSvm = totais.totalSvm,
                //    TotalCargas = totais.totalCarga,
                //    TotalMercante = totais.totalMercante,
                //    DadosDocumentacaoDash = dashboardDocumentacaos?.ToArray()
                //};

                return new JsonpResult(null);
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

        #endregion

        #region Métodos Privados

        private void ObterTotais(IList<Dominio.ObjetosDeValor.Embarcador.Dashboard.DashboardDocumentacao> dashboardDocumentacaos)
        {
            foreach (var itemDash in dashboardDocumentacaos)
            {
                itemDash.TotalCarga = itemDash.TotalCargaEmissao + itemDash.TotalCargaErro + itemDash.TotalCargaGerada + itemDash.TotalCargaPedentes;
                itemDash.TotalSvm = itemDash.TotalSvmErro + itemDash.TotalSvmGerado + itemDash.TotalSvmPendente;
                itemDash.TotalMercante = itemDash.TotalMercantePendente + itemDash.TotalMercanteRetornado;
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Dashboard.FiltroPesquisaDashboardDocumentacao ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Dashboard.FiltroPesquisaDashboardDocumentacao()
            {
                DataInicial = Request.GetDateTimeParam("DataInicial"),
                DataFinal = Request.GetDateTimeParam("DataFinal"),
                Regiao = Request.GetStringParam("Regiao"),
                Embarques = Request.GetListParam<string>("Embarque"),
                NavioAberto = Request.GetBoolParam("NavioAberto"),
                NavioFechado = Request.GetBoolParam("NavioFechado")
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Dashboard.FiltroPesquisaDetalhesDashboardDocumentacao ObterFiltrosPesquisaDetalhes()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Dashboard.FiltroPesquisaDetalhesDashboardDocumentacao()
            {
                StatusCarga = Request.GetStringParam("StatusCarga"),
                StatusSvm = Request.GetStringParam("StatusSvm"),
                StatusMercante = Request.GetStringParam("StatusMercante"),
                NomeNavio = Request.GetStringParam("NomeNavio")
            };
        }

        #endregion
    }
}
