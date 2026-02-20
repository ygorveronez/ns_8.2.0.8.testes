using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.ControleEntrega
{
    [CustomAuthorize("Cargas/ManutencaoEntregaCarga")]
    public class ManutencaoEntregaCargaController : BaseController
    {
		#region Construtores

		public ManutencaoEntregaCargaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("Carga");

                Models.Grid.EditableCell editableDataEntradaRaio = new Models.Grid.EditableCell(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoColunaGrid.aDateTime);
                Models.Grid.EditableCell editableSaidaRaio = new Models.Grid.EditableCell(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoColunaGrid.aDateTime);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Ordem", "Ordem", 5, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Carga", "CodigoCargaEmbarcador", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Cliente", "Cliente", 25, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Notas Fiscais", "NotasFiscais", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Data Chegada", "DataEntradaRaio", 10, Models.Grid.Align.center, true, false, false, false, true, editableDataEntradaRaio);
                grid.AdicionarCabecalho("Data Saída", "DataSaidaRaio", 10, Models.Grid.Align.center, true, false, false, false, true, editableSaidaRaio);

                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);

                int count = repCargaEntrega.ContarConsulta(codigoCarga);

                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> lista = repCargaEntrega.Consultar(codigoCarga, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(count);

                var retorno = (from obj in lista
                               select new
                               {
                                   obj.Codigo,
                                   Ordem = obj.Ordem + 1,
                                   obj.Carga.CodigoCargaEmbarcador,
                                   Cliente = obj.Cliente?.Nome,
                                   NotasFiscais = string.Join(", ", obj.NotasFiscais.Select(o => o.PedidoXMLNotaFiscal.XMLNotaFiscal.Numero)),
                                   DataEntradaRaio = obj.DataEntradaRaio?.ToString("dd/MM/yyyy HH:mm"),
                                   DataSaidaRaio = obj.DataSaidaRaio?.ToString("dd/MM/yyyy HH:mm")
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
        public async Task<IActionResult> AtualizarEntregaCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");
                DateTime? dataEntradaRaio = Request.GetNullableDateTimeParam("DataEntradaRaio");
                DateTime? dataSaidaRaio = Request.GetNullableDateTimeParam("DataSaidaRaio");

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repositorioCargaEntrega.BuscarPorCodigo(codigo, true);

                if (cargaEntrega == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (!dataEntradaRaio.HasValue && !dataSaidaRaio.HasValue)
                    return new JsonpResult(false, true, "Nenhuma data foi informada.");

                if (dataEntradaRaio.HasValue && dataSaidaRaio.HasValue && dataEntradaRaio > dataSaidaRaio)
                    return new JsonpResult(false, true, "Data de Chegada não pode ser maior que a de Saída.");

                cargaEntrega.DataEntradaRaio = dataEntradaRaio;
                cargaEntrega.DataSaidaRaio = dataSaidaRaio;
                cargaEntrega.Carga.DataAtualizacaoCarga = DateTime.Now;

                repositorioCarga.Atualizar(cargaEntrega.Carga);
                repositorioCargaEntrega.Atualizar(cargaEntrega, Auditado);
                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SincronizarEntregaOrigem(cargaEntrega, repositorioCargaEntrega, unitOfWork);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar a carga entrega.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
