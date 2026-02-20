using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.CTe;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using Microsoft.AspNetCore.Mvc;
using Repositorio;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.CTe
{
    [Area("Relatorios")]
    [CustomAuthorize("Relatorios/CTe/CTes")]
    public class CTesController : Relatorios.AutomatizacaoGeracaoRelatorioController<Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaCteRelatorio>
    {
        #region Construtores

        public CTesController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Atributos Privados

        private Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R005_CTes;
        private int UltimaColunaDinanica = 1;
        private int NumeroMaximoLayout = 6;
        private int NumeroMaximoComplementos = 60;
        private decimal TamanhoColunasValores = (decimal)1.75;
        private decimal TamanhoColunasParticipantes = (decimal)5.50;
        private decimal TamanhoColunasLocalidades = 3;

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

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(CodigoControleRelatorio, TipoServicoMultisoftware, "Relatório de CT-e(s)", "CTe", "CTes.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "", "", codigoRelatorio, unitOfWork, true, true);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio retorno = gridRelatorio.RetornoGridPadraoRelatorio(await ObterGridPadrao(unitOfWork, cancellationToken), relatorio);

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
                DateTime dataInicial = Request.GetDateTimeParam("DataInicialEmissao");
                DateTime dataFinal = Request.GetDateTimeParam("DataFinalEmissao");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();

                Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaCteRelatorio filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.CTes.CTe servicoRelatorioCTe = new Servicos.Embarcador.Relatorios.CTes.CTe(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioCTe.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.CTe.CTes.CTe> listaCTes, out int countRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

                grid.AdicionaRows(listaCTes);
                grid.setarQuantidadeTotal(countRegistros);

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

        public async Task<IActionResult> GerarRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string stringConexao = _conexao.StringConexao;

                int codigoEmpresa = Empresa?.Codigo ?? 0;

                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Servicos.Embarcador.Relatorios.Relatorio svcRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = svcRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaCteRelatorio filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await svcRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (Dominio.Excecoes.Embarcador.ServicoException servicoException)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, servicoException.Message);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o relatório.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaCteRelatorio ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);

            Repositorio.Embarcador.Configuracoes.ConfiguracaoRelatorio repConfiguracaoRelatorio = new Repositorio.Embarcador.Configuracoes.ConfiguracaoRelatorio(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoRelatorio configuracaoRelatorio = repConfiguracaoRelatorio.BuscarConfiguracaoPadrao();

            List<int> codigosTransportador = Request.GetListParam<int>("Transportador");

            if (codigosTransportador.Count > 0)
                codigosTransportador.AddRange(repositorioEmpresa.BuscarCodigosFiliaisVinculadas(codigosTransportador));

            Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaCteRelatorio filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaCteRelatorio()
            {
                cargaEmissaoFinalizada = Request.GetNullableBoolParam("CargaEmissaoFinalizada"),
                codigoCarga = Request.GetListParam<int>("Carga"),
                codigoCFOP = Request.GetIntParam("CFOP"),
                codigoContratoFrete = Request.GetIntParam("ContratoFrete"),
                codigoDestino = Request.GetIntParam("Destino"),
                codigoOrigem = Request.GetIntParam("Origem"),
                CodigosTransportador = codigosTransportador,
                cpfCnpjDestinatarios = Request.GetListParam<double>("Destinatario"),
                cpfCnpjRemetente = Request.GetStringParam("Remetente").ObterSomenteNumeros().ToDouble(),
                cpfCnpjTerceiro = Request.GetStringParam("Terceiro").ObterSomenteNumeros().ToDouble(),
                ctesNaoExistentesEmFaturas = Request.GetBoolParam("CTesNaoExistentesEmFaturas"),
                ctesNaoExistentesEmMinutas = Request.GetBoolParam("CTesNaoExistentesEmMinutas"),
                cteVinculadoACarga = Request.GetNullableBoolParam("CTeVinculadoACarga"),
                dataInicialAnulacao = Request.GetDateTimeParam("DataInicialAnulacao"),
                dataInicialAutorizacao = Request.GetDateTimeParam("DataInicialAutorizacao"),
                dataInicialCancelamento = Request.GetDateTimeParam("DataInicialCancelamento"),
                dataInicialEmissao = Request.GetDateTimeParam("DataInicialEmissao"),
                dataInicialEntrega = Request.GetDateTimeParam("DataInicialEntrega"),
                dataInicialFatura = Request.GetDateTimeParam("DataInicialFatura"),
                dataInicialImportacao = Request.GetDateTimeParam("DataInicialImportacao"),
                dataFinalAnulacao = Request.GetDateTimeParam("DataFinalAnulacao"),
                dataFinalAutorizacao = Request.GetDateTimeParam("DataFinalAutorizacao"),
                dataFinalCancelamento = Request.GetDateTimeParam("DataFinalCancelamento"),
                dataFinalEmissao = Request.GetDateTimeParam("DataFinalEmissao"),
                dataFinalEntrega = Request.GetDateTimeParam("DataFinalEntrega"),
                dataFinalFatura = Request.GetDateTimeParam("DataFinalFatura"),
                dataFinalImportacao = Request.GetDateTimeParam("DataFinalImportacao"),
                estadoDestino = Request.GetStringParam("EstadoDestino"),
                estadoOrigem = Request.GetStringParam("EstadoOrigem"),
                exibirNotasFiscais = Request.GetBoolParam("ExibirNotasFiscais"),
                AnuladoGerencialmente = Request.GetBoolParam("AnuladoGerencialmente"),
                faturado = Request.GetNullableBoolParam("SituacaoFaturamentoCTe"),
                gruposPessoas = Request.GetListParam<int>("GruposPessoas"),
                gruposPessoasDiferente = Request.GetListParam<int>("GruposPessoasDiferente"),
                modeloDocumento = Request.GetListParam<int>("ModeloDocumento"),
                nfe = Request.GetStringParam("NFe"),
                numeroInicial = Request.GetIntParam("NumeroInicial"),
                numeroFinal = Request.GetIntParam("NumeroFinal"),
                pago = Request.GetNullableBoolParam("Pago"),
                pedido = Request.GetStringParam("Pedido"),
                possuiDataEntrega = Request.GetNullableBoolParam("PossuiDataEntrega"),
                possuiNFSManual = Request.GetNullableBoolParam("PossuiNFSManual"),
                preCarga = Request.GetStringParam("PreCarga"),
                serie = Request.GetIntParam("Serie"),
                situacaoFatura = Request.GetNullableEnumParam<SituacaoFatura>("SituacaoFatura"),
                tipoDocumentoCreditoDebito = Request.GetEnumParam("TipoDocumentoCreditoDebito", TipoDocumentoCreditoDebito.Todos),
                statusCTe = Request.GetListParam<string>("Situacao"),
                tipoPropriedadeVeiculo = Request.GetStringParam("TipoPropriedadeVeiculo"),
                tiposCTe = Request.GetListEnumParam<TipoCTE>("TipoCTe"),
                tiposServicos = Request.GetListParam<int>("TipoServico"),
                tiposTomadores = Request.GetListParam<int>("TipoTomador"),
                SegmentoVeiculo = Request.GetListParam<int>("SegmentoVeiculo"),
                TipoProposta = Request.GetListEnumParam<TipoPropostaMultimodal>("TipoProposta"),
                NumeroBooking = Request.GetStringParam("NumeroBooking"),
                NumeroOS = Request.GetStringParam("NumeroOS"),
                NumeroControle = Request.GetStringParam("NumeroControle"),
                SituacaoCarga = Request.GetEnumParam<SituacaoCarga>("SituacaoCarga"),
                SituacoesCargaMercante = Request.GetListEnumParam<SituacaoCargaMercante>("SituacaoCargaMercante"),
                CodigoPortoOrigem = Request.GetIntParam("PortoOrigem"),
                CodigoPortoDestino = Request.GetIntParam("PortoDestino"),
                CodigoViagem = Request.GetIntParam("Viagem"),
                CodigoContainer = Request.GetIntParam("Container"),
                CpfCnpjTomadores = Request.GetListParam<double>("Tomador"),
                TipoServicoMultisoftware = TipoServicoMultisoftware,
                CodigoMotorista = Request.GetIntParam("Motorista"),
                NaoExibirValorFreteCTeComplementar = ConfiguracaoEmbarcador.NaoExibirValorFreteCTeComplementarRelatorioCTe,
                DataInicialColeta = Request.GetDateTimeParam("DataInicialColeta"),
                DataFinalColeta = Request.GetDateTimeParam("DataFinalColeta"),
                GruposPessoasRemetente = Request.GetListParam<int>("GruposPessoasRemetente"),
                SomenteCTeSubstituido = Request.GetBoolParam("SomenteCTeSubstituido"),
                ApenasCTeEnviadoMercante = Request.GetBoolParam("ApenasCTeEnviadoMercante"),
                TipoServicoMultimodal = Request.GetListEnumParam<TipoServicoMultimodal>("TipoServicoMultiModal"),
                VeioPorImportacao = Request.GetEnumParam<OpcaoSimNaoPesquisa>("VeioPorImportacao"),
                CodigosCentroResultado = Request.GetListParam<int>("CentroResultado"),
                CodigosTipoOcorrencia = Request.GetListParam<int>("TipoOcorrencia"),
                DataPagamentoInicial = Request.GetDateTimeParam("DataPagamentoInicial"),
                DataPagamentoFinal = Request.GetDateTimeParam("DataPagamentoFinal"),
                DataVencimentoInicial = Request.GetDateTimeParam("DataVencimentoInicial"),
                DataVencimentoFinal = Request.GetDateTimeParam("DataVencimentoFinal"),
                ChaveCTe = Request.GetStringParam("ChaveCTe"),
                NumeroDocumentoRecebedor = Request.GetStringParam("NumeroDocumentoRecebedor"),
                NumeroPedidoCliente = Request.GetStringParam("NumeroPedidoCliente"),
                CST = Request.GetListEnumParam<TipoICMS>("CST"),
                ModeloVeiculo = Request.GetIntParam("ModeloVeiculo"),
                TipoCarroceria = Request.GetEnumParam<TipoCarroceria>("TipoCarroceria"),
                DataConfirmacaoDocumentosInicial = Request.GetDateTimeParam("DataConfirmacaoDocumentosInicial"),
                DataConfirmacaoDocumentosFinal = Request.GetDateTimeParam("DataConfirmacaoDocumentosFinal"),
                TipoProprietarioVeiculo = Request.GetNullableEnumParam<Dominio.Enumeradores.TipoProprietarioVeiculo>("TipoProprietarioVeiculo"),
                TipoModal = Request.GetEnumParam<TipoModal>("TipoModal"),
                PermiteGerarFaturamento = Request.GetNullableBoolParam("PermiteGerarFaturamento"),
                CodigoContratoFreteTerceiro = Request.GetIntParam("NumeroContratoFreteTerceiro"),
                Vendedor = Request.GetListParam<int>("Vendedor"),
                FuncionarioResponsavel = Request.GetListParam<int>("FuncionarioResponsavel"),
                CodigosFiliais = ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork),
                CodigosFiliaisVenda = ObterListaCodigoFilialVendaPermitidasOperadorLogistica(unitOfWork),
                CodigosRecebedores = ObterListaCnpjCpfRecebedorPermitidosOperadorLogistica(unitOfWork),
                TipoOSConvertido = Request.GetListEnumParam<TipoOSConvertido>("TipoOSConvertido"),
                TipoOS = Request.GetListEnumParam<TipoOS>("TipoOS"),
                ProvedorOS = Request.GetDoubleParam("ProvedorOS"),
                CentroDeCustoViagemCodigo = Request.GetIntParam("CentroDeCustoViagemCodigo"),
                CNPJDivergenteCTeMDFe = Request.GetBoolParam("CNPJDivergenteCTeMDFe"),
                TipoEmissao = Request.GetEnumParam<TipoEmissao>("TipoEmissao"),
                RetornarDestinatarioDaNFeQuandoTipoForNFSeNoRelatorioDeCTes = configuracaoRelatorio.RetornarDestinatarioDaNFeQuandoTipoForNFSeNoRelatorioDeCTes,
                NumeroCRT = Request.GetStringParam("NumeroCRT"),
                MicDTA = Request.GetStringParam("MicDTA"),
            };

            int codigoFilial = Request.GetIntParam("Filial");
            int codigoFilialVenda = Request.GetIntParam("FilialVenda");
            List<int> codigosTipoOperacao = Request.GetListParam<int>("TipoOperacao");
            List<int> codigosVeiculos = Request.GetListParam<int>("Veiculo");
            List<int> codigosTipoDeCarga = Request.GetListParam<int>("TipoDeCarga");
            List<int> codigosCtes = Request.GetListParam<int>("CTe");
            List<int> codigosFuncionario = Request.GetListParam<int>("FuncionarioResponsavel");

            filtrosPesquisa.codigosFilial = codigoFilial == 0 ? ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork) : new List<int>() { codigoFilial };
            filtrosPesquisa.codigosFilialVenda = codigoFilialVenda == 0 ? ObterListaCodigoFilialVendaPermitidasOperadorLogistica(unitOfWork) : new List<int>() { codigoFilialVenda };
            filtrosPesquisa.codigosTipoCarga = codigosTipoDeCarga.Count <= 0 ? ObterListaCodigoTipoCargaPermitidosOperadorLogistica(unitOfWork) : codigosTipoDeCarga;
            filtrosPesquisa.codigosTipoOperacao = codigosTipoOperacao.Count <= 0 ? ObterListaCodigoTipoOperacaoPermitidosOperadorLogistica(unitOfWork) : codigosTipoOperacao;
            filtrosPesquisa.codigosCTes = codigosCtes;
            filtrosPesquisa.FuncionarioResponsavel = codigosFuncionario;

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
            {
                if (!Empresa.Matriz.Any())
                {
                    Repositorio.Empresa empresa = new Repositorio.Empresa(unitOfWork);
                    List<int> codigosEmpresa = empresa.BuscarCodigoMatrizEFiliais(Usuario.Empresa?.CNPJ_SemFormato);
                    filtrosPesquisa.CodigosTransportador = codigosEmpresa?.Count > 0 ? codigosEmpresa : null;
                }
                else
                    filtrosPesquisa.CodigosTransportador = new List<int>() { Usuario.Empresa.Codigo };
            }
            else if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Terceiros)
                filtrosPesquisa.cpfCnpjTerceiro = Usuario.ClienteTerceiro.CPF_CNPJ;

            if (codigosVeiculos?.Count > 0)
            {
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);

                filtrosPesquisa.placasVeiculos = repVeiculo.BuscarPlacas(codigosVeiculos);
            }

            return filtrosPesquisa;
        }

        #endregion Métodos Protegidos Sobrescritos

        #region Métodos Privados

        private async Task<Models.Grid.Grid> ObterGridPadrao(Repositorio.UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            UltimaColunaDinanica = 1;

            Repositorio.LayoutEDI repLayoutEDI = new Repositorio.LayoutEDI(unidadeDeTrabalho, cancellationToken);
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unidadeDeTrabalho, cancellationToken);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unidadeDeTrabalho, cancellationToken);
            Repositorio.Embarcador.Pedidos.Stage repositorioStage = new Repositorio.Embarcador.Pedidos.Stage(unidadeDeTrabalho, cancellationToken);
            Repositorio.TipoDeOcorrenciaDeCTe repOcorrencia = new Repositorio.TipoDeOcorrenciaDeCTe(unidadeDeTrabalho, cancellationToken);

            List<Dominio.Entidades.Embarcador.Frete.ComponenteFrete> componentes = await repComponenteFrete.BuscarTodosAtivosAsync();
            List<Dominio.Entidades.LayoutEDI> layoutes = await repLayoutEDI.BuscarParaRelatoriosAsync();

            Models.Grid.Grid grid = new Models.Grid.Grid();

            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false);

            grid.AdicionarCabecalho("Número", "NumeroCTe", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Série CTe", "SerieCTe", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("RPS", "RPS", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Nº da Carga", "NumeroCarga", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Nº da Carga Agrupamento", "NumeroCargaAgrupamento", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Nº da Pré Carga", "PreCarga", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, true, false);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                grid.AdicionarCabecalho("Nº do Pedido Embarcador", "NumeroPedido", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
            else
                grid.AdicionarCabecalho("Nº do Pedido", "NumeroPedido", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                grid.AdicionarCabecalho("Nº do Pedido Interno", "NumeroPedidoInterno", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
            else
                grid.AdicionarCabecalho("NumeroPedidoInterno", false);

            grid.AdicionarCabecalho("Doc", "AbreviacaoModeloDocumentoFiscal", TamanhoColunasValores, Models.Grid.Align.center, true, false, false, true, true);

            grid.AdicionarCabecalho("Tipo do CT-e", "DescricaoTipoCTe", TamanhoColunasValores, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Tipo do Serviço", "DescricaoTipoServico", TamanhoColunasValores, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Tipo do Tomador", "DescricaoTipoTomador", TamanhoColunasValores, Models.Grid.Align.center, true, false, false, false, false);

            grid.AdicionarCabecalho("Status", "StatusCTe", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Data Emissão", "DataEmissaoFormatada", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Ano Emissão", "AnoEmissao", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Mês Emissão", "MesEmissao", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Data Autorização", "DataAutorizacaoFormatada", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Data Cancelamento", "DataCancelamento", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Data Anulação", "DataAnulacao", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Data Importação", "DataImportacao", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Data Vínculo Carga", "DataVinculoCarga", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                grid.AdicionarCabecalho("Data Vencimento", "DataVencimentoTituloFormatada", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            else
                grid.AdicionarCabecalho("Data Vencimento", "DataVencimento", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);

            grid.AdicionarCabecalho("Data Entrega", "DataEntrega", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Data Criação Carga", "DataCriacaoCarga", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Situação Carga", "SituacaoCargaFormatada", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);

            grid.AdicionarCabecalho("Situação Título", "DescricaoStatusTitulo", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Nº Pré Fatura", "NumeroPreFatura", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, true, false);

            grid.AdicionarCabecalho("CNPJ Filial", "CNPJFilial", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Filial", "Filial", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Filial Venda", "FilialVenda", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);

            grid.AdicionarCabecalho("Código Remetente", "CodigoRemetente", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("CPF/CNPJ Remetente", "CPFCNPJRemetente", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("IE Remetente", "IERemetente", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Remetente", "Remetente", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Código Endereço Remetente", "CodigoEnderecoRemetente", TamanhoColunasLocalidades, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Endereço Remetente", "EnderecoRemetente", TamanhoColunasLocalidades, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Loc. Remetente", "LocalidadeRemetente", TamanhoColunasLocalidades, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("UF Remetente", "UFRemetente", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Grupo Remetente", "GrupoRemetente", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Categoria Remetente", "CategoriaRemetente", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, false);
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                grid.AdicionarCabecalho("Cód. Documentação Remetente", "CodigoDocumentoRemetente", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);

            grid.AdicionarCabecalho("Código Expedidor", "CodigoExpedidor", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("CPF/CNPJ Expedidor", "CPFCNPJExpedidor", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("IE Expedidor", "IEExpedidor", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Expedidor", "Expedidor", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Loc. Expedidor", "LocalidadeExpedidor", TamanhoColunasLocalidades, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("UF Expedidor", "UFExpedidor", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, true, false);
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                grid.AdicionarCabecalho("Cód. Documentação Expedidor", "CodigoDocumentoExpedidor", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);

            grid.AdicionarCabecalho("Código Recebedor", "CodigoRecebedor", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("CPF/CNPJ Recebedor", "CPFCNPJRecebedor", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("IE Recebedor", "IERecebedor", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Recebedor", "Recebedor", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Loc. Recebedor", "LocalidadeRecebedor", TamanhoColunasLocalidades, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("UF Recebedor", "UFRecebedor", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Rua Recebedor", "RuaRecebedor", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Nº Recebedor", "NumeroRecebedor", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, false);
            grid.AdicionarCabecalho("Bairro Recebedor", "BairroRecebedor", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                grid.AdicionarCabecalho("Cód. Documentação Recebedor", "CodigoDocumentoRecebedor", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);

            grid.AdicionarCabecalho("Código Destinatário", "CodigoDestinatario", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("CPF/CNPJ Destinatario", "CPFCNPJDestinatario", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("IE Destinatário", "IEDestinatario", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Destinatário", "Destinatario", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Código Endereço Destinatário", "CodigoEnderecoDestinatario", TamanhoColunasLocalidades, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Endereço Destinatário", "EnderecoDestinatario", TamanhoColunasLocalidades, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Bairro Destinatário", "BairroDestinatario", TamanhoColunasLocalidades, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("CEP Destinatário", "CEPDestinatario", TamanhoColunasLocalidades, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Loc. Destinatário", "LocalidadeDestinatario", TamanhoColunasLocalidades, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("UF Destinatário", "UFDestinatario", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Grupo Destinatário", "GrupoDestinatario", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Categoria Destinatário", "CategoriaDestinatario", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, false);
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                grid.AdicionarCabecalho("Cód. Documentação Destinatário", "CodigoDocumentoDestinatario", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);

            grid.AdicionarCabecalho("Código Tomador", "CodigoIntegracaoTomador", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("CPF/CNPJ Tomador", "CPFCNPJTomador", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("IE Tomador", "IETomador", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Tomador", "Tomador", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("UF Tomador", "UFTomador", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, true, false);
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                grid.AdicionarCabecalho("Cód. Documentação Tomador", "CodigoDocumentoTomador", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);

            grid.AdicionarCabecalho("IBGE início da Prestação", "IBGEInicioPrestacao", TamanhoColunasLocalidades, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Início da Prestação", "InicioPrestacao", TamanhoColunasLocalidades, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("UF Início", "UFInicioPrestacao", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("IBGE fim da Prestação", "IBGEFimPrestacao", TamanhoColunasLocalidades, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Fim da Prestação", "FimPrestacao", TamanhoColunasLocalidades, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("UF Fim", "UFFimPrestacao", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, true, false);

            grid.AdicionarCabecalho("CNPJ Transportador", "CNPJTransportadorFormatada", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("CNPJ MDF-e", "CnpjMdfeFormatada", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, true, false);

            if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
            {
                grid.AdicionarCabecalho("Razão Social Transp.", "RazaoSocialTransportador", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
                grid.AdicionarCabecalho("Nome Fantasia Transp.", "NomeFantasiaTransportador", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
                grid.AdicionarCabecalho("UF Transportador", "UFTransportador", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            }
            else
            {
                grid.AdicionarCabecalho("RazaoSocialTransportador", false);
                grid.AdicionarCabecalho("NomeFantasiaTransportador", false);
                grid.AdicionarCabecalho("UFTransportador", false);
            }

            grid.AdicionarCabecalho("Tipo Pagamento", "DescricaoTipoPagamento", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Frota", "Frota", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Peso (kg)", "PesoKg", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Peso Líquido (kg)", "PesoLiquidoKg", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Volumes", "Volumes", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Metros Cúbicos", "MetrosCubicos", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, TipoSumarizacao.nenhum, 0, 6);
            grid.AdicionarCabecalho("Pallets", "Pallets", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("CFOP", "CFOP", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("CST", "CSTFormatada", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("B.C. do ICMS", "BaseCalculoICMS", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Alíquota ICMS", "AliquotaICMS", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor do ICMS", "ValorICMS", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Alíquota ISS", "AliquotaISS", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor do ISS", "ValorISS", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Valor do ISS Retido", "ValorISSRetido", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Alíquota PIS", "AliquotaPIS", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor do PIS", "ValorPIS", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Alíquota COFINS", "AliquotaCOFINS", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor do COFINS", "ValorCOFINS", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Valor do Frete", "ValorFrete", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Valor a Receber", "ValorReceber", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Valor da Prestação", "ValorPrestacao", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Valor Total com Imposto Parcial", "ValorSemImposto", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Valor da Mercadoria", "ValorMercadoria", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, TipoSumarizacao.sum);

            grid.AdicionarCabecalho("CST IBS/CBS", "CSTIBSCBS", TamanhoColunasValores, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Classificação Tributária IBS/CBS", "ClassificacaoTributariaIBSCBS", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Base de Cálculo IBS/CBS", "BaseCalculoIBSCBS", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Alíquota IBS Estadual", "AliquotaIBSEstadual", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Percentual Redução IBS Estadual", "PercentualReducaoIBSEstadual", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor Redução IBS Estadual", "ValorReducaoIBSEstadual", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor IBS Estadual", "ValorIBSEstadual", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Alíquota IBS Municipal", "AliquotaIBSMunicipal", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Percentual Redução IBS Municipal", "PercentualReducaoIBSMunicipal", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor Redução IBS Municipal", "ValorReducaoIBSMunicipal", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor IBS Municipal", "ValorIBSMunicipal", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Alíquota CBS", "AliquotaCBS", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Percentual Redução CBS", "PercentualReducaoCBS", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor Redução CBS", "ValorReducaoCBS", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor CBS", "ValorCBS", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);

            grid.AdicionarCabecalho("Veículo", "Veiculo", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Proprietário do Veículo", "NomeProprietarioVeiculo", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Segmento do Veículo", "SegmentoVeiculo", TamanhoColunasParticipantes, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Motorista", "Motorista", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Nota Fiscal", "NumeroNotaFiscal", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Chave NFe", "ChaveNotaFiscal", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Data Emissão Doc Anterior", "DataNFEmissao", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Número Doc Anterior", "NumeroDocumentoAnterior", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Chave CTe", "ChaveCTe", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Protocolo Aut.", "ProtocoloAutorizacao", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Protocolo Inut./Canc.", "ProtocoloInutilizacaoCancelamento", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                grid.AdicionarCabecalho("Minuta", "NumeroMinuta", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, true, false);
            //else
            //    grid.AdicionarCabecalho("Minuta", false);

            grid.AdicionarCabecalho("Observação", "Observacao", TamanhoColunasParticipantes, Models.Grid.Align.left, false, false, false, false, false);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
            {
                grid.AdicionarCabecalho("Contrato de Frete", "ContratoFrete", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
                grid.AdicionarCabecalho("Fechamento de Frete", "NumeroFechamentoFrete", TamanhoColunasParticipantes, Models.Grid.Align.left, false, false, false, true, false);
            }
            else
                grid.AdicionarCabecalho("ContratoFrete", false);

            grid.AdicionarCabecalho("Distância", "KmRodado", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
            {
                grid.AdicionarCabecalho("Valor Contrato Franquia KM", "ValorKMContrato", TamanhoColunasValores, Models.Grid.Align.right, false, false);
                grid.AdicionarCabecalho("Km Consumido", "KmConsumido", TamanhoColunasValores, Models.Grid.Align.right, false, false);
                grid.AdicionarCabecalho("Valor Frete Franquia KM", "ValorFreteFranquiaKM", TamanhoColunasValores, Models.Grid.Align.right, false, false);
                grid.AdicionarCabecalho("Valor Franquia KM Excedido", "ValorKMExcedenteContrato", TamanhoColunasValores, Models.Grid.Align.right, false, false);
                grid.AdicionarCabecalho("Km Consumido Excedente", "KmConsumidoExcedente", TamanhoColunasValores, Models.Grid.Align.right, false, false);
                grid.AdicionarCabecalho("Valor Frete Franquia KM Excedido", "ValorFreteFranquiaKMExcedido", TamanhoColunasValores, Models.Grid.Align.right, false, false);
            }
            else
            {
                grid.AdicionarCabecalho("ValorKMContrato", false);
                grid.AdicionarCabecalho("ValorKMExcedenteContrato", false);
                grid.AdicionarCabecalho("KmConsumido", false);
                grid.AdicionarCabecalho("ValorFreteFranquiaKM", false);
                grid.AdicionarCabecalho("KmConsumidoExcedente", false);
                grid.AdicionarCabecalho("ValorFreteFranquiaKMExcedido", false);
            }

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
            {
                grid.AdicionarCabecalho("Pago", "Pago", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, true, true);
            }
            else
            {
                grid.AdicionarCabecalho("Pago", false);
            }

            grid.AdicionarCabecalho("Tipo de Carga", "TipoDeCarga", TamanhoColunasParticipantes, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Tipo de Operação", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Grupo Pessoa", "GrupoTomador", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Rotas", "Rotas", TamanhoColunasParticipantes, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Ocorrência", "Ocorrencia", TamanhoColunasValores, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Tipo Ocorrência", "TipoOcorrencia", TamanhoColunasValores, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Retorno Sefaz", "RetornoSefaz", TamanhoColunasParticipantes, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Data Ocorrência Final", "DataOcorrenciaFinal", TamanhoColunasValores, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Veículo do Último MDF-e", "VeiculoUltimoMDFe", TamanhoColunasValores, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Nº do Último MDF-e", "NumeroUltimoMDFe", TamanhoColunasValores, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Última Ocorrência", "DescricaoUltimaOcorrencia", TamanhoColunasParticipantes, Models.Grid.Align.left, false, false);

            if (await repTipoIntegracao.ExistePorTipoAsync(new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>() { Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Natura }))
                grid.AdicionarCabecalho("DT Natura", "NumeroDTNatura", TamanhoColunasValores, Models.Grid.Align.left, false, false);
            else
                grid.AdicionarCabecalho("NumeroDTNatura", false);

            grid.AdicionarCabecalho("Nº OCA Doc. Orig.", "NumeroOCADocumentoOriginario", TamanhoColunasValores, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Operador", "Operador", TamanhoColunasLocalidades, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Data Coleta", "DataColeta", TamanhoColunasLocalidades, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Modelo Veicular", "ModeloVeicular", TamanhoColunasLocalidades, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Log", "Log", TamanhoColunasLocalidades, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Previsão de Entrega", "DataPrevistaEntrega", TamanhoColunasLocalidades, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Nº DI", "NumeroDI", TamanhoColunasValores, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Nº DTA", "NumeroDTA", TamanhoColunasValores, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Cód. Referência", "CodigoReferencia", TamanhoColunasValores, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Cód. Importação", "CodigoImportacao", TamanhoColunasValores, Models.Grid.Align.left, false, false);

            grid.AdicionarCabecalho("Nº Pedido Nota Fiscal", "NumeroPedidoNotaFiscal", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Nº Vale Pedágio", "NumeroValePedagio", TamanhoColunasLocalidades, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Valor Vale Pedágio", "ValorValePedagio", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Tabela Frete", "TabelaFrete", TamanhoColunasLocalidades, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Código Tabela Frete Cliente", "CodigoTabelaFreteCliente", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Tabela Frete Cliente", "TabelaFreteCliente", TamanhoColunasLocalidades, Models.Grid.Align.left, true, false, false, false, false);

            grid.AdicionarCabecalho("Nº Escrituração", "NumeroEscrituracao", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
            {
                grid.AdicionarCabecalho("Nº Pagamento", "NumeroPagamento", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
                grid.AdicionarCabecalho("Situação Pagamento", "SituacaoPagamentoFormatada", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
                grid.AdicionarCabecalho("Data Aprovação Pagamento", "DataAprovacaoPagamentoFormatada", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            }
            else if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                grid.AdicionarCabecalho("Nº Contabilização", "NumeroContabilizacao", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Nº Escrituração Cancelado", "NumeroLoteCancelamento", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Motivo Cancelamento", "MotivoCancelamento", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, false);

            if (ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal)
            {
                grid.AdicionarCabecalho("Nº Booking", "NumeroBooking", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
                grid.AdicionarCabecalho("Nº OS", "NumeroOS", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
                grid.AdicionarCabecalho("Nº Controle", "NumeroControle", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
                grid.AdicionarCabecalho("Tipo Proposta", "TipoProposta", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
                grid.AdicionarCabecalho("Número Proposta", "NumeroProposta", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
                grid.AdicionarCabecalho("Tipo Carga", "TipoCarga", TamanhoColunasLocalidades, Models.Grid.Align.left, true, false, false, false, false);
                grid.AdicionarCabecalho("Qtd. NF", "QuantidadeNF", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
                grid.AdicionarCabecalho("Viagem", "Viagem", TamanhoColunasLocalidades, Models.Grid.Align.left, true, false, false, false, false);
                grid.AdicionarCabecalho("Nº Lacre", "NumeroLacre", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
                grid.AdicionarCabecalho("Tara", "Tara", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
                grid.AdicionarCabecalho("Container", "Container", TamanhoColunasLocalidades, Models.Grid.Align.left, true, false, false, false, false);
                grid.AdicionarCabecalho("Tipo Container", "TipoContainer", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
                grid.AdicionarCabecalho("Nº Fatura", "NumeroFatura", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
                grid.AdicionarCabecalho("Data Fatura", "DataFatura", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
                grid.AdicionarCabecalho("Nº Boleto", "NumeroBoleto", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
                grid.AdicionarCabecalho("Data Boleto", "DataBoleto", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
                grid.AdicionarCabecalho("Porto Origem", "PortoOrigem", TamanhoColunasLocalidades, Models.Grid.Align.left, true, false, false, false, false);
                grid.AdicionarCabecalho("Porto Destino", "PortoDestino", TamanhoColunasLocalidades, Models.Grid.Align.left, true, false, false, false, false);
                grid.AdicionarCabecalho("Porto Transbordo", "PortoTransbordo", TamanhoColunasLocalidades, Models.Grid.Align.left, true, false, false, false, false);
                grid.AdicionarCabecalho("Navio Transbordo", "NavioTransbordo", TamanhoColunasLocalidades, Models.Grid.Align.left, true, false, false, false, false);
                grid.AdicionarCabecalho("Possui CC-e", "PossuiCartaCorrecao", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
                grid.AdicionarCabecalho("Foi Anulado", "FoiAnulado", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
                grid.AdicionarCabecalho("Possui CT-e Comp.", "PossuiCTeComplementar", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
                grid.AdicionarCabecalho("Foi Substituído", "FoiSubstituido", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
                grid.AdicionarCabecalho("Previsão Saída Navio", "ETS", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
                grid.AdicionarCabecalho("Previsão Chegada Navio", "ETA", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
                grid.AdicionarCabecalho("Tipo Serviço Multimodal", "TipoServicoMultimodal", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
                grid.AdicionarCabecalho("Número Manifesto", "NumeroManifesto", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
                grid.AdicionarCabecalho("Número CE Mercante", "NumeroCEMercante", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
                grid.AdicionarCabecalho("Afretamento", "DescricaoAfretamento", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
                grid.AdicionarCabecalho("Número Protocolo ANTAQ", "NumeroProtocoloANTAQ", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
                grid.AdicionarCabecalho("Número CE FEEDER", "NumeroCEANTAQ", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
                grid.AdicionarCabecalho("Número Manifesto FEEDER", "NumeroManifestoFeeder", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
                grid.AdicionarCabecalho("Qtd. Container", "QuantidadeContainer", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
                grid.AdicionarCabecalho("PTAX", "Taxa", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, TipoSumarizacao.sum);
                grid.AdicionarCabecalho("Justificativa Mercante", "JustificativaNaoEnviarParaMercante", TamanhoColunasLocalidades, Models.Grid.Align.left, true, false, false, false, false);
                grid.AdicionarCabecalho("Anexos", "Anexos", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
                grid.AdicionarCabecalho("Descrição Anexos", "DescricaoAnexos", TamanhoColunasLocalidades, Models.Grid.Align.left, false, false, false, false, false);
                grid.AdicionarCabecalho("Just. Motivo Mercante", "JustificativaMotivoMercante", TamanhoColunasParticipantes, Models.Grid.Align.left, false, false, false, false, false);
            }

            grid.AdicionarCabecalho("Alíquota ICMS Interna", "AliquotaICMSInterna", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("% ICMS Partilha", "PercentualICMSPartilha", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Valor ICMS UF Origem", "ValorICMSUFOrigem", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Valor ICMS UF Destino", "ValorICMSUFDestino", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Valor ICMS FCP Fim", "ValorICMSFCPFim", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Característica Transporte", "CaracteristicaTransporteCTe", TamanhoColunasLocalidades, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Produto Predominante", "ProdutoPredominante", TamanhoColunasLocalidades, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Centro Resultado", "CentroResultado", TamanhoColunasLocalidades, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Proc. Importação", "ProcImportacao", TamanhoColunasLocalidades, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("CPF Motorista", "CpfMotorista", TamanhoColunasLocalidades, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Rota de Frete", "RotaFrete", TamanhoColunasLocalidades, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor total sem Tributo", "ValorSemTributo", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, TipoSumarizacao.sum);

            grid.AdicionarCabecalho("Nº CT-e Substituto", "NumeroCTeSubstituto", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Nº Controle CT-e Substituto", "NumeroControleCTeSubstituto", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Nº CT-e Anulação", "NumeroCTeAnulacao", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Nº Controle CT-e Anulação", "NumeroControleCTeAnulacao", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Nº CT-e Complementar", "NumeroCTeComplementar", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Nº Controle CT-e Complementar", "NumeroControleCTeComplementar", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Nº CT-e Manual Duplicado", "NumeroCTeDuplicado", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Nº Controle CT-e Manual Duplicado", "NumeroControleCTeDuplicado", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Nº CT-e Original", "NumeroCTeOriginal", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Nº Controle CT-e Original", "NumeroControleCTeOriginal", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Nº CIOT", "NumeroCIOT", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Nº Doc. Originário", "NumeroDocumentoOriginario", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Data Pagamento", "DataPagamentoFormatada", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Data Início Viagem", "DataInicioViagem", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Data Fim Viagem", "DataFimViagem", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor Frete Terceiro", "ValorFreteTerceiro", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Nº CT-e Terceiro Ocorrência", "NumeroCTeTerceiroOcorrencia", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Nº Doc. Recebedor Ocorrência", "NumeroDocumentoRecebedor", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Nº Pedido Cliente", "NumeroPedidoCliente", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Qtd. Total Produto", "QuantidadeTotalProduto", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Distância Carga Agrupada", "DistanciaCargaAgrupada", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Modelo Veículo", "ModeloVeiculo", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Modelo Veicular de Carga", "ModeloVeiculoCarga", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Tipo Carroceria", "TipoCarroceria", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Cta. Contábil", "ContaContabil", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Operador Resp. Cancelamento", "OperadorResponsavelCancelamento", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Veículo Tração", "VeiculoTracao", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Chassi tração", "ChassiTracao", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Veículo Reboque", "VeiculoReboque", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("KM da Rota", "KMRota", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Confirmação Documentos", "DataConfirmacaoDocumento", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Usuário que Solicitou", "UsuarioSolicitante", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Lacre(s)", "LacresCargaLacre", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Pallets (Pedido)", "PalletsPedido", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Lei da Regra de ICMS", "RegraICMS", TamanhoColunasLocalidades, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Regra de ICMS", "DescricaoRegraICMS", TamanhoColunasLocalidades, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("CPF/CNPJ Terceiro", "CPFCNPJTerceiro", TamanhoColunasLocalidades, Models.Grid.Align.left, false, false, false, false, false).Ocultar(TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS);
            grid.AdicionarCabecalho("Nome Terceiro", "NomeTerceiro", TamanhoColunasParticipantes, Models.Grid.Align.left, false, false, false, false, false).Ocultar(TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS);
            grid.AdicionarCabecalho("Nº Container", "NumeroContainer", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false).Ocultar(ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal);
            grid.AdicionarCabecalho("Nº EXP", "NumeroEXP", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false).Ocultar(TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);
            grid.AdicionarCabecalho("Situação Canhoto", "SituacaoCanhotoFormatada", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Data Ocorrência", "DataOcorrenciaFormatada", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Data Vigência Tabela de Frete", "DataVigenciaTabelaFrete", TamanhoColunasValores, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Tipo Transportador", "TipoProprietario", TamanhoColunasLocalidades, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Tipo Modal", "DescricaoTipoModal", TamanhoColunasLocalidades, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Número do Contrato De Frete", "NumeroContratoFreteTerceiro", TamanhoColunasLocalidades, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Situação da Fatura", "SituacaoFaturaDescricao", TamanhoColunasLocalidades, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Número Doc. Original", "NumeroDocumentoOriginal", TamanhoColunasLocalidades, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Chave CT-e Original", "ChaveCTeOriginal", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor a Receber CT-e Original", "ValorReceberCTeOriginal", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Terceiro", "Terceiro", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Redespacho", "Redespacho", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Carga Data Carregamento", "CargaDataCarregamentoFormatada", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Funcionário Resp. Veic.", "FuncionarioResponsavel", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Exceção Cab", "DescricaoExcecaoCab", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Número CT-e Anterior", "NumeroCTEAnterior", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Chave CT-e Anterior", "ChaveCTEAnterior", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Escrituração", "ExisteEscrituracao", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Código Escrituração", "CodigoEscrituracao", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Nº Carregamento", "NumeroCarregamento", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Vendedor", "Vendedor", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Número Completo", "NumeroCompleto", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Etapa", "Etapa", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Ordem", "Ordem", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Numero Miro", "NumeroMiro", TamanhoColunasValores, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Numero Estorno", "NumeroEstorno", TamanhoColunasValores, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Bloqueio", "Bloqueio", TamanhoColunasValores, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Data Miro", "DataMiro", TamanhoColunasValores, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho("Vencimento", "Vencimento", TamanhoColunasValores, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho("Termo Pagamento", "TermoPagamento", TamanhoColunasValores, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("B.C. CTe Substituído", "BCCTeSubstituido", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Frete Informado Manualmente", "FreteInformadoManualmente", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Peso Pedido (kg)", "PesoPedido", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Data da Operação do Navio (POD)", "DataOperacaoNavioFormatada", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Booking Reference FEEDER", "BookingReferente", TamanhoColunasParticipantes, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Codigo Documentação Navio", "CodigoDocumentacaoNavio", TamanhoColunasParticipantes, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Tipo OS Convertido", "TipoOSConvertidoDescricao", TamanhoColunasParticipantes, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Tipo OS", "TipoOSDescricao", TamanhoColunasParticipantes, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Provedor OS", "ProvedorOS", TamanhoColunasParticipantes, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Número CRT", "NumeroCRT", TamanhoColunasParticipantes, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Mic/DTA", "MicDTA", TamanhoColunasParticipantes, Models.Grid.Align.left, false, false, false, false, true);


            Dominio.Entidades.Embarcador.Pedidos.Stage existeStages = await repositorioStage.BuscarPrimeiroRegistroAsync();
            if (existeStages != null)
            {
                grid.AdicionarCabecalho("Numero Folha", "NumeroFolha", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
                grid.AdicionarCabecalho("Data Folha", "DataFolhaFormatada", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
                grid.AdicionarCabecalho("Folha Calculada", "FolhaCalculada", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
                grid.AdicionarCabecalho("Folha atribuída", "FolhaAtribuida", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
                grid.AdicionarCabecalho("Folha transferida", "FolhaTransferida", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
                grid.AdicionarCabecalho("Folha cancelada", "FolhaCanceladaFormatada", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
                grid.AdicionarCabecalho("Folha inconsistente", "FolhaInconsistenteFormatada", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
                grid.AdicionarCabecalho("Inconsistência folha", "InconsistenciaFolha", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            }

            //verificar se possui integracao com gnre
            if (new Repositorio.Embarcador.Configuracoes.IntegracaoGNRE(unidadeDeTrabalho)?.BuscarPrimeiroRegistro()?.PossuiIntegracaoGNRE ?? false)
                grid.AdicionarCabecalho("ICMS GNRE", "ICMSGNRE", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);

            if (new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unidadeDeTrabalho)?.BuscarPrimeiroRegistro()?.PermiteInformarPedidoDeSubstituicao ?? false)
                grid.AdicionarCabecalho("Substituição", "SubstituicaoDescricao", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);

            //Colunas montadas dinamicamente
            for (int i = 0; i < componentes.Count; i++)
            {
                if (i < NumeroMaximoComplementos)
                {
                    if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                        grid.AdicionarCabecalho(componentes[i].Descricao, "ValorComponente" + UltimaColunaDinanica.ToString(), TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, TipoSumarizacao.sum, componentes[i].Codigo);
                    else
                    {
                        bool exibirComponenteTransportador = !await repOcorrencia.ExisteBloqueioTransportadorPorComponenteAsync(componentes[i].Codigo);

                        if (exibirComponenteTransportador)
                            grid.AdicionarCabecalho(componentes[i].Descricao, "ValorComponente" + UltimaColunaDinanica.ToString(), TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, TipoSumarizacao.sum, componentes[i].Codigo);
                    }

                    UltimaColunaDinanica++;
                }
                else
                    break;
            }

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
            {
                for (int i = 0; i < layoutes.Count; i++)
                {
                    if (i < NumeroMaximoLayout)
                        grid.AdicionarCabecalho(layoutes[i].Descricao, "LayoutArquivo" + (i + 1).ToString(), TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, TipoSumarizacao.nenhum, layoutes[i].Codigo);
                    else
                        break;
                }
            }

            grid.AdicionarCabecalho("Nº Vale Pedágio Manual", "NumeroValePedagioManual", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor Vale Pedágio Manual", "ValorValePedagioManual", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Série da NFe", "SerieNota", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Doc.Fatura SAP", "DocFaturaSapFormatada", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Cód. Centro Custo", "CodigoCentroDeCustoEmissor", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.CentroDeCustoViagem, "CentroDeCustoViagemDescricao", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);

            return grid;
        }

        protected override Task<FiltroPesquisaCteRelatorio> ObterFiltrosPesquisaAsync(UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
