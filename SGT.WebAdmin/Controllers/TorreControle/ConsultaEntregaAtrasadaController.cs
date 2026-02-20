using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;


namespace SGT.WebAdmin.Controllers.TorreControle
{
    public class ConsultaEntregaAtrasadaController : BaseController
    {
		#region Construtores

		public ConsultaEntregaAtrasadaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = ObterGridPesquisa(unitOfWork);

                (IList<Dominio.ObjetosDeValor.Embarcador.TorreControle.ConsultaEntregaAtrasada> Retorno, int TotalRegistros) retorno = ExecutarPesquisa(grid.ObterParametrosConsulta(), unitOfWork);

                grid.AdicionaRows(retorno.Retorno);
                grid.setarQuantidadeTotal(retorno.TotalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExportarPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = ObterGridPesquisa(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                parametrosConsulta.InicioRegistros = 0;
                parametrosConsulta.LimiteRegistros = 0;

                (IList<Dominio.ObjetosDeValor.Embarcador.TorreControle.ConsultaEntregaAtrasada> Retorno, int TotalRegistros) retorno = ExecutarPesquisa(parametrosConsulta, unitOfWork);

                grid.AdicionaRows(retorno.Retorno);
                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo == null)
                    throw new Exception(Localization.Resources.Cargas.ControleEntrega.FalhaAoExportarPesquisa);

                return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.ControleEntrega.OcorreuFalhaAoExportarPesquisa);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AdicionarResponsavelEntregaAtraso()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int CodigoEntrega = Request.GetIntParam("CodigoEntrega");
                int TipoReponsavel = Request.GetIntParam("TipoResponsavel");
                string Observacao = Request.GetStringParam("Observacao");

                if (CodigoEntrega > 0 && TipoReponsavel > 0)
                {
                    Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                    Repositorio.Embarcador.Cargas.ControleEntrega.TipoResponsavelAtrasoEntrega repositorioTipoResponsavelEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.TipoResponsavelAtrasoEntrega(unitOfWork);
                    Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repositorioCargaEntrega.BuscarPorCodigo(CodigoEntrega);
                  
                    unitOfWork.Start();

                    cargaEntrega.ObservacaoResponsavelAtraso = Observacao;
                    cargaEntrega.TipoResponsavelAtrasoEntrega = repositorioTipoResponsavelEntrega.BuscarPorCodigo(TipoReponsavel, false);

                    repositorioCargaEntrega.Atualizar(cargaEntrega);
                    Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SincronizarEntregaOrigem(cargaEntrega, repositorioCargaEntrega, unitOfWork);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaEntrega, Localization.Resources.Cargas.ControleEntrega.AdicionouResposavelAtrasoEntrega, unitOfWork);

                    unitOfWork.CommitChanges();
                    return new JsonpResult(true);

                }
                else
                    return new JsonpResult(false, Localization.Resources.Cargas.ControleEntrega.EntregaNaoLocalizada);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoAdicionarDados);
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }



        #endregion

        #region Métodos Privados

        private Models.Grid.Grid ObterGridPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("CodigoEntrega", false);
            grid.AdicionarCabecalho("TipoResponsavel", false);

            grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.Carga, "Carga", 20, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.TipoOperacao, "TipoOperacao", 20, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.Cliente, "Cliente", 20, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.Transportador, "Transportador", 20, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.DataDeAgendamento, "DataAgendamentoFormatada", 10, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.DataConfirmacaoEntrega, "DataConfirmacaoEntregaFormatada", 10, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.TipoResponsavel, "DescricaoResponsavel", 10, Models.Grid.Align.center, false);

            Models.Grid.GridPreferencias gridPreferencias = new Models.Grid.GridPreferencias(unitOfWork, "ConsultaEntregaAtrasada/Pesquisa", "grid-consulta-entrega_atrasada");
            grid.AplicarPreferenciasGrid(gridPreferencias.ObterPreferenciaGrid(this.Usuario.Codigo, grid.modelo));

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaConsultaEntregaAtrasada ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaConsultaEntregaAtrasada()
            {
                Transportador = Request.GetIntParam("Transportador"),
                DataPrevisaoEntregaInicial = Request.GetNullableDateTimeParam("DataPrevisaoEntregaInicial"),
                DataPrevisaoEntregaFinal = Request.GetNullableDateTimeParam("DataPrevisaoEntregaFinal"),
                NumeroCarga = Request.GetStringParam("NumeroCarga"),
                NumeroNota = Request.GetIntParam("NumeroNota"),
                TipoOperacao = Request.GetIntParam("TipoOperacao"),
                Cliente = Request.GetDoubleParam("Cliente")
            };
        }

        private (IList<Dominio.ObjetosDeValor.Embarcador.TorreControle.ConsultaEntregaAtrasada> Retorno, int TotalRegistros) ExecutarPesquisa(Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.TorreControle.ConsultaPorEntrega repositorioConsultaPorEntrega = new Repositorio.Embarcador.TorreControle.ConsultaPorEntrega(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaConsultaEntregaAtrasada filtrosPesquisa = ObterFiltrosPesquisa();

            int totalRegistros = repositorioConsultaPorEntrega.ContarConsultaEntregaAtrasada(filtrosPesquisa);
            IList<Dominio.ObjetosDeValor.Embarcador.TorreControle.ConsultaEntregaAtrasada> listaRetorno = totalRegistros > 0 ? repositorioConsultaPorEntrega.ConsultarEntregaAtrasada(filtrosPesquisa, parametrosConsulta) : new List<Dominio.ObjetosDeValor.Embarcador.TorreControle.ConsultaEntregaAtrasada>();

            return (listaRetorno, totalRegistros);
        }


        #endregion
    }

}
