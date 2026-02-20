using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.TorreControle
{
    public class ConsultaPorNotaFiscalController : BaseController
    {
		#region Construtores

		public ConsultaPorNotaFiscalController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = ObterGridPesquisa(unitOfWork);

                (IList<Dominio.ObjetosDeValor.Embarcador.TorreControle.ConsultaPorNotaFiscal> Retorno, int TotalRegistros) retorno = ExecutarPesquisa(grid.ObterParametrosConsulta(), unitOfWork);

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

                (IList<Dominio.ObjetosDeValor.Embarcador.TorreControle.ConsultaPorNotaFiscal> Retorno, int TotalRegistros) retorno = ExecutarPesquisa(parametrosConsulta, unitOfWork);

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

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid ObterGridPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };
            
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.NotaFiscal, "Numero", 20, Models.Grid.Align.right, false);
            grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Carga, "Carga", 20, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.SituacaoNFe, "SituacaoNotaFiscal", 20, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.TipoOperacao, "TipoOperacao", 20, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.SituacaoAgendamento , "SituacaoAgendamentoDescricao", 20, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.OberservacaoReeagendamento , "ObservacaoReagendamento", 20, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.Cliente, "ClienteDescricao", 20, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Destino, "Destino", 20, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Transportador, "TransportadorDescricao", 20, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.SugestaoDataEntrega , "SugestaoDataEntregaFormatada", 20, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.DataAgendamento , "DataAgendamentoFormatada", 20, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.DataReagendamento , "DataReagendamentoFormatada", 20, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.DataTerminoCarregamento, "DataTerminoCarregamentoFormatada", 20, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.DataInicioEntrega, "DataInicioEntregaFormatada", 20, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.DataFimEntrega , "DataFimEntregaFormatada", 20, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.SituacaoViagem, "SituacaoViagem", 20, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.ContatoCliente , "ContatoCliente", 20, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.ContatoTransportador , "ContatoTransportador", 20, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.Ocorrencia, "Ocorrencia", 20, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.SituacaoEntrega, "SituacaoEntregaDescricao", 20, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.SituacaoEntregaNFe, "SituacaoEntregaNotaFiscalDescricao", 20, Models.Grid.Align.left, false);

            Models.Grid.GridPreferencias gridPreferencias = new Models.Grid.GridPreferencias(unitOfWork, "ConsultaPorNotaFiscal/Pesquisa", "grid-consulta-por-nota-fiscal");
            grid.AplicarPreferenciasGrid(gridPreferencias.ObterPreferenciaGrid(this.Usuario.Codigo, grid.modelo));

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaConsultaPorNotaFiscal ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaConsultaPorNotaFiscal()
            {
                Transportador = Request.GetIntParam("Transportador"),
                DataCarregamentoInicial = Request.GetNullableDateTimeParam("DataCarregamentoInicial"),
                DataCarregamentoFinal = Request.GetNullableDateTimeParam("DataCarregamentoFinal"),
                DataPrevisaoEntregaInicial = Request.GetNullableDateTimeParam("DataPrevisaoEntregaInicial"),
                DataPrevisaoEntregaFinal = Request.GetNullableDateTimeParam("DataPrevisaoEntregaFinal"),
                DataAgendamentoInicial = Request.GetNullableDateTimeParam("DataAgendamentoInicial"),
                DataAgendamentoFinal = Request.GetNullableDateTimeParam("DataAgendamentoFinal"),
                NumeroCarga = Request.GetStringParam("NumeroCarga"),
                NumeroNota = Request.GetIntParam("NumeroNota"),
                SituacaoAgendamento = Request.GetNullableEnumParam<SituacaoAgendamentoEntregaPedido>("SituacaoAgendamento"),
                TipoOperacao = Request.GetIntParam("TipoOperacao"),
                Cliente = Request.GetDoubleParam("Cliente")
            };
        }

        private (IList<Dominio.ObjetosDeValor.Embarcador.TorreControle.ConsultaPorNotaFiscal> Retorno, int TotalRegistros) ExecutarPesquisa(Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.TorreControle.ConsultaPorNotaFiscal repositorioConsultaPorNotaFiscal = new Repositorio.Embarcador.TorreControle.ConsultaPorNotaFiscal(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaConsultaPorNotaFiscal filtrosPesquisa = ObterFiltrosPesquisa();

            int totalRegistros = repositorioConsultaPorNotaFiscal.ContarConsulta(filtrosPesquisa, parametrosConsulta);
            IList<Dominio.ObjetosDeValor.Embarcador.TorreControle.ConsultaPorNotaFiscal> listaRetorno = totalRegistros > 0 ? repositorioConsultaPorNotaFiscal.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.ObjetosDeValor.Embarcador.TorreControle.ConsultaPorNotaFiscal>();

            return (listaRetorno, totalRegistros);
        }

        #endregion
    }
}
