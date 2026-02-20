using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.RH
{
    [CustomAuthorize("RH/TabelaMediaModeloPeso")]
    public class TabelaMediaModeloPesoController : BaseController
    {
		#region Construtores

		public TabelaMediaModeloPesoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoModelo = Request.GetIntParam("Modelo");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Modelo", "Modelo", 40, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Média ideal", "MediaIdeal", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Peso Inicial", "PesoInicial", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Peso Final", "PesoFinal", 10, Models.Grid.Align.left, true);

                Repositorio.Embarcador.RH.TabelaMediaModeloPeso repTabelaMediaModeloPeso = new Repositorio.Embarcador.RH.TabelaMediaModeloPeso(unitOfWork);
                List<Dominio.Entidades.Embarcador.RH.TabelaMediaModeloPeso> informacoesFolha = repTabelaMediaModeloPeso.Consultar(codigoModelo, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repTabelaMediaModeloPeso.ContarConsulta(codigoModelo));

                var lista = (from p in informacoesFolha
                             select new
                             {
                                 p.Codigo,
                                 Modelo = p.Modelo?.Descricao ?? "",
                                 MediaIdeal = p.MediaIdeal.ToString("n2"),
                                 PesoInicial = p.PesoInicial.ToString("n2"),
                                 PesoFinal = p.PesoFinal.ToString("n2")
                             }).ToList();

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
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.RH.TabelaMediaModeloPeso repTabelaMediaModeloPeso = new Repositorio.Embarcador.RH.TabelaMediaModeloPeso(unitOfWork);
                Dominio.Entidades.Embarcador.RH.TabelaMediaModeloPeso tabelaMediaModeloPeso = new Dominio.Entidades.Embarcador.RH.TabelaMediaModeloPeso();

                PreencherTabelaMediaModeloPeso(tabelaMediaModeloPeso, unitOfWork);
                repTabelaMediaModeloPeso.Inserir(tabelaMediaModeloPeso, Auditado);

                if (tabelaMediaModeloPeso.PesoFinal < tabelaMediaModeloPeso.PesoInicial)
                    return new JsonpResult(false, true, "Peso final não pode ser menor que o inicial.");

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

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.RH.TabelaMediaModeloPeso repTabelaMediaModeloPeso = new Repositorio.Embarcador.RH.TabelaMediaModeloPeso(unitOfWork);
                Dominio.Entidades.Embarcador.RH.TabelaMediaModeloPeso tabelaMediaModeloPeso = repTabelaMediaModeloPeso.BuscarPorCodigo(codigo, true);

                PreencherTabelaMediaModeloPeso(tabelaMediaModeloPeso, unitOfWork);
                repTabelaMediaModeloPeso.Atualizar(tabelaMediaModeloPeso, Auditado);

                if (tabelaMediaModeloPeso.PesoFinal < tabelaMediaModeloPeso.PesoInicial)
                    return new JsonpResult(false, true, "Peso final não pode ser menor que o inicial.");

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
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.RH.TabelaMediaModeloPeso repTabelaMediaModeloPeso = new Repositorio.Embarcador.RH.TabelaMediaModeloPeso(unitOfWork);
                Dominio.Entidades.Embarcador.RH.TabelaMediaModeloPeso tabelaMediaModeloPeso = repTabelaMediaModeloPeso.BuscarPorCodigo(codigo, false);

                var dynTabelaMediaModeloPeso = new
                {
                    tabelaMediaModeloPeso.Codigo,
                    Modelo = tabelaMediaModeloPeso.Modelo != null ? new { tabelaMediaModeloPeso.Modelo.Codigo, tabelaMediaModeloPeso.Modelo.Descricao } : null,
                    MediaIdeal = tabelaMediaModeloPeso.MediaIdeal.ToString("n2"),
                    PesoInicial = tabelaMediaModeloPeso.PesoInicial.ToString("n2"),
                    PesoFinal = tabelaMediaModeloPeso.PesoFinal.ToString("n2")
                };

                return new JsonpResult(dynTabelaMediaModeloPeso);
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
                Repositorio.Embarcador.RH.TabelaMediaModeloPeso repTabelaMediaModeloPeso = new Repositorio.Embarcador.RH.TabelaMediaModeloPeso(unitOfWork);
                Dominio.Entidades.Embarcador.RH.TabelaMediaModeloPeso tabelaMediaModeloPeso = repTabelaMediaModeloPeso.BuscarPorCodigo(codigo, true);

                if (tabelaMediaModeloPeso == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                repTabelaMediaModeloPeso.Deletar(tabelaMediaModeloPeso, Auditado);
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

        #region Métodos Privados

        private void PreencherTabelaMediaModeloPeso(Dominio.Entidades.Embarcador.RH.TabelaMediaModeloPeso tabelaMediaModeloPeso, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.ModeloVeiculo repModeloVeiculo = new Repositorio.ModeloVeiculo(unitOfWork);

            int.TryParse(Request.Params("Modelo"), out int modelo);
                        
            tabelaMediaModeloPeso.Modelo = modelo > 0 ? repModeloVeiculo.BuscarPorCodigo(modelo, false) : null;
            tabelaMediaModeloPeso.MediaIdeal = Request.GetDecimalParam("MediaIdeal");
            tabelaMediaModeloPeso.PesoInicial = Request.GetDecimalParam("PesoInicial");
            tabelaMediaModeloPeso.PesoFinal = Request.GetDecimalParam("PesoFinal");
        }

        #endregion
    }
}
