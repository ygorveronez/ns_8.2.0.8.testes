using Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Cargas
{
    [Area("Relatorios")]
    [CustomAuthorize(new string[] { "ObterQuantidadeDeStages" }, "Relatorios/Cargas/Carga")]
    public class CargaController : Relatorios.AutomatizacaoGeracaoRelatorioController<Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarga>
    {
        #region Construtores

        public CargaController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Atributos

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios _codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R012_Cargas;
        private readonly int _numeroMaximoComplementos = 60;
        private readonly decimal _tamanhoColunaExtraPequena = 1m;
        private readonly decimal _tamanhoColunaGrande = 5.50m;
        private readonly decimal _tamanhoColunaMedia = 3m;
        private readonly decimal _tamanhoColunaPequena = 1.75m;

        #endregion Atributos

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigoRelatorio = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(_codigoControleRelatorio, TipoServicoMultisoftware, Localization.Resources.Relatorios.Cargas.Carga.RelatorioDeCargas, "Cargas", "Carga.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "DataCarregamento", "desc", "", "", codigoRelatorio, unitOfWork, true, true);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio retorno = gridRelatorio.RetornoGridPadraoRelatorio(await ObterGridPadraoAsync(unitOfWork, cancellationToken), relatorio);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Relatorios.Relatorio.OcorreuUmaFalhaAoBuscarOsDadosDoRelatorio);
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

                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarga filtrosPesquisa = await ObterFiltrosPesquisaAsync(unitOfWork, cancellationToken);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.Carga.Carga srvRelatorioCarga = new Servicos.Embarcador.Relatorios.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente);

                srvRelatorioCarga.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.RelatorioCarga> listaCargas, out int totalRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(listaCargas);

                return new JsonpResult(grid);
            }
            catch (ServicoException excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
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
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemporario = serRelatorio.ObterRelatorioTemporario(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                gridRelatorio.SetarRelatorioPelaGrid(dynRelatorio.Grid, relatorioTemporario);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = serRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarga filtrosPesquisa = await ObterFiltrosPesquisaAsync(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (ServicoException servicoException)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, servicoException.Message);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoGerar);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> ObterQuantidadeDeStages(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Pedidos.Stage repositorioStage = new Repositorio.Embarcador.Pedidos.Stage(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);

                var retorno = new
                {
                    QuantidadeStages = await repositorioStage.BuscarQuantidadeStageAsync(),
                    PossuiTipoOperacaoConsolidacao = await repTipoOperacao.ExisteTipoOperacaoConsolidacaoAsync(cancellationToken),
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> VerificarSeExisteTipoOperacaoCarga(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork, cancellationToken);
                List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> tiposOperacao = await repositorioTipoOperacao.BuscarTodosAsync();

                var dynConfiguracaoTipoOperacaoCarga = new
                {
                    PermitirIntegrarPacotes = tiposOperacao.Any(t => t.ConfiguracaoCarga != null && t.ConfiguracaoCarga.PermitirIntegrarPacotes),
                };

                return new JsonpResult(dynConfiguracaoTipoOperacaoCarga);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao obter as Configurações Gerais do Sistema.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion Métodos Globais

        #region Métodos Protegidos Sobrescritos

        protected override Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarga ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarga filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarga()
            {
                DataInicialInicioEmissaoDocumentos = Request.GetNullableDateTimeParam("DataInicialInicioEmissaoDocumentos"),
                DataFinalInicioEmissaoDocumentos = Request.GetNullableDateTimeParam("DataFinalInicioEmissaoDocumentos"),
                DataInicialFimEmissaoDocumentos = Request.GetNullableDateTimeParam("DataInicialFimEmissaoDocumentos"),
                DataFinalFimEmissaoDocumentos = Request.GetNullableDateTimeParam("DataFinalFimEmissaoDocumentos"),
                DataAnulacaoInicial = Request.GetNullableDateTimeParam("DataAnulacaoInicial"),
                DataAnulacaoFinal = Request.GetNullableDateTimeParam("DataAnulacaoFinal"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicial"),
                DataFinal = Request.GetNullableDateTimeParam("DataFinal"),
                DataInclusaoBookingInicial = Request.GetNullableDateTimeParam("DataInclusaoBookingInicial"),
                DataInclusaoBookingLimite = Request.GetNullableDateTimeParam("DataInclusaoBookingLimite"),
                DataInclusaoPCPInicial = Request.GetNullableDateTimeParam("DataInclusaoPCPInicial"),
                DataInclusaoPCPLimite = Request.GetNullableDateTimeParam("DataInclusaoPCPLimite"),
                CodigoTransportador = Request.GetIntParam("Transportador"),
                CodigosCentroCarregamento = Request.GetListParam<int>("CentroCarregamento"),
                CodigosRotas = Request.GetListParam<int>("Rota"),
                CodigosOrigem = Request.GetListParam<int>("Origem"),
                CodigosDestino = Request.GetListParam<int>("Destino"),
                CodigosCentroResultado = Request.GetListParam<int>("CentroResultado"),
                CodigosModeloVeicularCarga = Request.GetListParam<int>("ModeloVeiculo"),
                CpfCnpjRemetente = Request.GetStringParam("Remetente").ObterSomenteNumeros().ToDouble(),
                CpfCnpjDestinatarios = Request.GetListParam<double>("Destinatario"),
                CodigosVeiculos = Request.GetListParam<int>("Veiculo"),
                CodigosMotorista = Request.GetListParam<int>("Motorista"),
                CodigoGrupoPessoas = Request.GetListParam<int>("GrupoPessoas"),
                DeliveryTerm = Request.GetStringParam("DeliveryTerm"),
                IdAutorizacao = Request.GetStringParam("IdAutorizacao"),
                Situacoes = Request.GetListEnumParam<SituacaoCarga>("Situacoes"),
                SituacoesCargaMercante = Request.GetListEnumParam<SituacaoCargaMercante>("SituacaoCargaMercante"),
                CodigoOperador = Request.GetIntParam("Operador"),
                TipoLocalPrestacao = Request.GetEnumParam<TipoLocalPrestacao>("TipoLocalPrestacao"),
                SomenteDescontoOperador = Request.GetBoolParam("SomenteDescontoOperador"),
                NumeroMDFe = Request.GetIntParam("NumeroMdfe"),
                PreCarga = Request.GetStringParam("PreCarga"),
                NumeroPreCarga = Request.GetStringParam("NumeroPreCarga"),
                Pedido = Request.GetStringParam("Pedido"),
                SomenteComReserva = Request.GetBoolParam("SomenteComReserva"),
                SomenteTerceiros = Request.GetNullableBoolParam("SomenteTerceiros"),
                Transbordo = Request.GetNullableBoolParam("Transbordo"),
                ExibirCargasAgrupadas = Request.GetBoolParam("ExibirCargasAgrupadas"),
                CodigoCarregamento = Request.GetIntParam("Carregamento"),
                CodigoCargaEmbarcador = Request.GetStringParam("CodigoCargaEmbarcador"),
                CodigosTabelasFrete = Request.GetListParam<int>("TabelasFrete"),
                NumeroBooking = Request.GetStringParam("NumeroBooking"),
                NumeroOS = Request.GetStringParam("NumeroOS"),
                NumeroNF = Request.GetIntParam("NumeroNota"),
                NumeroControle = Request.GetStringParam("NumeroControle"),
                SituacaoCTe = Request.GetStringParam("SituacaoCTe"),
                PortoOrigem = Request.GetIntParam("PortoOrigem"),
                PortoDestino = Request.GetIntParam("PortoDestino"),
                CodigoPedidoViagemNavio = Request.GetIntParam("Viagem"),
                Container = Request.GetIntParam("Container"),
                CodigoTipoSeparacao = Request.GetIntParam("TipoSeparacao"),
                TipoCTe = Request.GetListEnumParam<TipoCTE>("TipoCTe"),
                TipoPropostaMultimodal = Request.GetListEnumParam<TipoPropostaMultimodal>("TipoPropostaMultimodal"),
                TipoServicoMultimodal = Request.GetListEnumParam<TipoServicoMultimodal>("TipoServicoMultimodal"),
                VeioPorImportacao = Request.GetEnumParam<OpcaoSimNaoPesquisa>("VeioPorImportacao"),
                SomenteCTeSubstituido = Request.GetBoolParam("SomenteCTeSubstituido"),
                InformacoesRelatorioCargas = Request.GetEnumParam<InformacoesRelatorioCarga>("InformacoesCargaPreCarga"),
                Problemas = Request.GetEnumParam<ProblemasCarga>("Problemas"),
                NumeroPedidoCliente = Request.GetStringParam("NumeroPedidoCliente"),
                DataEncerramentoInicial = Request.GetNullableDateTimeParam("DataEncerramentoInicial"),
                DataEncerramentoFinal = Request.GetNullableDateTimeParam("DataEncerramentoFinal"),
                TipoServicoMultisoftware = TipoServicoMultisoftware,
                DataConfirmacaoDocumentosInicial = Request.GetNullableDateTimeParam("DataConfirmacaoDocumentosInicial"),
                DataConfirmacaoDocumentosFinal = Request.GetNullableDateTimeParam("DataConfirmacaoDocumentosFinal"),
                CpfCnpjExpedidores = Request.GetListParam<double>("Expedidor"),
                CpfCnpjRecebedores = Request.GetListParam<double>("Recebedor"),
                NaoComparecimento = Request.GetEnumParam<OpcaoSimNaoPesquisa>("NaoComparecimento"),
                CargaTrechos = Request.GetEnumParam<CargaTrechos>("CargaTrechos"),
                CanalEntrega = Request.GetIntParam("CanalEntrega"),
                CargaTrechoSumarizada = Request.GetNullableEnumParam<CargaTrechoSumarizada>("CargaTrechoSumarizada"),
                FiltrarCargasPorParteDoNumero = ConfiguracaoEmbarcador?.FiltrarCargasPorParteDoNumero ?? false,
                DataFaturamentoInicial = Request.GetNullableDateTimeParam("DataFaturamentoInicial"),
                DataFaturamentoFinal = Request.GetNullableDateTimeParam("DataFaturamentoFinal"),
                DataCarregamentoInicio = Request.GetNullableDateTimeParam("DataCarregamentoInicio"),
                DataCarregamentoFim = Request.GetNullableDateTimeParam("DataCarregamentoFim"),
                CargasSemPacote = Request.GetBoolParam("CargasSemPacote"),
                TipoOSConvertido = Request.GetListEnumParam<TipoOSConvertido>("TipoOSConvertido"),
                TipoOS = Request.GetListEnumParam<TipoOS>("TipoOS"),
                DirecionamentoCustoExtra = Request.GetListEnumParam<TipoDirecionamentoCustoExtra>("DirecionamentoCustoExtra"),
                StatusCustoExtra = Request.GetListEnumParam<StatusCustoExtra>("StatusCustoExtra"),
                CodigosProvedores = Request.GetListParam<double>("ProvedorOS"),
                CentroDeCustoViagemCodigo = Request.GetIntParam("CentroDeCustoViagemCodigo"),
                CodigosGrupoProduto = Request.GetListParam<int>("GrupoProduto"),
                FlagCargaPercentualExecucao = Request.GetBoolParam("FlagCargaPercentualExecucao"),
                DataInclusaoDadosTransporte = Request.GetNullableDateTimeParam("DataInclusaoDadosTransporte")
            };

            // TODO (ct-reports): Repassar CT
            filtrosPesquisa.CpfCnpjRecebedoresOuSemRecebedores = ObterListaCnpjCpfRecebedorPermitidosOperadorLogistica(unitOfWork);
            filtrosPesquisa.CodigosFilial = Request.GetNullableListParam<int>("Filial") ?? ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork);
            filtrosPesquisa.CodigosFilialVenda = Request.GetNullableListParam<int>("FilialVenda") ?? ObterListaCodigoFilialVendaPermitidasOperadorLogistica(unitOfWork);
            filtrosPesquisa.CodigosTipoOperacao = Request.GetNullableListParam<int>("TipoOperacao") ?? ObterListaCodigoTipoOperacaoPermitidosOperadorLogistica(unitOfWork);
            filtrosPesquisa.CodigosTipoCarga = Request.GetNullableListParam<int>("TipoCarga") ?? ObterListaCodigoTipoCargaPermitidosOperadorLogistica(unitOfWork);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
            {
                if (!Empresa.Matriz.Any())
                {
                    Repositorio.Empresa empresa = new Repositorio.Empresa(unitOfWork);
                    List<int> codigosEmpresa = empresa.BuscarCodigoMatrizEFiliais(Usuario.Empresa?.CNPJ_SemFormato);
                    filtrosPesquisa.CodigosTransportador = codigosEmpresa?.Count > 0 ? codigosEmpresa : null;
                }
                else
                    filtrosPesquisa.CodigoTransportador = Usuario.Empresa?.Codigo ?? 0;
            }
            else
                filtrosPesquisa.CodigosTransportador = Request.GetListParam<int>("Transportador");

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador = repConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
            filtrosPesquisa.HabilitarHoraFiltroDataInicialFinalRelatorioCargas = _configuracaoEmbarcador?.HabilitarHoraFiltroDataInicialFinalRelatorioCargas ?? false;
            filtrosPesquisa.VisualizarValorNFSeDescontandoISSRetido = _configuracaoEmbarcador?.VisualizarValorNFSeDescontandoISSRetido ?? false;

            return filtrosPesquisa;
        }

        protected override async Task<Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarga> ObterFiltrosPesquisaAsync(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarga filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarga()
            {
                DataInicialInicioEmissaoDocumentos = Request.GetNullableDateTimeParam("DataInicialInicioEmissaoDocumentos"),
                DataFinalInicioEmissaoDocumentos = Request.GetNullableDateTimeParam("DataFinalInicioEmissaoDocumentos"),
                DataInicialFimEmissaoDocumentos = Request.GetNullableDateTimeParam("DataInicialFimEmissaoDocumentos"),
                DataFinalFimEmissaoDocumentos = Request.GetNullableDateTimeParam("DataFinalFimEmissaoDocumentos"),
                DataAnulacaoInicial = Request.GetNullableDateTimeParam("DataAnulacaoInicial"),
                DataAnulacaoFinal = Request.GetNullableDateTimeParam("DataAnulacaoFinal"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicial"),
                DataFinal = Request.GetNullableDateTimeParam("DataFinal"),
                DataInclusaoBookingInicial = Request.GetNullableDateTimeParam("DataInclusaoBookingInicial"),
                DataInclusaoBookingLimite = Request.GetNullableDateTimeParam("DataInclusaoBookingLimite"),
                DataInclusaoPCPInicial = Request.GetNullableDateTimeParam("DataInclusaoPCPInicial"),
                DataInclusaoPCPLimite = Request.GetNullableDateTimeParam("DataInclusaoPCPLimite"),
                CodigoTransportador = Request.GetIntParam("Transportador"),
                CodigosCentroCarregamento = Request.GetListParam<int>("CentroCarregamento"),
                CodigosRotas = Request.GetListParam<int>("Rota"),
                CodigosOrigem = Request.GetListParam<int>("Origem"),
                CodigosDestino = Request.GetListParam<int>("Destino"),
                CodigosCentroResultado = Request.GetListParam<int>("CentroResultado"),
                CodigosModeloVeicularCarga = Request.GetListParam<int>("ModeloVeiculo"),
                CpfCnpjRemetente = Request.GetStringParam("Remetente").ObterSomenteNumeros().ToDouble(),
                CpfCnpjDestinatarios = Request.GetListParam<double>("Destinatario"),
                CodigosVeiculos = Request.GetListParam<int>("Veiculo"),
                CodigosMotorista = Request.GetListParam<int>("Motorista"),
                CodigoGrupoPessoas = Request.GetListParam<int>("GrupoPessoas"),
                DeliveryTerm = Request.GetStringParam("DeliveryTerm"),
                IdAutorizacao = Request.GetStringParam("IdAutorizacao"),
                Situacoes = Request.GetListEnumParam<SituacaoCarga>("Situacoes"),
                SituacoesCargaMercante = Request.GetListEnumParam<SituacaoCargaMercante>("SituacaoCargaMercante"),
                CodigoOperador = Request.GetIntParam("Operador"),
                TipoLocalPrestacao = Request.GetEnumParam<TipoLocalPrestacao>("TipoLocalPrestacao"),
                SomenteDescontoOperador = Request.GetBoolParam("SomenteDescontoOperador"),
                NumeroMDFe = Request.GetIntParam("NumeroMdfe"),
                PreCarga = Request.GetStringParam("PreCarga"),
                NumeroPreCarga = Request.GetStringParam("NumeroPreCarga"),
                Pedido = Request.GetStringParam("Pedido"),
                SomenteComReserva = Request.GetBoolParam("SomenteComReserva"),
                SomenteTerceiros = Request.GetNullableBoolParam("SomenteTerceiros"),
                Transbordo = Request.GetNullableBoolParam("Transbordo"),
                ExibirCargasAgrupadas = Request.GetBoolParam("ExibirCargasAgrupadas"),
                CodigoCarregamento = Request.GetIntParam("Carregamento"),
                CodigoCargaEmbarcador = Request.GetStringParam("CodigoCargaEmbarcador"),
                CodigosTabelasFrete = Request.GetListParam<int>("TabelasFrete"),
                NumeroBooking = Request.GetStringParam("NumeroBooking"),
                NumeroOS = Request.GetStringParam("NumeroOS"),
                NumeroNF = Request.GetIntParam("NumeroNota"),
                NumeroControle = Request.GetStringParam("NumeroControle"),
                SituacaoCTe = Request.GetStringParam("SituacaoCTe"),
                PortoOrigem = Request.GetIntParam("PortoOrigem"),
                PortoDestino = Request.GetIntParam("PortoDestino"),
                CodigoPedidoViagemNavio = Request.GetIntParam("Viagem"),
                Container = Request.GetIntParam("Container"),
                CodigoTipoSeparacao = Request.GetIntParam("TipoSeparacao"),
                TipoCTe = Request.GetListEnumParam<TipoCTE>("TipoCTe"),
                TipoPropostaMultimodal = Request.GetListEnumParam<TipoPropostaMultimodal>("TipoPropostaMultimodal"),
                TipoServicoMultimodal = Request.GetListEnumParam<TipoServicoMultimodal>("TipoServicoMultimodal"),
                VeioPorImportacao = Request.GetEnumParam<OpcaoSimNaoPesquisa>("VeioPorImportacao"),
                SomenteCTeSubstituido = Request.GetBoolParam("SomenteCTeSubstituido"),
                InformacoesRelatorioCargas = Request.GetEnumParam<InformacoesRelatorioCarga>("InformacoesCargaPreCarga"),
                Problemas = Request.GetEnumParam<ProblemasCarga>("Problemas"),
                NumeroPedidoCliente = Request.GetStringParam("NumeroPedidoCliente"),
                DataEncerramentoInicial = Request.GetNullableDateTimeParam("DataEncerramentoInicial"),
                DataEncerramentoFinal = Request.GetNullableDateTimeParam("DataEncerramentoFinal"),
                TipoServicoMultisoftware = TipoServicoMultisoftware,
                DataConfirmacaoDocumentosInicial = Request.GetNullableDateTimeParam("DataConfirmacaoDocumentosInicial"),
                DataConfirmacaoDocumentosFinal = Request.GetNullableDateTimeParam("DataConfirmacaoDocumentosFinal"),
                CpfCnpjExpedidores = Request.GetListParam<double>("Expedidor"),
                CpfCnpjRecebedores = Request.GetListParam<double>("Recebedor"),
                NaoComparecimento = Request.GetEnumParam<OpcaoSimNaoPesquisa>("NaoComparecimento"),
                CargaTrechos = Request.GetEnumParam<CargaTrechos>("CargaTrechos"),
                CanalEntrega = Request.GetIntParam("CanalEntrega"),
                CargaTrechoSumarizada = Request.GetNullableEnumParam<CargaTrechoSumarizada>("CargaTrechoSumarizada"),
                FiltrarCargasPorParteDoNumero = ConfiguracaoEmbarcador?.FiltrarCargasPorParteDoNumero ?? false,
                DataFaturamentoInicial = Request.GetNullableDateTimeParam("DataFaturamentoInicial"),
                DataFaturamentoFinal = Request.GetNullableDateTimeParam("DataFaturamentoFinal"),
                DataCarregamentoInicio = Request.GetNullableDateTimeParam("DataCarregamentoInicio"),
                DataCarregamentoFim = Request.GetNullableDateTimeParam("DataCarregamentoFim"),
                CargasSemPacote = Request.GetBoolParam("CargasSemPacote"),
                TipoOSConvertido = Request.GetListEnumParam<TipoOSConvertido>("TipoOSConvertido"),
                TipoOS = Request.GetListEnumParam<TipoOS>("TipoOS"),
                DirecionamentoCustoExtra = Request.GetListEnumParam<TipoDirecionamentoCustoExtra>("DirecionamentoCustoExtra"),
                StatusCustoExtra = Request.GetListEnumParam<StatusCustoExtra>("StatusCustoExtra"),
                CodigosProvedores = Request.GetListParam<double>("ProvedorOS"),
                CentroDeCustoViagemCodigo = Request.GetIntParam("CentroDeCustoViagemCodigo"),
                CodigosGrupoProduto = Request.GetListParam<int>("GrupoProduto"),
                FlagCargaPercentualExecucao = Request.GetBoolParam("FlagCargaPercentualExecucao"),
                DataInclusaoDadosTransporte = Request.GetNullableDateTimeParam("DataInclusaoDadosTransporte"),
                NumeroDtNatura = Request.GetIntParam("NumeroDtNatura")
            };
            filtrosPesquisa.CpfCnpjRecebedoresOuSemRecebedores = ObterListaCnpjCpfRecebedorPermitidosOperadorLogistica(unitOfWork);
            filtrosPesquisa.CodigosFilial = Request.GetNullableListParam<int>("Filial") ?? ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork);
            filtrosPesquisa.CodigosFilialVenda = Request.GetNullableListParam<int>("FilialVenda") ?? ObterListaCodigoFilialVendaPermitidasOperadorLogistica(unitOfWork);
            filtrosPesquisa.CodigosTipoOperacao = Request.GetNullableListParam<int>("TipoOperacao") ?? ObterListaCodigoTipoOperacaoPermitidosOperadorLogistica(unitOfWork);
            filtrosPesquisa.CodigosTipoCarga = Request.GetNullableListParam<int>("TipoCarga") ?? ObterListaCodigoTipoCargaPermitidosOperadorLogistica(unitOfWork);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
            {
                if (!Empresa.Matriz.Any())
                {
                    Repositorio.Empresa empresa = new Repositorio.Empresa(unitOfWork, cancellationToken);
                    List<int> codigosEmpresa = await empresa.BuscarCodigoMatrizEFiliaisAsync(Usuario.Empresa?.CNPJ_SemFormato);
                    filtrosPesquisa.CodigosTransportador = codigosEmpresa?.Count > 0 ? codigosEmpresa : null;
                }
                else
                    filtrosPesquisa.CodigoTransportador = Usuario.Empresa?.Codigo ?? 0;
            }
            else
                filtrosPesquisa.CodigosTransportador = Request.GetListParam<int>("Transportador");

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork, cancellationToken);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador = await repConfiguracaoEmbarcador.BuscarConfiguracaoPadraoAsync();
            filtrosPesquisa.HabilitarHoraFiltroDataInicialFinalRelatorioCargas = _configuracaoEmbarcador?.HabilitarHoraFiltroDataInicialFinalRelatorioCargas ?? false;
            filtrosPesquisa.VisualizarValorNFSeDescontandoISSRetido = _configuracaoEmbarcador?.VisualizarValorNFSeDescontandoISSRetido ?? false;

            return filtrosPesquisa;
        }

        #endregion Métodos Protegidos Sobrescritos

        #region Métodos Privados

        private async Task<Models.Grid.Grid> ObterGridPadraoAsync(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid()
            {
                header = new List<Models.Grid.Head>()
            };

            Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(unitOfWork, cancellationToken);
            Repositorio.Embarcador.Configuracoes.IntegracaoDigitalCom repConfigDigitalCom = new Repositorio.Embarcador.Configuracoes.IntegracaoDigitalCom(unitOfWork, cancellationToken);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDigitalCom configuracaoDigitalCom = await repConfigDigitalCom.BuscarAsync();
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = await repIntegracaoIntercab.BuscarIntegracaoAsync();

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("ID Carga", "IDCarga", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.NumeroCarga, "NumeroCarga", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.NumeroPreCarga, "PreCarga", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.DataCarga, "DescricaoDataCarregamento", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.DataColeta, "DataColeta", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.DataCarregamento, "DataCarregamentoFormatada", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.NumeroDoDT, "NumeroDtNatura", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.InicioEmissaoDocumentos, "DataInicioEmissaoDocumentos", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.FimEmissaoDocumentos, "DataFimEmissaoDocumentos", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.CMDID, "CMDID", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.CodigoNavio, "CodigoNavio", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.EmpresaFilial, "Transportador", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, true);
                grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.CNPJDaEmpresaFilial, "CNPJTransportadorFormatado", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, false);
            }
            else
            {
                grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.Transportador, "Transportador", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, true);
                grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.CNPJDoTransportador, "CNPJTransportadorFormatado", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, false);
            }

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
            {
                grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.Filial, "Filial", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, true);
                grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.FilialVenda, "FilialVenda", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
                grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.CNPJDaFilial, "CNPJFormatado", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, true);
            }

            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.Veiculo, "Veiculos", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.ModeloDoVeiculo, "ModeloVeiculo", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.CapacidadeVeiculo, "CapacidadePesoVeiculo", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.TipoDeCarga, "TipoCarga", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.TipoDeOperacao, "TipoOperacao", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.Rota, "Rota", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.NumeroPedidoEmbarcador, "NumeroPedido", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.NumeroPedidoInterno, "NumeroPedidoInterno", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.KmRodado, "KmRodado", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.ValorFreteLiquidoPorKM, "ValorLiquidoKm", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.ValorKM, "ValorKm", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.GrupoDePessoas, "GrupoPessoas", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.Tomador, "Tomador", _tamanhoColunaGrande, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.CodigoRemetente, "CodigoIntegracaoRemetentes", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.Remetente, "Remetente", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.Origem, "Origem", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.UFOrigem, "UFOrigem", _tamanhoColunaExtraPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.PaisOrigem, "PaisOrigem", _tamanhoColunaExtraPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.GrupoRemetente, "GrupoRemetente", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.CategoriaRemetente, "CategoriaRemetente", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.Carregamento, "Carregamento", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false).Ocultar(TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.DataDescarregamento, "DataDescarregamentoFormatada", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false).Ocultar(TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.CodigoDestinatario, "CodigoIntegracaoDestinatarios", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.Destinatario, "Destinatario", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.Destino, "Destino", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.UFDestino, "UFDestino", _tamanhoColunaExtraPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.PaisDestino, "PaisDestino", _tamanhoColunaExtraPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.GrupoDestinatario, "GrupoDestinatario", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.CategoriaDestinatario, "CategoriaDestinatario", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.Coletas, "NumeroColetas", _tamanhoColunaExtraPequena, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.Entregas, "NumeroEntregas", _tamanhoColunaExtraPequena, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.PesoCarga, "PesoCarga", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);

            if (!(integracaoIntercab?.AtivarNovosFiltrosConsultaCarga ?? false))
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "DescricaoSituacao", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, true);

            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.OperadorDaCarga, "OperadorCarga", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.DataAnulacao, "DataAnulacao", _tamanhoColunaMedia, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.JustificativaAnulacao, "JustificativaAnulacao", _tamanhoColunaGrande, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.OperadorDaAnulacao, "OperadorAnulacao", _tamanhoColunaMedia, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.DataCancelamento, "DataCancelamento", _tamanhoColunaMedia, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.JustificativaCancelamento, "JustificativaCancelamento", _tamanhoColunaGrande, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.MotivoCancelamento, "MotivoCancelamento", _tamanhoColunaGrande, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.OperadorDoCancelamento, "OperadorCancelamento", _tamanhoColunaMedia, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.ValorFreteLiquido, "ValorFreteLiquido", _tamanhoColunaMedia, Models.Grid.Align.right, true, false, false, false, TipoSumarizacao.sum);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.ValorICMS, "ValorICMS", _tamanhoColunaMedia, Models.Grid.Align.right, true, false, false, true, TipoSumarizacao.sum);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.ValorISS, "ValorISS", _tamanhoColunaMedia, Models.Grid.Align.right, true, false, false, true, TipoSumarizacao.sum);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.ValorNF, "ValorTotalNotaFiscal", _tamanhoColunaMedia, Models.Grid.Align.right, true, false, false, true, TipoSumarizacao.sum);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.ValorFrete, "ValorFrete", _tamanhoColunaMedia, Models.Grid.Align.right, true, false, false, true, TipoSumarizacao.sum);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.ValorFreteSemImposto, "ValorFreteSemImposto", _tamanhoColunaMedia, Models.Grid.Align.right, true, false, false, false, TipoSumarizacao.sum);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.ValorFreteResidual, "ValorFreteResidual", _tamanhoColunaMedia, Models.Grid.Align.right, true, false, false, false, TipoSumarizacao.sum);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.ValorViagem, "ValorViagem", _tamanhoColunaMedia, Models.Grid.Align.right, true, false, false, false, TipoSumarizacao.sum);

            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.CSTIBSCBS, "CSTIBSCBS", _tamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.ClassifTribIBSCBS, "ClassificacaoTributariaIBSCBS", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.BaseCalculoIBSCBS, "BaseCalculoIBSCBS", _tamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.AliquotaIBSEstadual, "AliquotaIBSEstadual", _tamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.PercentualReducaoIBSEstadual, "PercentualReducaoIBSEstadual", _tamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.ValorReducaoIBSEstadual, "ValorReducaoIBSEstadual", _tamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.ValorIBSEstadual, "ValorIBSEstadual", _tamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.AliquotaIBSMunicipal, "AliquotaIBSMunicipal", _tamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.PercentualReducaoIBSMunicipal, "PercentualReducaoIBSMunicipal", _tamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.ValorReducaoIBSMunicipal, "ValorReducaoIBSMunicipal", _tamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.ValorIBSMunicipal, "ValorIBSMunicipal", _tamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.AliquotaCBS, "AliquotaCBS", _tamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.PercentualReducaoCBS, "PercentualReducaoCBS", _tamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.ValorReducaoCBS, "ValorReducaoCBS", _tamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.ValorCBS, "ValorCBS", _tamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false, false);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.DescontoDoOperador, "DescontoOperador", _tamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false, TipoSumarizacao.sum);

            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.TaxaOcupacaoVeiculo, "TaxaOcupacaoVeiculo", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, TipoSumarizacao.media);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.TaxaIncidenciaFrete, "TaxaIncidenciaFrete", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, TipoSumarizacao.media);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.NumeroPreNota, "NotasParciais", _tamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.NumeroNotaFiscalAbreviado, "NotasFiscais", _tamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.Pallets, "Pallets", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(descricao: Localization.Resources.Relatorios.Cargas.Carga.Motoristas, propriedade: "Motoristas", tamanho: _tamanhoColunaGrande, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false, visible: false);
            grid.AdicionarCabecalho(descricao: Localization.Resources.Relatorios.Cargas.Carga.MDFes, propriedade: "Mdfes", tamanho: _tamanhoColunaMedia, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false, visible: false);
            grid.AdicionarCabecalho(descricao: Localization.Resources.Relatorios.Cargas.Carga.KMInicial, propriedade: "KMInicial", tamanho: _tamanhoColunaPequena, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true, visible: false);
            grid.AdicionarCabecalho(descricao: Localization.Resources.Relatorios.Cargas.Carga.KMFinal, propriedade: "KMFinal", tamanho: _tamanhoColunaPequena, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true, visible: false);
            grid.AdicionarCabecalho(descricao: Localization.Resources.Relatorios.Cargas.Carga.KMTotal, propriedade: "KMTotal", tamanho: _tamanhoColunaPequena, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true, visible: false);
            grid.AdicionarCabecalho(descricao: Localization.Resources.Relatorios.Cargas.Carga.HoraInicial, propriedade: "HoraInicial", tamanho: _tamanhoColunaPequena, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true, visible: false);
            grid.AdicionarCabecalho(descricao: Localization.Resources.Relatorios.Cargas.Carga.HoraFinal, propriedade: "HoraFinal", tamanho: _tamanhoColunaPequena, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true, visible: false);
            grid.AdicionarCabecalho(descricao: Localization.Resources.Relatorios.Cargas.Carga.HorasNormais, propriedade: "QuantidadeHorasNormais", tamanho: _tamanhoColunaPequena, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true, visible: false);
            grid.AdicionarCabecalho(descricao: Localization.Resources.Relatorios.Cargas.Carga.HorasExcedentes, propriedade: "QuantidadeHorasExcedentes", tamanho: _tamanhoColunaPequena, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true, visible: false);
            grid.AdicionarCabecalho(descricao: Localization.Resources.Relatorios.Cargas.Carga.TotalHoras, propriedade: "QuantidadeHorasTotais", tamanho: _tamanhoColunaPequena, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true, visible: false);
            grid.AdicionarCabecalho(descricao: Localization.Resources.Relatorios.Cargas.Carga.Transbordo, propriedade: "Transbordo", tamanho: _tamanhoColunaPequena, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true, visible: false);
            grid.AdicionarCabecalho(descricao: Localization.Resources.Relatorios.Cargas.Carga.FreteDeTerceiro, propriedade: "FreteTerceiro", tamanho: _tamanhoColunaPequena, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true, visible: false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.Ordem, "Ordem", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.Genset, "Genset", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.DiaDaSemana, "DiaSemana", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.DataDeEmbarque, "DataEmbarqueFormatada", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.PortoDeChegada, "PortoChegada", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false).Ocultar(ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.PortoDeSaida, "PortoSaida", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false).Ocultar(ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.Temperatura, "Temperatura", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.TipoDeEmbarque, "TipoEmbarque", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false).Ocultar(!ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.Companhia, "Companhia", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.NumeroNavio, "NumeroNavio", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false).Ocultar(ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.Resumo, "Resumo", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.Booking, "NumeroBooking", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.Reserva, "Reserva", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.DataETA, "DataETA", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false).Ocultar(ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.DeliveryTerm, "DeliveryTerm", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.IDAutorizacao, "IdAutorizacao", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.InclusaoDeBooking, "DataInclusaoBooking", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.InclusaoDePCP, "DataInclusaoPCP", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.SituacaoAverbacao, "SituacaoAverbacao", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.DataAverbacao, "DataAverbacao", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.NumeroAverbacao, "NumeroAverbacao", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.RetiradaCTRN, "DataRetiradaCtrn", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.NumeroValePedagio, "NumeroValePedagio", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.ValorValePedagio, "ValorValePedagio", _tamanhoColunaMedia, Models.Grid.Align.right, true, false, false, false, TipoSumarizacao.sum);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.NumeroPedidoNotaFiscal, "NumeroPedidoNotaFiscal", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.TabelaFrete, "TabelaFrete", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.GuaritaEntrada, "GuaritaEntrada", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.GuaritaSaida, "GuaritaSaida", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.PrevisaoSaidaNavio, "ETS", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false).Ocultar(!ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.PrevisaoChegadaNavio, "ETA", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false).Ocultar(!ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.Navio, "Navio", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false).Ocultar(!ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.PortoOrigem, "PortoOrigem", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false).Ocultar(!ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.PortoDestino, "PortoDestino", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false).Ocultar(!ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.PortoTransbordo, "PortoTransbordo", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false).Ocultar(!ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.NumeroControleAbreviado, "NumeroDeControle", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.Containers, "Containeres", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false).Ocultar(!ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.NumeroOrdemServico, "NumeroOS", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.NumeroProposta, "NumeroProposta", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.TipoProposta, "TipoProposta", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.CargaIMO, "CargaIMO", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.QuantidadeNotaFiscal, "QuantidadeNF", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.DataFaturaDocumento, "DataFaturaDocumento", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.ProprietarioDoVeiculo, "NomeProprietarioVeiculo", _tamanhoColunaMedia, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.ContratoDeFrete, "ContratoFrete", _tamanhoColunaMedia, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.SerieCTe, "SerieCte", _tamanhoColunaMedia, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.NumeroCargaAgrupada, "NumeroCargaAgrupada", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.NumeroFatura, "NumeroFatura", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false).Ocultar(!ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.NumeroBoleto, "NumeroBoleto", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false).Ocultar(!ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.DataBoleto, "DataBoleto", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false).Ocultar(!ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.SituacaoFatura, "SituacaoFatura", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false).Ocultar(!ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.SituacaoBoleto, "SituacaoBoleto", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false).Ocultar(!ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.Lacres, "Lacres", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false).Ocultar(!ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.Taras, "Tara", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false).Ocultar(!ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.TipoContainers, "TipoContainers", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false).Ocultar(!ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.TipoSeparacao, "TipoSeparacao", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.DataInicioViagem, "DataInicioViagemFormatada", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.DataFimViagem, "DataFimViagemFormatada", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.ValorPIS, "ValorPIS", _tamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false, TipoSumarizacao.sum);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.ValorCOFINS, "ValorCOFINS", _tamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false, TipoSumarizacao.sum);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.ObservacaoInterna, "ObservacaoInterna", _tamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.ObservacaoCarga, "ObservacaoCarga", _tamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.OperadorDoPedido, "OperadorPedido", _tamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.TipoServico, "TipoServico", _tamanhoColunaPequena, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.CentroResultado, "CentroResultado", _tamanhoColunaMedia, Models.Grid.Align.right, false, false, false, true, false);

            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.QuantidadeVolumesCarga, "QtdVolumesCarga", _tamanhoColunaPequena, Models.Grid.Align.right, false, false, false, true, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.VolumesCTes, "VolumesCTe", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.PrevisaoEntregaPrimeiroPedido, "PrevisaoEntregaPrimeiroPedido", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.PrevisaoEntregaUltimoPedido, "PrevisaoEntregaUltimoPedido", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.HorasEmTransito, "HorasEmTransito", _tamanhoColunaPequena, Models.Grid.Align.right, false, false, false, true, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.DataRealizadaUltimaEntrega, "DataRealizadaUltimaEntrega", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.TotalEntregasRealizadas, "TotalEntregasRealizadas", _tamanhoColunaPequena, Models.Grid.Align.right, false, false, false, true, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.TotalEntregasDevolvidas, "TotalEntregasDevolvidas", _tamanhoColunaPequena, Models.Grid.Align.right, false, false, false, true, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.TotalEntregasPendentes, "TotalEntregasPendentes", _tamanhoColunaPequena, Models.Grid.Align.right, false, false, false, true, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.TotalEntregasBaixaManual, "TotalEntregasBaixaManual", _tamanhoColunaPequena, Models.Grid.Align.right, false, false, false, true, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.DistanciaRota, "DistanciaRota", _tamanhoColunaPequena, Models.Grid.Align.right, false, false, false, true, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.DataFinalizacaoDocumentosFiscais, "DataFinalizacaoDocumentosFiscaisFormatada", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.DataInicioGeracaoCTes, "DataInicioGeracaoCTesFormatada", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);

            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.DataPedido, "DataPedido", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.DataRoteirizacaoCarga, "DataRoteirizacaoCarga", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.DataProgramacaoCarga, "DataProgramacaoCarga", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.DataExpedicao, "DataExpedicao", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false).Ocultar(TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.NumeroDoPedidoCliente, "NumeroPedidoCliente", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);

            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.PrevisaoDeEntregaTransportador, "PrevisaoEntregaTransportador", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.ValorCustoDoFrete, "ValorCustoFrete", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.QuantidadeTotalProduto, "QuantidadeTotalProduto", _tamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false, TipoSumarizacao.sum);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.NumeroNotaFiscalProdutor, "NumeroNfProdutor", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.PorcentagemDeRefugo, "PorcentagemPerda", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.QuantidadeDeCaixas, "PesagemQuantidadeCaixas", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.PesoLiquidoPosPerdasCXS, "PesoLiquidoPosPerdas", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.ResultadoFinalProcessoCXS, "ResultadoFinalProcessoCaixas", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.ConfirmacaoDocumentos, "DataConfirmacaoDocumentoFormatada", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.ObservacaoDoCTe, "ObservacaoCTe", _tamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.Expedidor, "Expedidor", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.Recebedor, "Recebedor", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.LocRecebedor, "LocRecebedor", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.DataUltimaSaidaLoja, "DataUltimaSaidaRaio", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false).Ocultar(TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.NumeroContainer, "NumeroContainer", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false).Ocultar(ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.NumeroEXP, "NumeroEXP", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false).Ocultar(TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.ValorReceberCTe, "ValorReceberCTe", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, TipoSumarizacao.sum);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.DataEnvioUltimaNotaFiscal, "DataEnvioUltimaNfe", _tamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.Frota, "Frotas", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.DataVigenciaTabelaFrete, "DataVigenciaTabelaFrete", _tamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.PesoLiquido, "PesoLiquidoCarga", _tamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.PercentualDeBonificacaoAoTransportador, "PercentualBonificsacaoTransportador", _tamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false, false);

            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.DataAceiteTransportador, "DataAceiteTransportadorFormatada", _tamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.UsuarioAceiteTransportador, "UsuarioAceiteTransportador", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.NaoComparecimento, "NaoComparecimentoDescricao", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.TipoDaPropriedade, "TipoPropriedade", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.ModeloDaCarroceria, "ModeloCarroceria", _tamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, false);

            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.PesagemInicial, "PesagemInicial", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.PesagemFinal, "PesagemFinal", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false).Ocultar(TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.PesoLiquidoPesagem, "PesoLiquidoPesagem", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false).Ocultar(TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.LacrePesagem, "NumeroLacrePesagem", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false).Ocultar(TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.LoteInternoPesagem, "LoteInternoPesagem", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false).Ocultar(TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.NumeroContainerVeiculo, "NumeroContainerVeiculo", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.ValorDoFretePelaTabela, "ValorFreteTabelaFrete", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.LocalDeRetiradaContainer, "LocalRetiradaContainer", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.PreCarga, "CargaPreCarga", _tamanhoColunaPequena, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.Terceiro, "Terceiro", _tamanhoColunaPequena, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.Redespacho, "Redespacho", _tamanhoColunaPequena, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.HorarioEncaixado, "HorarioEncaixadoDescricao", _tamanhoColunaPequena, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.KmRota, "KmRota", _tamanhoColunaPequena, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.ValorKmRota, "ValorKmRota", _tamanhoColunaPequena, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.ClienteFinal, "ClienteFinal", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.TaraContainer, "TaraContainer", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.MaxGross, "MaxGross", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.ProtocoloIntegracao, "Protocolo", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Nº Doc. Originário", "NumeroDocumentoOriginario", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Data Nº Doc. Originário", "DataNumeroDocumentoOriginario", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.NaturezaOP, "NaturezaOP", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.KMExecutado, "KMExecutado", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Quantida de Anexos", "QuantidadeAnexos", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Tipo de Anexo", "TipoAnexo", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("ExternalID 1", "ExternalDT1", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("ExternalID 2", "ExternalDT2", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.CNPJLocalRetiradaContainer, "CNPJLocalRetiradaContainerFormatado", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.EncerramentoManualViagem, "PossuiEncerramentoManual", _tamanhoColunaPequena, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.JustificativaEncerramentoManual, "JustificativaEncerramento", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.ObservacaoEncerramentoManual, "ObservacaoEncerramento", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.KMPlanejado, "KMPrevisto", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.KMRealizado, "KMRealizado", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.ValorNFSemPallet, "ValorNFSemPallet", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Carga Relacionada", "CargaRelacionada", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Canal Entrega", "CanalEntrega", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Justificativa", "JustificativaCargaRelacionada", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.CustoFrete, "CustoFrete", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Lacre", propriedade: "CargaLacre", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.Cubagem, "Cubagem", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Aprovador, "Aprovador", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.SituacaoAprovacao, "SituacaoAprovacaoFormatada", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.DataAprovacao, "DataAutorizacaoFormatada", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.PedagioPagoTerceiro, "PedagioPagoTerceiro", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.OutrosDescontosTerceiro, "OutrosDescontosTerceiro", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.IRPFTerceiro, "IRPFTerceiro", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.INSSTerceiro, "INSSTerceiro", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.SESTSENATTerceiro, "SESTSENATTerceiro", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.OutrasTaxasTerceiro, "OutrasTaxasTerceiro", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Valor Total Terceiro", "ValorTotalTerceiro", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("N° Contrato Terceiro", "ContratoTerceiro", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.QuantidadeCTeAnterior, "QuantidadeCTeAnterior", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.PacotesIntegrados, "QuantidadePacotes", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.PacotesColetados, "QuantidadePacotesColetados", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.PalletsCarregadosNestaCarga, "PalletsCarregadosNestaCarga", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false, 3);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.NumeroContratoFrete, "NumeroContratoFrete", _tamanhoColunaMedia, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.DescricaoContratoFrete, "DescricaoContratoFrete", _tamanhoColunaMedia, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.DataInicialContratoFrete, "DataInicialContratoFrete", _tamanhoColunaMedia, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.DataFinalContratoFrete, "DataFinalContratoFrete", _tamanhoColunaMedia, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.TipoContratoFrete, "TipoContratoFrete", _tamanhoColunaMedia, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Agrupamento das Cargas", "AgrupamentoCargas", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false, 3);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.RotaRecorrente, "RotaRecorrenteDescricao", _tamanhoColunaPequena, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.CodigoIntegracaoRota, "CodigoIntegracaoRota", _tamanhoColunaPequena, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.AprovadorDocumento, "AprovadorDocumento", _tamanhoColunaPequena, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.DataAprovacaoDocumento, "DataAprovacaoDocumentoFormatada", _tamanhoColunaPequena, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.SituacaoAprovacaoDocumento, "SituacaoAprovacaoDocumento", _tamanhoColunaPequena, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.ParametroBaseCalculoFrete, "ParametroBaseCalculoFreteDescricao", _tamanhoColunaPequena, Models.Grid.Align.center, false, false);

            grid.AdicionarCabecalho("Tipo OS Convertido", "TipoOSConvertidoDescricao", _tamanhoColunaPequena, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho("Tipo OS", "TipoOSDescricao", _tamanhoColunaPequena, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho("Provedor OS", "ProvedorOS", _tamanhoColunaPequena, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho("Direcionamento Custo Extra", "DirecionamentoCustoExtraDescricao", _tamanhoColunaPequena, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho("Status Custo Extra", "StatusCustoExtraDescricao", _tamanhoColunaPequena, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.ValorFreteProvedor, "ValorFreteIntegracao", _tamanhoColunaPequena, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.DiferencaFreteValorOperadorValorTabelaFrete, "DiferencaFreteValorOperadorValorTabelaFrete", _tamanhoColunaPequena, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho("Usuario Alteração Frete", "UsuarioAlteracaoFrete", _tamanhoColunaPequena, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho("Justificativa Custo Extra", "JustificativaCustoExtra", _tamanhoColunaPequena, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho("Setor Responsavel", "SetorResponsavel", _tamanhoColunaPequena, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.NumeroTransferencia, nameof(RelatorioCarga.NumeroTransferencia), _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.Alocacao, nameof(RelatorioCarga.Alocacao), _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            //verificar se possui integracao com gnre            
            Repositorio.Embarcador.Configuracoes.IntegracaoGNRE repIntegracaoGNRE = new Repositorio.Embarcador.Configuracoes.IntegracaoGNRE(unitOfWork, cancellationToken);
            var integracao = await repIntegracaoGNRE.BuscarPrimeiroRegistroAsync();
            if (integracao != null)
            {
                if (integracao.PossuiIntegracaoGNRE == true)
                    grid.AdicionarCabecalho("ICMS GNRE", "ICMSGNRE", _tamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false, false);
            }

            if (configuracaoDigitalCom?.ValidacaoTAGDigitalCom ?? false)
                grid.AdicionarCabecalho("TAG Pedágio", "DescricaoTAGPedagio", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);

            //Colunas do Fluxo de Pátio
            if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa(unitOfWork);

                string aux = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterDescricaoEtapa(EtapaFluxoGestaoPatio.InformarDoca, TipoFluxoGestaoPatio.Origem).Descricao;
                grid.AdicionarCabecalho("D. " + aux, "DataDocaInformadaFormatada", 10, Models.Grid.Align.center, false, false, false, false, false);
                aux = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterDescricaoEtapa(EtapaFluxoGestaoPatio.ChegadaVeiculo, TipoFluxoGestaoPatio.Origem).Descricao;
                grid.AdicionarCabecalho("D. " + aux, "DataChegadaVeiculoFormatada", 10, Models.Grid.Align.center, false, false, false, false, false);
                aux = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterDescricaoEtapa(EtapaFluxoGestaoPatio.Faturamento, TipoFluxoGestaoPatio.Origem).Descricao;
                grid.AdicionarCabecalho("D. " + aux, "DataFaturamentoFormatada", 10, Models.Grid.Align.center, false, false, false, false, false);
            }

            grid.AdicionarCabecalho("% Exec", "PercentualExecucao", _tamanhoColunaPequena, Models.Grid.Align.center, false, false);

            //Colunas montadas dinamicamente
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork, cancellationToken);
            Repositorio.TipoDeOcorrenciaDeCTe repOcorrencia = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork, cancellationToken);

            List<Dominio.Entidades.Embarcador.Frete.ComponenteFrete> componentes = await repComponenteFrete.BuscarTodosAtivosAsync(_numeroMaximoComplementos);

            int ultimaColunaDinanica = 1;

            for (int i = 0; i < componentes.Count; i++)
            {
                if (i < _numeroMaximoComplementos)
                {
                    if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                        grid.AdicionarCabecalho(componentes[i].Descricao, "ValorComponente" + ultimaColunaDinanica.ToString(), _tamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false, TipoSumarizacao.sum, componentes[i].Codigo);
                    else
                    {
                        bool exibirComponenteTransportador = !await repOcorrencia.ExisteBloqueioTransportadorPorComponenteAsync(componentes[i].Codigo);

                        if (exibirComponenteTransportador)
                            grid.AdicionarCabecalho(componentes[i].Descricao, "ValorComponente" + ultimaColunaDinanica.ToString(), _tamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false, TipoSumarizacao.sum, componentes[i].Codigo);
                    }

                    ultimaColunaDinanica++;
                }
                else
                    break;
            }

            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.FreteSimulado, "ValorFreteSimulacao", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.ValorToneladaSimulado, "ValorToneladaSimulado", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.SerieNFe, "SerieNfe", _tamanhoColunaMedia, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.ComponenteDuplicado, "PossuiComponenteDuplicado", _tamanhoColunaMedia, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.NumeroPagamento, "NumeroPagamento", _tamanhoColunaMedia, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.DataEnvioPagamento, "DataEnvioPagamentoFormatada", _tamanhoColunaMedia, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.SituacaoPagamento, "SituacaoPagamentoDescricao", _tamanhoColunaMedia, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.DiferencaValores, "DiferencaValores", _tamanhoColunaMedia, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.ResponsavelValePedagio, "ResponsavelValePedagio", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.CentroDeCustoViagem, "CentroDeCustoViagemDescricao", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.GrupoProduto, "GrupoProduto", _tamanhoColunaGrande, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.SituacaoBRK, "SituacaoBRKFormatada", _tamanhoColunaMedia, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.MensagemBRK, "MensagemBRK", _tamanhoColunaMedia, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho("Tipo Terceiro", "TipoTerceiro", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.CargaBloqueada, "CargaBloqueada", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.ValorTotalMercadoriaDosPedidos, "ValorTotalMercadoriaDosPedidos", _tamanhoColunaMedia, Models.Grid.Align.right, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.OutrosNumerosDaCarga, "OutrosNumerosDaCarga", _tamanhoColunaMedia, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.DataInclusaoDadosTransporte, "DataInclusaoDadosTransporte", _tamanhoColunaMedia, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.CargaReentrega, "CargaReentrega", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.NumeroOrdemServicoPedidos, "NumeroOrdemServico", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.MotivoSolicitacaoFrete, "MotivoSolicitacaoFrete", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.ObservacaoSolicitacaoFrete, "ObservacaoSolicitacaoFrete", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.NumeroCTes, "NumeroCTes", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.DataVinculoTracao, "DataVinculoTracaoFormatada", _tamanhoColunaMedia, Models.Grid.Align.left, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.DataVinculoReboque, "DataVinculoReboqueFormatada", _tamanhoColunaMedia, Models.Grid.Align.left, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.DataVinculoMotorista, "DataVinculoMotoristaFormatada", _tamanhoColunaMedia, Models.Grid.Align.left, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.LocalVinculo, "DescricaoLocalVinculo", _tamanhoColunaMedia, Models.Grid.Align.left, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.NumeroDoca, "NumeroDoca", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.ObservacaoCarga, "Observacao", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Peso Reentrega", "PesoReentrega", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Peso Total", "PesoTotal", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.ObservacoesTransportador, "ObservacaoTransportador", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);

            return grid;
        }

        #endregion
    }
}
