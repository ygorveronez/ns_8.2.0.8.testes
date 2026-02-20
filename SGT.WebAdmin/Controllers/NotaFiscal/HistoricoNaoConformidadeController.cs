using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.NotaFiscal
{
    [CustomAuthorize("NotaFiscal/HistoricoNaoConformidade")]
    public class HistoricoNaoConformidadeController : BaseController
    {
		#region Construtores

		public HistoricoNaoConformidadeController(Conexao conexao) : base(conexao) { }

		#endregion

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.NotaFiscal.ItemNaoConformidade repositorioItemNaoConformidade = new Repositorio.Embarcador.NotaFiscal.ItemNaoConformidade(unitOfWork);
                Repositorio.Embarcador.NotaFiscal.NaoConformidade repositorioNaoConformidade = new Repositorio.Embarcador.NotaFiscal.NaoConformidade(unitOfWork);
                Repositorio.Embarcador.Pedidos.XMLNotaFiscal repNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.NotaFiscal.FiltroPesquisaHistoricoNaoConformidade filtrosPesquisa = ObterFiltrosPesquisaHistoricoNaoConformidade();
                Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = repNotaFiscal.BuscarPorNumero(filtrosPesquisa.NotaFiscal.ToInt());
                List<Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidade> naoConformidade = repositorioItemNaoConformidade.BuscarPorNotaFiscalComNaoConformidadePendente(filtrosPesquisa.NotaFiscal.ToInt());
                Dominio.Entidades.Embarcador.Cargas.Carga carga = naoConformidade.Count > 0 ? repositorioNaoConformidade.BuscarCargaPorItemNaoConformidadeENota(naoConformidade.FirstOrDefault().Codigo, filtrosPesquisa.NotaFiscal.ToInt()) : null;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Item", "Item", 40, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Descrição", "Descricao", 40, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Bloqueio CTE", "BloqueioCTE", 40, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Situacao", "Situacao", 40, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Número da Carga", "NumeroCarga", 40, Models.Grid.Align.left);

                List<dynamic> linhas = new List<dynamic>();
                if (xmlNotaFiscal == null)
                {
                    grid.AdicionaRows(linhas);
                    grid.setarQuantidadeTotal(0);
                    return new JsonpResult(grid);
                }

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                foreach (TipoRegraNaoConformidade tipo in Enum.GetValues(typeof(TipoRegraNaoConformidade)))
                {
                    linhas.Add(
                            new
                            {
                                Codigo = Guid.NewGuid().ToString().Replace("-", ""),
                                Item = (int)tipo,
                                Descricao = tipo.ObterDescricao(),
                                BloqueioCTE = naoConformidade.Where(o => o.TipoRegra == tipo).Select(o => o.PermiteContingencia).FirstOrDefault() ? "Sim" : "Não",
                                Situacao = naoConformidade.Any(o => o.TipoRegra == tipo) ? "Não Ok" : "Ok",
                                NumeroCarga = carga?.CodigoCargaEmbarcador ?? string.Empty
                            }
                        );
                };

                grid.AdicionaRows(linhas);
                grid.setarQuantidadeTotal(Enum.GetValues(typeof(TipoRegraNaoConformidade)).Length);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.NotaFiscal.FiltroPesquisaHistoricoNaoConformidade ObterFiltrosPesquisaHistoricoNaoConformidade()
        {
            return new Dominio.ObjetosDeValor.Embarcador.NotaFiscal.FiltroPesquisaHistoricoNaoConformidade()
            {
                NotaFiscal = Request.GetStringParam("NotaFiscal")
            };
        }
    }
}
