using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Models.Grid;
using Repositorio;
using Repositorio.Embarcador.Veiculos;
using entidade = Dominio.Entidades.Embarcador.Veiculos;

namespace SGT.WebAdmin.Controllers.Veiculos
{
    [CustomAuthorize("Veiculos/TabelaMediaPorSegmento")]
    public class TabelaMediaPorSegmentoController : BaseController
    {
		#region Construtores

		public TabelaMediaPorSegmentoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            UnitOfWork unitOfWork = new UnitOfWork(_conexao.StringConexao);
            try
            {
                var repoTabelaMediaPorSegmento = new TabelaMediaPorSegmento(unitOfWork);
                List<Dominio.Entidades.Embarcador.Veiculos.TabelaMediaPorSegmento> tabela = new List<Dominio.Entidades.Embarcador.Veiculos.TabelaMediaPorSegmento>();
                int contadorTabelaSegmento = repoTabelaMediaPorSegmento.ContarTodos();
                int codigoModelo = Request.GetIntParam("Segmento");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Segmento", "Segmento", 40, Align.left, true);
                grid.AdicionarCabecalho("Média Inicial", "MediaInicial", 10, Align.left, true);
                grid.AdicionarCabecalho("Media Final", "MediaFinal", 10, Align.left, true);
                grid.AdicionarCabecalho("Percentual", "Percentual", 10, Align.left, true);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                if (contadorTabelaSegmento > 0)
                    tabela = repoTabelaMediaPorSegmento.Consultar(codigoModelo, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);

                var lista = (from p in tabela
                             select new
                             {
                                 p.Codigo,
                                 Segmento = p.Segmento?.Descricao ?? "",
                                 MediaInicial = p.MediaInicial.ToString("n2"),
                                 MediaFinal = p.MediaFinal.ToString("n2"),
                                 Percentual = p.Percentual.ToString("n2")
                             }).ToList();

                grid.setarQuantidadeTotal(contadorTabelaSegmento);
                grid.AdicionaRows(lista);
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

        public async Task<IActionResult> Adicionar()
        {
            UnitOfWork unitOfWork = new UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                TabelaMediaPorSegmento repMediaPorSegmento = new TabelaMediaPorSegmento(unitOfWork);
                var MediaPorSegmento = new entidade.TabelaMediaPorSegmento();

                NovaMediaPorSegmento(MediaPorSegmento, unitOfWork);
                repMediaPorSegmento.Inserir(MediaPorSegmento, Auditado);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
        }

        public async Task<IActionResult> Atualizar()
        {

            UnitOfWork unitOfWork = new UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                var repMediaPorSegmento = new TabelaMediaPorSegmento(unitOfWork);
                var tuplas = repMediaPorSegmento.BuscarPorCodigo(codigo, true);

                NovaMediaPorSegmento(tuplas, unitOfWork);
                repMediaPorSegmento.Atualizar(tuplas, Auditado);

                //TODO 77958: checar alguma restrição

                //if (tuplas.Percentual < tuplas.MediaFinal)
                //{
                //    return new JsonpResult(false, true, "Peso final não pode ser menor que o inicial.");
                //}

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            UnitOfWork unitOfWork = new UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));
                var repoMediaPorSegmento = new TabelaMediaPorSegmento(unitOfWork);
                var tuplas = repoMediaPorSegmento.BuscarPorCodigo(codigo, false);

                var tabela = new
                {
                    tuplas.Codigo,
                    Segmento = tuplas.Segmento != null ? new { tuplas.Segmento.Codigo, tuplas.Segmento.Descricao } : null,
                    MediaInicial = tuplas.MediaInicial.ToString("n2"),
                    MediaFinal = tuplas.MediaFinal.ToString("n2"),
                    Percentual = tuplas.Percentual.ToString("n2")
                };

                return new JsonpResult(tabela);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                var repMediaPorSegmento = new TabelaMediaPorSegmento(unitOfWork);
                var tabelaMediaModeloPeso = repMediaPorSegmento.BuscarPorCodigo(codigo, true);

                if (tabelaMediaModeloPeso == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                repMediaPorSegmento.Deletar(tabelaMediaModeloPeso, Auditado);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Metodos Privados

        private void NovaMediaPorSegmento(entidade.TabelaMediaPorSegmento mediaPorSegmento, UnitOfWork unitOfWork)
        {
            var repoSegmentoVeiculo = new SegmentoVeiculo(unitOfWork);

            int.TryParse(Request.Params("Segmento"), out int segmento);

            mediaPorSegmento.Segmento = segmento > 0 ? repoSegmentoVeiculo.BuscarPorCodigo(segmento, false) : null;
            mediaPorSegmento.MediaInicial = Request.GetDecimalParam("MediaInicial");
            mediaPorSegmento.MediaFinal = Request.GetDecimalParam("MediaFinal");
            mediaPorSegmento.Percentual = Request.GetDecimalParam("Percentual");
        }

        #endregion
    }
}
