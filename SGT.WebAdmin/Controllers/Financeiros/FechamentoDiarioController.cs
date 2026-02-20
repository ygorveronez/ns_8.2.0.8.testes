using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Financeiros
{
    [CustomAuthorize("Financeiros/FechamentoDiario")]
    public class FechamentoDiarioController : BaseController
    {
		#region Construtores

		public FechamentoDiarioController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                DateTime.TryParseExact(Request.Params("DataFechamentoInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataFechamentoInicial);
                DateTime.TryParseExact(Request.Params("DataFechamentoFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataFechamentoFinal);
                int codigoEmpresa = Request.GetIntParam("Empresa");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data Fechamento", "DataFechamento", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Usuário", "Usuario", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Empresa", "Empresa", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Bloquear apenas Documento de Entrada", "DescricaoBloquearApenasDocumentoEntrada", 30, Models.Grid.Align.left, false);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdena == "Usuario")
                    propOrdena = "Usuario.Nome";

                Repositorio.Embarcador.Financeiro.FechamentoDiario repFechamentoDiario = new Repositorio.Embarcador.Financeiro.FechamentoDiario(unitOfWork);

                List<Dominio.Entidades.Embarcador.Financeiro.FechamentoDiario> listaFechamentoDiario = repFechamentoDiario.Consultar(codigoEmpresa, dataFechamentoInicial, dataFechamentoFinal, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);

                grid.setarQuantidadeTotal(repFechamentoDiario.ContarConsulta(codigoEmpresa, dataFechamentoInicial, dataFechamentoFinal));

                var lista = (from p in listaFechamentoDiario
                             select new
                             {
                                 p.Codigo,
                                 DataFechamento = p.DataFechamento.ToString("dd/MM/yyyy"),
                                 Usuario = p.Usuario.Nome,
                                 Empresa = p.Empresa?.Descricao ?? "",
                                 DescricaoBloquearApenasDocumentoEntrada = p.BloquearApenasDocumentoEntrada ? "Sim" : "Não"
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
                DateTime dataFechamento = Request.GetDateTimeParam("DataFechamento");
                bool bloquearApenasDocumentoEntrada = Request.GetBoolParam("BloquearApenasDocumentoEntrada");
                int codigoEmpresa = Request.GetIntParam("Empresa");

                string erro = string.Empty;

                if (!Servicos.Embarcador.Financeiro.FechamentoDiario.RealizarFechamento(out erro, codigoEmpresa, dataFechamento, bloquearApenasDocumentoEntrada, Usuario, Auditado, unitOfWork))
                    return new JsonpResult(false, true, erro);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Excluir()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                string erro = string.Empty;

                Repositorio.Embarcador.Financeiro.FechamentoDiario repFechamentoDiario = new Repositorio.Embarcador.Financeiro.FechamentoDiario(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.FechamentoDiario fechamentoDiario = repFechamentoDiario.BuscarPorCodigo(codigo, true);

                if (!Servicos.Embarcador.Financeiro.FechamentoDiario.ReabrirFechamento(out erro, fechamentoDiario, Auditado, unitOfWork))
                    return new JsonpResult(false, true, erro);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
