using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using System.Threading.Tasks;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Logistica/TempoCarregamento")]
    public class TempoCarregamentoController : BaseController
    {
		#region Construtores

		public TempoCarregamentoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios _codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R015_TempoCarregamento;

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao); ;
            try
            {
                await unitOfWork.StartAsync(cancellationToken);
                int Codigo = int.Parse(Request.Params("Codigo"));

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(_codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de tempo de Carregamento", "Logistica", "TempoCarregamento.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "Codigo", "desc", "", "", Codigo, unitOfWork, true, true, 7);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                var retorno = gridRelatorio.RetornoGridPadraoRelatorio(ObterGridPadrao(), relatorio);
                await unitOfWork.CommitChangesAsync(cancellationToken);
                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados do relatório.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                DateTime dataInicioCarregamento;
                DateTime.TryParseExact(Request.Params("DataInicioCarregamento"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicioCarregamento);
                DateTime dataFimCarregamento;
                DateTime.TryParseExact(Request.Params("DataFimCarregamento"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFimCarregamento);

                int codigoTransportador, codigoMotorista, codigoVeiculo, centroCarregamento;
                int.TryParse(Request.Params("Transportador"), out codigoTransportador);
                int.TryParse(Request.Params("Motorista"), out codigoMotorista);
                int.TryParse(Request.Params("Veiculo"), out codigoVeiculo);
                int.TryParse(Request.Params("CentroCarregamento"), out centroCarregamento);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repositorioCargaGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(unitOfWork);
                int totalRegistros = repositorioCargaGuarita.ContarConsultaRelatorioTempoCarregamento(dataInicioCarregamento, dataFimCarregamento, codigoTransportador, codigoVeiculo, codigoMotorista, centroCarregamento);
                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita> listaCargaGuarita = (totalRegistros > 0) ? repositorioCargaGuarita.ConsultarRelatorioTempoCarregamento(dataInicioCarregamento, dataFimCarregamento, codigoTransportador, codigoVeiculo, codigoMotorista, centroCarregamento, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita>();

                var listaCargaGuaritaRetornar = (
                    from obj in listaCargaGuarita
                    select new
                    {
                        obj.Codigo,
                        CentroCarregamento = obj.CargaJanelaCarregamento.CentroCarregamento.Descricao,
                        DataCarregamento = obj.CargaJanelaCarregamento.InicioCarregamento,
                        DataChegadaVeiculo = obj.DataChegadaVeiculo?.ToString("dd/MM/yyyy HH:mm") ?? "",
                        DataEntregaGuarita = obj.DataEntregaGuarita?.ToString("dd/MM/yyyy HH:mm") ?? "",
                        DataFinalCarregamento = obj.DataFinalCarregamento?.ToString("dd/MM/yyyy HH:mm") ?? "",
                        DataLiberacaoVeiculo = obj.DataLiberacaoVeiculo?.ToString("dd/MM/yyyy HH:mm") ?? "",
                        ModeloVeiculo = obj.CargaBase.ModeloVeicularCarga?.Descricao,
                        Motorista = obj.CargaBase.ListaMotorista?.FirstOrDefault()?.Nome,
                        NumeroCarga = obj.Carga != null ? obj.Carga.CodigoCargaEmbarcador : "",
                        Veiculo = obj.CargaBase.Veiculo?.Placa,
                        Transportador = obj.CargaBase.Empresa?.RazaoSocial,
                        TempoParaEntradaVeiculo = TimeSpan.FromMinutes(ObterTempoEmMinutos(obj.DataChegadaVeiculo, obj.DataEntregaGuarita)).ToString(@"hh\:mm"),
                        TempoLiberacaoVeiculo = TimeSpan.FromMinutes(ObterTempoEmMinutos(obj.DataFinalCarregamento, obj.DataLiberacaoVeiculo)).ToString(@"hh\:mm"),
                        TempoTerminoCarregamento = TimeSpan.FromMinutes(ObterTempoEmMinutos(obj.DataEntregaGuarita, obj.DataFinalCarregamento)).ToString(@"hh\:mm"),
                        TempoTotalDeCarregamento = TimeSpan.FromMinutes(ObterTempoEmMinutos(obj.DataChegadaVeiculo, obj.DataLiberacaoVeiculo)).ToString(@"hh\:mm"),
                        DataProgramadaInicial = obj.CargaJanelaCarregamento?.DataCarregamentoProgramada.ToString("dd/MM/yyyy HH:mm") ?? "",
                        DiferencaDatasProgEProgInicial = TimeSpan.FromMinutes(ObterTempoEmMinutos(obj.CargaJanelaCarregamento?.DataCarregamentoProgramada, obj.CargaJanelaCarregamento?.InicioCarregamento)).ToString(@"hh\:mm"),
                        FimFaturamento = obj.Carga?.DataEnvioUltimaNFe?.ToString("dd/MM/yyyy HH:mm") ?? "",
                        TipoOperacao = obj.Carga?.TipoOperacao?.Descricao ?? string.Empty,
                        ObservacaoCarregamento = obj.CargaJanelaCarregamento.Carga?.Carregamento?.Observacao ?? string.Empty,
                    }
                ).ToList();

                grid.AdicionaRows(listaCargaGuaritaRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> GerarRelatorio(CancellationToken cancellationToken)
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigoTransportador, codigoMotorista, codigoVeiculo, centroCarregamento;
                int.TryParse(Request.Params("Transportador"), out codigoTransportador);
                int.TryParse(Request.Params("Motorista"), out codigoMotorista);
                int.TryParse(Request.Params("Veiculo"), out codigoVeiculo);
                int.TryParse(Request.Params("CentroCarregamento"), out centroCarregamento);

                DateTime dataInicioCarregamento;
                DateTime.TryParseExact(Request.Params("DataInicioCarregamento"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicioCarregamento);
                DateTime dataFimCarregamento;
                DateTime.TryParseExact(Request.Params("DataFimCarregamento"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFimCarregamento);

                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);


                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);
                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, this.Usuario, dynRelatorio.TipoArquivoRelatorio, unitOfWork);
                await unitOfWork.CommitChangesAsync(cancellationToken);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp = serRelatorio.ObterRelatorioTemporario(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware);


                gridRelatorio.SetarRelatorioPelaGrid(dynRelatorio.Grid, relatorioTemp);

                string stringConexao = _conexao.StringConexao;
                _ = Task.Factory.StartNew(() => GerarRelatorioTempoCarregamento(dataInicioCarregamento, dataFimCarregamento, codigoTransportador, codigoVeiculo, codigoMotorista, centroCarregamento, relatorioControleGeracao, relatorioTemp, stringConexao, CancellationToken.None));

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o relatorio.");
            }
        }

        #endregion

        #region Métodos Privados

        private async Task GerarRelatorioTempoCarregamento(DateTime dataInicioCarregamento, DateTime dataFimCarregamento, int codigoTransportador, int codigoVeiculo, int codigoMotorista, int centroCarregamento, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp, string stringConexao, CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
            Repositorio.Embarcador.Logistica.CentroCarregamento repCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repCargaGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(unitOfWork);
            Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta()
                {
                    DirecaoOrdenar = relatorioTemp.OrdemOrdenacao,
                    PropriedadeOrdenar = relatorioTemp.PropriedadeOrdena
                };
                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita> listaCargaGuarita = repCargaGuarita.ConsultarRelatorioTempoCarregamento(dataInicioCarregamento, dataFimCarregamento, codigoTransportador, codigoVeiculo, codigoMotorista, centroCarregamento, parametrosConsulta);

                List<Dominio.Relatorios.Embarcador.DataSource.Logistica.TempoCarregamento> listaCargaGuaritaRetornar = (
                    from obj in listaCargaGuarita
                    select new Dominio.Relatorios.Embarcador.DataSource.Logistica.TempoCarregamento
                    {
                        CentroCarregamento = obj.CargaJanelaCarregamento.CentroCarregamento.Descricao,
                        Codigo = obj.Codigo,
                        DataCarregamento = obj.CargaJanelaCarregamento.InicioCarregamento,
                        DataEntregaGuarita = obj.DataEntregaGuarita?.ToString("dd/MM/yyyy HH:mm") ?? "",
                        DataChegadaVeiculo = obj.DataChegadaVeiculo?.ToString("dd/MM/yyyy HH:mm") ?? "",
                        DataFinalCarregamento = obj.DataFinalCarregamento?.ToString("dd/MM/yyyy HH:mm") ?? "",
                        DataLiberacaoVeiculo = obj.DataLiberacaoVeiculo?.ToString("dd/MM/yyyy HH:mm") ?? "",
                        Veiculo = obj.CargaBase.Veiculo?.Placa,
                        ModeloVeiculo = obj.CargaBase.ModeloVeicularCarga?.Descricao,
                        Motorista = obj.CargaBase.ListaMotorista?.FirstOrDefault()?.Nome,
                        NumeroCarga = obj.Carga != null ? obj.Carga.CodigoCargaEmbarcador : "",
                        Transportador = obj.CargaBase.Empresa?.RazaoSocial,
                        TempoLiberacaoVeiculo = ObterTempoEmMinutos(obj.DataFinalCarregamento, obj.DataLiberacaoVeiculo),
                        TempoTerminoCarregamento = ObterTempoEmMinutos(obj.DataEntregaGuarita, obj.DataFinalCarregamento),
                        TempoTotalDeCarregamento = ObterTempoEmMinutos(obj.DataChegadaVeiculo, obj.DataLiberacaoVeiculo),
                        TempoParaEntradaVeiculo = ObterTempoEmMinutos(obj.DataChegadaVeiculo, obj.DataEntregaGuarita),
                        DataProgramadaInicial = obj.CargaJanelaCarregamento?.DataCarregamentoProgramada.ToString("dd/MM/yyyy HH:mm") ?? "",
                        DiferencaDatasProgEProgInicial = TimeSpan.FromMinutes(ObterTempoEmMinutos(obj.CargaJanelaCarregamento?.DataCarregamentoProgramada, obj.CargaJanelaCarregamento?.InicioCarregamento)).ToString(@"hh\:mm"),
                        FimFaturamento = obj.Carga?.DataEnvioUltimaNFe?.ToString("dd/MM/yyyy HH:mm") ?? "",
                        TipoOperacao = obj.Carga?.TipoOperacao?.Descricao ?? string.Empty,
                        ObservacaoCarregamento = obj.CargaJanelaCarregamento.Carga?.Carregamento?.Observacao ?? string.Empty,
                    }
                ).ToList();

                Dominio.Relatorios.Embarcador.ObjetosDeValor.IdentificacaoCamposRPT identificacao = new Dominio.Relatorios.Embarcador.ObjetosDeValor.IdentificacaoCamposRPT();
                identificacao.PrefixoCamposSum = "Médiade";


                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

                if (dataInicioCarregamento != DateTime.MinValue || dataInicioCarregamento != DateTime.MinValue)
                {
                    string data = "";
                    if (dataInicioCarregamento != DateTime.MinValue)
                        data = dataInicioCarregamento.ToString("dd/MM/yyyy");
                    if (dataFimCarregamento != DateTime.MinValue)
                        data += " até " + dataFimCarregamento.ToString("dd/MM/yyyy");

                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataCarregamento", data, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataCarregamento", false));

                if (codigoTransportador > 0)
                {
                    Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoTransportador);

                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Transportador", empresa.CNPJ_Formatado + " - " + empresa.RazaoSocial, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Transportador", false));

                if (codigoVeiculo > 0)
                {
                    Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(codigoVeiculo);

                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Veiculo", veiculo.Placa, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Veiculo", false));

                if (codigoMotorista > 0)
                {
                    Dominio.Entidades.Usuario usuario = repUsuario.BuscarPorCodigo(codigoMotorista);

                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Motorista", usuario.Nome, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Motorista", false));

                if (centroCarregamento > 0)
                {
                    Dominio.Entidades.Embarcador.Logistica.CentroCarregamento ecentroCarregamento = repCentroCarregamento.BuscarPorCodigo(centroCarregamento);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CentroCarregamento", ecentroCarregamento.Descricao, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CentroCarregamento", false));

                serRelatorio.GerarRelatorioDinamico("Relatorios/Logistica/TempoCarregamento", parametros,relatorioControleGeracao, relatorioTemp, listaCargaGuaritaRetornar, unitOfWork, identificacao);

                await unitOfWork.DisposeAsync();
            }
            catch (Exception ex)
            {
                serRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, ex);
            }
        }

        private double ObterTempoEmMinutos(DateTime? dataInicial, DateTime? dataFinal)
        {
            if (!dataInicial.HasValue || !dataFinal.HasValue)
                return 0d;

            return (dataFinal.Value - dataInicial.Value).TotalMinutes;
        }

        private Models.Grid.Grid ObterGridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();
            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Centro de Carregamento", "CentroCarregamento", 15, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Carga", "NumeroCarga", 8, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Transportador", "Transportador", 15, Models.Grid.Align.left, false, true, false, true, false);
            grid.AdicionarCabecalho("Motorista", "Motorista", 10, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Veículo", "Veiculo", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Modelo Veículo", "ModeloVeiculo", 10, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("D. Programada", "DataCarregamento", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("D. Chegada", "DataChegadaVeiculo", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("D. Entrada", "DataEntregaGuarita", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("D. Carregamento", "DataFinalCarregamento", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("D. Liberação", "DataLiberacaoVeiculo", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("T. Entrada", "TempoParaEntradaVeiculo", (decimal)8.5, Models.Grid.Align.right, false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.media).DataTypeExportacao("TimeSpan");
            grid.AdicionarCabecalho("T. Carregando", "TempoTerminoCarregamento", (decimal)8.5, Models.Grid.Align.right, false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.media).DataTypeExportacao("TimeSpan");
            grid.AdicionarCabecalho("T. Liberação", "TempoLiberacaoVeiculo", (decimal)8.5, Models.Grid.Align.right, false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.media).DataTypeExportacao("TimeSpan");
            grid.AdicionarCabecalho("T. Total", "TempoTotalDeCarregamento", (decimal)8.5, Models.Grid.Align.right, false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.media).DataTypeExportacao("TimeSpan");
            grid.AdicionarCabecalho("D. Prog. Inicial", "DataProgramadaInicial", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("D. Prog. - D. Prog. Inicial", "DiferencaDatasProgEProgInicial", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Fim Faturamento", "FimFaturamento", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Tipo de Operação", "TipoOperacao", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Observação do Carregamento", "ObservacaoCarregamento", 10, Models.Grid.Align.left, false, false, false, false, false);

            return grid;
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "DataCarregamento")
                return "CargaJanelaCarregamento.InicioCarregamento";

            if (propriedadeOrdenar == "NumeroCarga")
                return "CargaJanelaCarregamento.Carga.CodigoCargaEmbarcador";

            if (propriedadeOrdenar == "Transportador")
                return "CargaJanelaCarregamento.Carga.Empresa.RazaoSocial";

            if (propriedadeOrdenar == "DescricaoSituacao")
                return "Situacao";

            if (propriedadeOrdenar == "CentroCarregamento")
                return "CargaJanelaCarregamento.CentroCarregamento.Descricao";

            if (propriedadeOrdenar == "CarregamentoFinalizado")
                return "DataFinalCarregamento";

            return propriedadeOrdenar;
        }

        #endregion
    }
}
