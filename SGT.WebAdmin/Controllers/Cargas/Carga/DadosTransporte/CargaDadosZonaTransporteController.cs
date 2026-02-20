using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.Carga.DadosTransporte
{
    [CustomAuthorize("Cargas/Carga")]
    public class CargaDadosZonaTransporteController : BaseController
    {
		#region Construtores

		public CargaDadosZonaTransporteController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> ObterZonasTransportes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoCarga = Request.GetIntParam("Carga");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoCarga", false);
                grid.AdicionarCabecalho("Sequência", "Sequencia", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Zona de Transporte", "ZonaTransporte", 30, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Peso Totais dos Pedidos", "PesoTotalPedido", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Cubagem Totais dos Pedidos", "CubagemTotalPedido", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Valores de Mercadoria Totais dos Pedidos", "ValorMercadoriaPedido", 20, Models.Grid.Align.left, false);

                Repositorio.Embarcador.Cargas.CargaZonaTransporte repositorioCargaZonaTransporte = new Repositorio.Embarcador.Cargas.CargaZonaTransporte(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Entidades.Embarcador.Cargas.CargaZonaTransporte> listaCargaZonaTransporte = repositorioCargaZonaTransporte.Consultar(codigoCarga, parametrosConsulta);
                grid.setarQuantidadeTotal(repositorioCargaZonaTransporte.ContarConsulta(codigoCarga));

                var lista = (from p in listaCargaZonaTransporte
                             select new
                             {
                                 p.Codigo,
                                 CodigoCarga = p.Carga.Codigo,
                                 p.Sequencia,
                                 ZonaTransporte = p.ZonaTransporte?.Descricao ?? string.Empty,
                                 PesoTotalPedido = p.PesoTotalPedido.ToString("n2"),
                                 CubagemTotalPedido = p.CubagemTotalPedido.ToString("n2"),
                                 ValorMercadoriaPedido = p.ValorMercadoriaPedido.ToString("n2")
                             }).ToList();

                grid.AdicionaRows(lista);
                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar as Zonas de Transporte.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReordenarSequenciaZonasTransporte()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigoCarga = Request.GetIntParam("Carga");
                int codigo = Request.GetIntParam("Codigo");
                int novaSequencia = Request.GetIntParam("NovaSequencia");

                Repositorio.Embarcador.Cargas.CargaZonaTransporte repositorioCargaZonaTransporte = new Repositorio.Embarcador.Cargas.CargaZonaTransporte(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga, false);
                if (carga == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro da Carga.");

                if (carga.SituacaoCarga != SituacaoCarga.Nova && carga.SituacaoCarga != SituacaoCarga.CalculoFrete && carga.SituacaoCarga != SituacaoCarga.AgTransportador && carga.SituacaoCarga != SituacaoCarga.AgNFe)
                    return new JsonpResult(false, true, "Não é possível alterar na atual situação da carga.");

                Dominio.Entidades.Embarcador.Cargas.CargaZonaTransporte cargaZonaTransporte = repositorioCargaZonaTransporte.BuscarPorCodigo(codigo, false);
                if (cargaZonaTransporte == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro da Zona de Transporte.");

                if (novaSequencia == 0 || cargaZonaTransporte.Sequencia == 0)
                    return new JsonpResult(false, true, "Sequência está zerada, não sendo possível alterar.");

                if (novaSequencia > cargaZonaTransporte.Sequencia)
                    repositorioCargaZonaTransporte.AtualizarPrioridadesInferiores(carga.Codigo, novaSequencia, cargaZonaTransporte.Sequencia);
                else
                    repositorioCargaZonaTransporte.AtualizarPrioridadesSuperiores(carga.Codigo, novaSequencia, cargaZonaTransporte.Sequencia);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, $"Alterada a sequência da zona de transporte {cargaZonaTransporte.ZonaTransporte.Descricao} de {cargaZonaTransporte.Sequencia} para {novaSequencia}", unitOfWork);

                cargaZonaTransporte.Sequencia = novaSequencia;
                repositorioCargaZonaTransporte.Atualizar(cargaZonaTransporte);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao reordenar as sequências.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
