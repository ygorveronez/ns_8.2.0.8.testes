using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.TorreControle
{
    public class ConsultaPorEntregaController : BaseController
    {
		#region Construtores

		public ConsultaPorEntregaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                return new JsonpResult(ObterGridPesquisa());
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        #endregion
        #region Métodos Privados
        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Carga, "Carga", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.Pedidos, "Pedidos", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.Notas , "Notas", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.Status , "Status", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.Ocorrencia, "Ocorrencia", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.CidadeOrigem, "CidadeOrigem", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.Cliente, "Cliente", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.CidadeDestino, "CidadeDestino", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.DataCriacaoCarga , "DataCriacaoCarga", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.DataCarregamento , "DataCarregamento", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.DataPrevisaoEntrega , "DataPrevisaoEntrega", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.DataEntregaReprogramada , "DataEntregaReprogramada", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.Operacao, "Operacao", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.Veiculo, "Veiculo", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Motorista, "Motorista", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Transportador, "Transportador", 20, Models.Grid.Align.left, true);

                Models.Grid.GridPreferencias gridPreferencias = new Models.Grid.GridPreferencias(unitOfWork, "ConsultaPorEntrega/Pesquisa", "gridConsultaPorEntrega");
                grid.AplicarPreferenciasGrid(gridPreferencias.ObterPreferenciaGrid(this.Usuario.Codigo, grid.modelo));

                string propriedadeOrdenar = ObterPropriedadeOrdenar(grid.header[grid.indiceColunaOrdena].data);

                Repositorio.Embarcador.TorreControle.ConsultaPorEntrega repTorreControle = new Repositorio.Embarcador.TorreControle.ConsultaPorEntrega(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaConsultaPorEntrega filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                IList<Dominio.ObjetosDeValor.Embarcador.TorreControle.ConsultaPorEntrega> listaRetiradaProduto = repTorreControle.Consultar(filtrosPesquisa, parametrosConsulta);
                int totalRegistros = repTorreControle.ContarConsulta(filtrosPesquisa, parametrosConsulta);

                grid.AdicionaRows(listaRetiradaProduto);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch(Exception ex)
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            return propriedadeOrdenar;
        }

        private Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaConsultaPorEntrega ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaConsultaPorEntrega()
            {
                Transportador = Request.GetIntParam("Transportador"),
                DataCriacaoCargaInicial = Request.GetDateTimeParam("DataCriacaoCargaInicial"),
                DataCriacaoCargaFinal = Request.GetDateTimeParam("DataCriacaoCargaFinal"),
                DataPrevisaoEntregaInicial = Request.GetDateTimeParam("DataPrevisaoEntregaInicial"),
                DataPrevisaoEntregaFinal = Request.GetDateTimeParam("DataPrevisaoEntregaFinal"),
                NumeroCarga = Request.GetStringParam("NumeroCarga"),
                NumeroNota = Request.GetStringParam("NumeroNota"),
                Operacao = Request.GetIntParam("Operacao"),
                Placa = Request.GetStringParam("Placa"),
                Status = Request.GetStringParam("Status"),
            };
        }
    }
    #endregion
}
