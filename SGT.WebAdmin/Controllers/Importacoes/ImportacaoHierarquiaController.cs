using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Importacoes
{
    public class ImportacaoHierarquiaController : BaseController
    {
		#region Construtores

		public ImportacaoHierarquiaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Públicos

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            
            try
            {
                return new JsonpResult(ObterGridPesquisa(unitOfWork));
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

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Importacoes.FiltroPesquisaImportacaoHierarquia ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Importacoes.FiltroPesquisaImportacaoHierarquia
            {
                DataInicial = Request.GetNullableDateTimeParam("DataInicial"),
                DataFinal = Request.GetNullableDateTimeParam("DataFinal")
            };
        }

        private Models.Grid.Grid ObterGridPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Nome Arquivo", "NomeArquivo", 20, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.center, false, true);
            grid.AdicionarCabecalho("Quantidade Importada", "QuantidadeRegistrosImportados", 20, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("Quantidade Total", "QuantidadeRegistrosTotal", 20, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("Descrição", "Descricao", 20, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Tipo Arquivo", "TipoArquivo", 20, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Situação", "Situacao", 20, Models.Grid.Align.center, false, true);
            
            PreencherGridPesquisa(grid, unitOfWork);

            return grid;
        }
        
        private void PreencherGridPesquisa(Models.Grid.Grid grid, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Importacoes.ImportacaoHierarquiaHistorico repositorio = new Repositorio.Embarcador.Importacoes.ImportacaoHierarquiaHistorico(unitOfWork);
            dynamic filtrosPesquisa = ObterFiltrosPesquisa();
            
            int totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);
            List<Dominio.Entidades.Embarcador.Importacoes.ImportacaoHierarquiaHistorico> listaRegistros = totalRegistros > 0 ? repositorio.Consultar(filtrosPesquisa, grid.ObterParametrosConsulta()) : new List<Dominio.Entidades.Embarcador.Importacoes.ImportacaoHierarquiaHistorico>();

            dynamic listaRetorno = (from o in listaRegistros
                                    select new
                                    {
                                        o.Codigo,
                                        o.NomeArquivo,
                                        Data = o.Data.ToString("dd/MM/yyyy HH:mm"),
                                        o.QuantidadeRegistrosImportados,
                                        o.QuantidadeRegistrosTotal,
                                        o.Descricao,
                                        o.TipoArquivo,
                                        Situacao = o.Situacao.ObterDescricao()
                                    }).ToList();

            grid.setarQuantidadeTotal(totalRegistros);
            grid.AdicionaRows(listaRetorno);
        }

        #endregion
    }
}
