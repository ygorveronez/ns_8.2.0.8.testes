using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using Dominio.Enumeradores;
using System.Linq;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Logisitca
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Logistica/MonitoramentoHistoricoTemperatura")]
    public class MonitoramentoHistoricoTemperaturaController : BaseController
    {
		#region Construtores

		public MonitoramentoHistoricoTemperaturaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios _codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R214_MonitoramentoHistoricoTemperatura;
        private readonly decimal _tamanhoColunaGrande = 5.50m;
        private readonly decimal _tamanhoColunaMedia = 3m;

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigoRelatorio = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(_codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Histórico de Temperatura", "Logistica", "MonitoramentoHistoricoTemperatura.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "Codigo", "asc", "", "", codigoRelatorio, unitOfWork, true, true);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio retorno = gridRelatorio.RetornoGridPadraoRelatorio(ObterGridPadrao(), relatorio);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados do relatório.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> Pesquisa(CancellationToken cancellationToken)
         {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();
                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioMonitoramentoHistoricoTemperatura filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.Logistica.MonitoramentoHistoricoTemperatura servicoRelatorioMonitoramentoHistoricoTemperatura = new Servicos.Embarcador.Relatorios.Logistica.MonitoramentoHistoricoTemperatura(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioMonitoramentoHistoricoTemperatura.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.Logistica.MonitoramentoHistoricoTemperatura> lista, out int totalRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(lista);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, excecao.Message);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> GerarRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string stringConexao = _conexao.StringConexao;

                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = serRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioMonitoramentoHistoricoTemperatura filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);

                await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (Dominio.Excecoes.Embarcador.ServicoException servicoException)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, servicoException.Message);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o reltároio.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioMonitoramentoHistoricoTemperatura ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {

            DateTime dataInicial = Request.GetDateTimeParam("DataInicial");
            DateTime dataFinal = Request.GetDateTimeParam("DataFinal");
            DateTime dataCriacaoCargaInicial = Request.GetDateTimeParam("DataCriacaoCargaInicial");
            DateTime dataCriacaoCargaFinal = Request.GetDateTimeParam("DataCriacaoCargaFinal");
            if (dataFinal == DateTime.MinValue) dataFinal = DateTime.Now;
            string numeroCarga = Request.GetStringParam("NumeroCarga");
            int codigoTransportador = Request.GetIntParam("Transportador");
            int codigoVeiculo = Request.GetIntParam("Veiculo");

            // Validação dos parâmetros
            if ((dataInicial == DateTime.MinValue && dataCriacaoCargaInicial == DateTime.MinValue) || (string.IsNullOrWhiteSpace(numeroCarga) && codigoTransportador == 0 && codigoVeiculo == 0))
            {
                throw new Exception("Deve ser informada a data inicial e ainda a carga ou transportador ou veículo");
            }

            if (dataInicial > dataFinal)
            {
                throw new Exception("A data inicial deve ser menor que a data final.");
            }

            TimeSpan diferenca = dataFinal - dataInicial;
            if (diferenca.TotalDays > 30 && dataCriacaoCargaInicial == DateTime.MinValue)
            {
                throw new Exception("O período entre e a data inicial e a data final não deve ultrapassar 30 dias.");
            }

            Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioMonitoramentoHistoricoTemperatura filtroPesquisa = new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioMonitoramentoHistoricoTemperatura()
            {
                DataInicial = dataInicial,
                DataFinal = dataFinal,
                DuranteMonitoramento = Request.GetBoolParam("DuranteMonitoramento"),
                NumeroCarga = numeroCarga,
                CodigoTransportador = codigoTransportador,
                CodigoVeiculo = codigoVeiculo,
                CodigoFaixaTemperatura = Request.GetIntParam("FaixaTemperatura"),
                StatusMonitoramento = Request.GetEnumParam<MonitoramentoStatus>("StatusMonitoramento"),
                ForaFaixa = Request.GetEnumParam<OpcaoSimNaoPesquisa>("ForaFaixa"),
                EntregasRealizadas = Request.GetEnumParam<OpcaoSimNaoPesquisa>("EntregasRealizadas"),
                DataCriacaoCargaInicial = dataCriacaoCargaInicial,
                DataCriacaoCargaFinal = dataCriacaoCargaFinal,
                CodigosRecebedores = ObterListaCnpjCpfRecebedorPermitidosOperadorLogistica(unitOfWork),
                CodigosStatusViagem = Request.GetListParam<int>("StatusViagem"),

            };

            List<int> codigosFiliais = ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork);

            if (codigosFiliais.Count() == 0)
                filtroPesquisa.CodigosFiliais = Request.GetIntParam("Filial") > 0 ? new List<int>() { Request.GetIntParam("Filial") } : new List<int>();

            return filtroPesquisa;
        }

        private Models.Grid.Grid ObterGridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid()
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Número da Carga", "NumeroCarga", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Placa", "Placa", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Reboques", "Reboques", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Transportador", "Transportador", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Motoristas", "Motoristas", _tamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Data Evento", "DataEventoFormatada", _tamanhoColunaMedia, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Faixa Temperatura", "FaixaTemperatura", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Faixa Inicial", "FaixaInicial", _tamanhoColunaMedia, Models.Grid.Align.right, true, false, false, false, false);
            grid.AdicionarCabecalho("Faixa Final", "FaixaFinal", _tamanhoColunaMedia, Models.Grid.Align.right, true, false, false, false, false);
            grid.AdicionarCabecalho("CD Origem", "CDOrigem", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Localização", "PosicaoDescricao", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Destino", "Destino", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Data Entrada Loja", "DataEntradaLojaFormatada", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Data Saída Loja", "DataSaidaLojaFormatada", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Latitude", "Latitude", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Longitude", "Longitude", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Temperatura", "Temperatura", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Data Criação Carga", "DataCriacaoCargaFormatada", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);

            return grid;
        }

        #endregion
    }
}
