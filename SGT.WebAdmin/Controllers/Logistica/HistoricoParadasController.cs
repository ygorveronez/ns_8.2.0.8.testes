using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;
using Utilidades.Extensions;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/HistoricoParadas")]
    public class HistoricoParadasController : BaseController
    {
        #region Construtores

        public HistoricoParadasController(Conexao conexao) : base(conexao) { }

        #endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R227_HistoricoParadas;

        private decimal TamanhoColumaExtraPequena = 1m;
        private decimal TamanhoColunaPequena = 1.75m;
        private decimal TamanhoColunaGrande = 5.50m;
        private decimal TamanhoColunaMedia = 3m;

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorio()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();
                int codigoRelatorio = Request.GetIntParam("Codigo");
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = serRelatorio.BuscarConfiguracaoPadrao(CodigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Histórico de Paradas", "Logistica", "HistoricoParadas.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "", "", codigoRelatorio, unitOfWork, false, false);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                var retorno = gridRelatorio.RetornoGridPadraoRelatorio(GridPadrao(), relatorio);
                unitOfWork.CommitChanges();

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados do relatório.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioMonitoramentoVeiculo filtrosPesquisa = ObterFiltrosPesquisa();

                string mensagem = VerificarCamposObrigatorios(filtrosPesquisa);

                if (!string.IsNullOrEmpty(mensagem))
                    return new JsonpResult(false, false, mensagem);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametros = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametros, string.Empty);
                List<PropriedadeAgrupamento> propriedades = ObterPropriedades();
                List<Dominio.Relatorios.Embarcador.DataSource.Logistica.MonitoramentoParadaVeiculo> listaParadas = Servicos.Embarcador.Monitoramento.MonitoramentoEventoParada.ConsultarRelatorioParada(unitOfWork, filtrosPesquisa, propriedades, parametros, filtrosPesquisa.ApenasMonitoramentosFinalizados);

                grid.setarQuantidadeTotal(listaParadas.Count);
                grid.AdicionaRows(listaParadas);

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

        public async Task<IActionResult> GerarExcel()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string caminhoArquivos = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoArquivo().CaminhoArquivos, "Histórico de Paradas");

                Utilidades.File.RemoverArquivos(caminhoArquivos);

                caminhoArquivos = Utilidades.IO.FileStorageService.Storage.Combine(caminhoArquivos, System.IO.Path.GetRandomFileName());

                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioMonitoramentoVeiculo filtrosPesquisa = ObterFiltrosPesquisa();

                string mensagem = VerificarCamposObrigatorios(filtrosPesquisa);

                if (!string.IsNullOrEmpty(mensagem))
                    return new JsonpResult(false, false, mensagem);

                List<PropriedadeAgrupamento> propriedades = ObterPropriedades();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametros = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta();

                Servicos.Log.TratarErro($"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")} - Iniciou a consulta do histórico de paradas");

                List<Dominio.Relatorios.Embarcador.DataSource.Logistica.MonitoramentoParadaVeiculo> listaParadas = Servicos.Embarcador.Monitoramento.MonitoramentoEventoParada.ConsultarRelatorioParada(unidadeTrabalho, filtrosPesquisa, propriedades, parametros, filtrosPesquisa.ApenasMonitoramentosFinalizados);

                Servicos.Log.TratarErro($"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")} - Iniciou a geração do arquivo de histórico de paradas");

                Utilidades.CSV.GerarCSV<Dominio.Relatorios.Embarcador.DataSource.Logistica.MonitoramentoParadaVeiculo>(listaParadas, caminhoArquivos);

                listaParadas = null;
                GC.Collect();

                Servicos.Log.TratarErro($"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")} - Terminou a geração do arquivo do histórico de paradas");

                return Arquivo(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoArquivos), "plain/text", "Histórico de Paradas - " + DateTime.Now.ToString("dd-MM-yyyy") + ".csv");
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao gerar o relatório.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> GerarPDF()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork);
                Repositorio.Embarcador.Compras.RequisicaoMercadoria repRequisicaoMercadoria = new Repositorio.Embarcador.Compras.RequisicaoMercadoria(unitOfWork);
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");
                string stringConexao = _conexao.StringConexao;

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = repRelatorio.BuscarPadraoPorCodigoControleRelatorio(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R227_HistoricoParadas, TipoServicoMultisoftware);
                if (relatorio == null)
                    relatorio = serRelatorio.BuscarConfiguracaoPadrao(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R227_HistoricoParadas, TipoServicoMultisoftware, "Relatório de Histórico de Paradas", "Logistica", "HistoricoParadas.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "", "", 0, unitOfWork, false, false);

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = serRelatorio.AdicionarRelatorioParaGeracao(relatorio, this.Usuario, Dominio.Enumeradores.TipoArquivoRelatorio.PDF, unitOfWork);

                List<PropriedadeAgrupamento> propriedades = ObterPropriedades();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametros = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta();
                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioMonitoramentoVeiculo filtrosPesquisa = ObterFiltrosPesquisa();

                string mensagem = VerificarCamposObrigatorios(filtrosPesquisa);

                if (!string.IsNullOrEmpty(mensagem))
                    return new JsonpResult(false, false, mensagem);

                List<Dominio.Relatorios.Embarcador.DataSource.Logistica.MonitoramentoParadaVeiculo> listaParadas = Servicos.Embarcador.Monitoramento.MonitoramentoEventoParada.ConsultarRelatorioParada(unitOfWork, filtrosPesquisa, propriedades, parametros, filtrosPesquisa.ApenasMonitoramentosFinalizados);

                if (listaParadas.Count > 0)
                {
                    Task.Factory.StartNew(() => GerarRelatorioHistoricoParadas(stringConexao, relatorioControleGeracao, listaParadas, filtrosPesquisa));
                    return new JsonpResult(true);
                }
                else
                {
                    return new JsonpResult(false, false, "Nenhum registro de histórico de paradas para gerar o relatório.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao gerar o relatório.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void GerarRelatorioHistoricoParadas(string stringConexao, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, List<Dominio.Relatorios.Embarcador.DataSource.Logistica.MonitoramentoParadaVeiculo> dadosHistoricoParadas, Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioMonitoramentoVeiculo filtrosPesquisa)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

            Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
            try
            {
                ReportRequest.WithType(ReportType.HistoricoParada)
                    .WithExecutionType(ExecutionType.Async)
                    .AddExtraData("DadosHistoricoParadas", dadosHistoricoParadas.ToJson())
                    .AddExtraData("FiltrosPesquisa", filtrosPesquisa.ToJson())
                    .AddExtraData("RelatorioControleGeracao", relatorioControleGeracao.Codigo)
                    .CallReport();
            }
            catch (Exception ex)
            {
                serRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, ex);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioMonitoramentoVeiculo ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioMonitoramentoVeiculo()
            {
                CodigoFilial = Request.GetIntParam("Filial"),
                CodigosVeiculo = Request.GetListParam<int>("Veiculos"),
                CodigoTransportador = Request.GetIntParam("Transportador"),
                DataInicial = Request.GetDateTimeParam("DataInicial"),
                DataFinal = Request.GetDateTimeParam("DataFinal").AddDays(1).AddMilliseconds(-1), // Até os últimos milisegundos do dia
                CodigosContratoFrete = Request.GetListParam<int>("ContratoFrete"),
                ApenasMonitoramentosFinalizados = Request.GetBoolParam("ApenasMonitoramentosFinalizados")
            };
        }

        private List<PropriedadeAgrupamento> ObterPropriedades()
        {
            List<PropriedadeAgrupamento> propriedades = new List<PropriedadeAgrupamento>();
            List<string> nomeColunas = new List<string>(new string[] { "Latitude", "Longitude", "Expedicao", "InicioParada", "FimParada", "DuracaoParada", "Transportador", "CodigoVeiculo", "Placa", "Filial", "DescricaoPosicao" });

            foreach (string nome in nomeColunas)
            {
                PropriedadeAgrupamento propriedade = new PropriedadeAgrupamento();
                propriedade.Propriedade = nome;
                propriedades.Add(propriedade);
            }

            return propriedades;
        }

        private List<Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioMonitoramentoVeiculo filtrosPesquisa)
        {
            List<Parametro> parametros = new List<Parametro>();

            parametros.Add(new Parametro("Data", filtrosPesquisa.DataInicial, filtrosPesquisa.DataFinal));

            return parametros;
        }

        private Models.Grid.Grid GridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();

            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Locais", "DescricaoPosicao", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Posição", "LocalidadeAproximada", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Latitude/Longitude", "LatitudeLongitude", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Expedição", "Expedicao", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Situação", "Situacao", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Data Início", "DataInicioFormatada", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Data Fim", "DataFimFormatada", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Tempo", "Tempo", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, true);

            return grid;
        }

        private string VerificarCamposObrigatorios(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioMonitoramentoVeiculo filtrosPesquisa)
        {
            if (filtrosPesquisa.CodigoTransportador == 0 && (filtrosPesquisa.CodigosVeiculo == null || filtrosPesquisa.CodigosVeiculo.Count == 0) && filtrosPesquisa.CodigosContratoFrete.Count == 0)
                return "Obrigatório informar um Transportador ou um Veículo ou um Contrato de Frete.";
            if (ConfiguracaoEmbarcador.QuantidadeMaximaDiasRelatorios > 0 && (filtrosPesquisa.DataFinal - filtrosPesquisa.DataInicial).Days > ConfiguracaoEmbarcador.QuantidadeMaximaDiasRelatorios)
                return $"O período não pode exceder {ConfiguracaoEmbarcador.QuantidadeMaximaDiasRelatorios} dias.";
            return string.Empty;
        }

        #endregion
    }
}
