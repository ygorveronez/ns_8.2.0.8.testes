using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Financeiros
{
    [Area("Relatorios")]
    [CustomAuthorize("Relatorios/Financeiros/Titulo")]
    public class TituloController : BaseController
    {
        #region Construtores

        public TituloController(Conexao conexao) : base(conexao) { }

        #endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R030_Titulo;

        #region Métodos Públicos

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                await unitOfWork.StartAsync(cancellationToken);
                int Codigo = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Títulos", "Financeiros", "Titulo.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "Codigo", "asc", "", "", Codigo, unitOfWork, false, false);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                var retorno = gridRelatorio.RetornoGridPadraoRelatorio(GridPadrao(), relatorio);
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

                Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioTitulo filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.Financeiros.Titulo servicoRelatorioTitulo = new Servicos.Embarcador.Relatorios.Financeiros.Titulo(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioTitulo.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.Titulo> listaTitulo, out int totalRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

                grid.AdicionaRows(listaTitulo);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
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

        public async Task<IActionResult> GerarRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                string stringConexao = _conexao.StringConexao;

                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = serRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioTitulo filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

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
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o relatório.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid GridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();
            grid.header = new List<Models.Grid.Head>();
            grid.AdicionarCabecalho("Nº Título", "Codigo", 8, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("Fatura", "Fatura", 8, Models.Grid.Align.right, true, false, false, true, false);
            grid.AdicionarCabecalho("Borderô", "Bordero", 8, Models.Grid.Align.right, true, false, false, true, false);
            grid.AdicionarCabecalho("Filial", "Filial", 5, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("CPF/CNPJ", "CPFCNPJPessoaFormatado", 10, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Nome Pessoa", "NomePessoa", 15, Models.Grid.Align.left, true, false, false, true, true);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
            {
                grid.AdicionarCabecalho("CNPJ Transportador", "CNPJEmpresaFormatado", 10, Models.Grid.Align.center, false, false, false, true, false);
                grid.AdicionarCabecalho("Transportador", "Empresa", 15, Models.Grid.Align.left, false, false, false, true, false);
            }
            else
            {
                grid.AdicionarCabecalho("CNPJ Empresa/Filial", "CNPJEmpresaFormatado", 10, Models.Grid.Align.center, false, false, false, true, false);
                grid.AdicionarCabecalho("Empresa/Filial", "Empresa", 15, Models.Grid.Align.left, false, false, false, true, false);
            }

            grid.AdicionarCabecalho("Grupo de Pessoas", "GrupoPessoa", 12, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Tipo", "TipoTitulo", 8, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Status", "StatusTitulo", 8, Models.Grid.Align.center, true, true);
            grid.AdicionarCabecalho("Renegociado", "Renegociado", 5, Models.Grid.Align.left, false);

            grid.AdicionarCabecalho("Emissão", "DataEmissaoFormatada", 12, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Data Base", "DataBaseLiquidacao", 12, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho("Vencimento", "DataVencimentoFormatada", 12, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Programação", "DataProgramacaoPagamento", 12, Models.Grid.Align.center, true, false, false, true, false);

            grid.AdicionarCabecalho("Autorização", "DataAutorizacao", 12, Models.Grid.Align.center, true, false, false, true, false);
            grid.AdicionarCabecalho("Data Pagamento", "DataPagamento", 12, Models.Grid.Align.center, true, true);
            grid.AdicionarCabecalho("Data Doc. Entrada", "DataLancamentoNota", 8, Models.Grid.Align.center, true, false, false, true, false);
            grid.AdicionarCabecalho("Data Doc. Saída", "DataEmissaoDocumentos", 8, Models.Grid.Align.center, true, false, false, false, false);

            grid.AdicionarCabecalho("Tipo Doc", "TipoDocumento", 8, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Nº Doc.", "NumeroDocumento", 8, Models.Grid.Align.right, true, false, false, true, true);
            grid.AdicionarCabecalho("Parcela", "Parcela", 5, Models.Grid.Align.center, true, true);
            grid.AdicionarCabecalho("Observação", "Observacao", 20, Models.Grid.Align.left, false, true);

            grid.AdicionarCabecalho("Valor Pendente", "ValorPendente", 8, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("Valor Título", "ValorTitulo", 8, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("Acréscimo", "ValorAcrescimo", 8, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("Acréscimo Geração", "AcrescimoGeracao", 8, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Acréscimo Baixa", "AcrescimoBaixa", 8, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Desconto", "ValorDesonto", 8, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("Desconto Geração", "DescontoGeracao", 8, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Desconto Baixa", "DescontoBaixa", 8, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Valor Saldo", "ValorSaldo", 8, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("Valor Pago", "ValorPago", 8, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("Observação Fatura", "ObservacaoFatura", 20, Models.Grid.Align.left, false, false);

            grid.AdicionarCabecalho("Modelo Doc.", "ModeloDocumento", 8, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Tipo Doc. Original", "TipoDocumentoTituloOriginal", 8, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Nº Doc. Original", "NumeroDocumentoTituloOriginal", 10, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Data Cancelamento", "DataCancelamento", 12, Models.Grid.Align.center, true, false);
            grid.AdicionarCabecalho("Nº Boleto", "NumeroBoleto", 12, Models.Grid.Align.left, false, false);

            grid.AdicionarCabecalho("CT-es", "Conhecimentos", 30, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Series CT-es", "SeriesConhecimentos", 10, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Cargas", "Cargas", 20, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Emitente CT-es", "EmpresaConhecimentos", 30, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Banco", "BancoCliente", 10, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Tipo Conta", "TipoContaCliente", 10, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Agência", "AgenciaCliente", 10, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Dígito Agência", "DigitoAgenciaCliente", 10, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Número Conta", "NumeroContaCliente", 10, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Adiantado", "Adiantado", 6, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho("Portador", "Portador", 10, Models.Grid.Align.left, false, false);

            grid.AdicionarCabecalho("Nat. Operação", "NaturezaOperacao", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Forma", "DescricaoFormaTitulo", 10, Models.Grid.Align.left, false, false);

            grid.AdicionarCabecalho(descricao: "Tipo Operação", propriedade: "TipoOperacao", tamanho: 20, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false, visible: false);
            grid.AdicionarCabecalho("Provisão", "Provisao", 8, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Tip. Pag/Receb", "TipoPagamentoRecebimento", 15, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Tip. Movimento", "TipoMovimento", 15, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Remessa", "Remessa", 5, Models.Grid.Align.right, true, false, false, true, false);
            grid.AdicionarCabecalho("Cód. Remessa", "CodigoRemessa", 5, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Arquivo Remessa", "ArquivoRemessa", 8, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Nº Cheques", "NumerosCheques", 8, Models.Grid.Align.left, true, false, false, true, false);

            grid.AdicionarCabecalho("Valor Moeda Estrangeira", "ValorMoedaCotacao", 8, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Valor Total Moeda Estrangeira", "ValorOriginalMoedaEstrangeira", 8, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Data Base CRT", "DataBaseCRT", 12, Models.Grid.Align.center, true, false);
            grid.AdicionarCabecalho("Moeda Estrangeira", "MoedaCotacaoBancoCentral", 15, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Categoria", "Categoria", 15, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Obs. Interna", "ObservacaoInterna", 15, Models.Grid.Align.left, false, false, false, false, false);

            grid.AdicionarCabecalho("Ordem Compra", "OrdemCompra", 15, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Ordem Serviço", "OrdemServico", 15, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Variação de Saldo", "VariacaoCambial", 8, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Tipo Chave Pix", "TipoChavePixFormatada", 8, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Chave Pix", "ChavePix", 8, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Cta Contábil", "ContaFornecedorEBS", 8, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Usuário", "Usuario", 8, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Data Lançamento", "DataLancamento", 8, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Portador Conta", "PortadorConta", 8, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor Capital Contrato Financiamento", "ValorCapitalContratoFinanciamento", 8, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor Acrescimo Contrato Financiamento", "ValorAcrescimoContratoFinanciamento", 8, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Acréscimo Calculado", "AcrescimoCalculado", 8, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Acréscimo na Baixa", "AcrescimoNaBaixa", 8, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Acréscimo no acréscimo e desconto do contrato", "AcrescimoLancamentoTitulo", 8, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Desconto no acréscimo e desconto do contrato", "DescontoLancamentoTitulo", 8, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Veículo", "Veiculo", 8, Models.Grid.Align.left, false, false, false, false, false);

            grid.AdicionarCabecalho("Tipo Proposta", "TipoProposta", 8, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Nº Proposta", "NumeroProposta", 8, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Nº Controle CT-e", "NumeroControleCTe", 8, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Nº Booking CT-e", "NumeroBookingCTe", 8, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Navio/Viagem/Direção", "NavioViagemDirecao", 8, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Aprovador Ordem de Compra", "AprovadorOrdemCompra", 8, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Tipo de contato", "TipoContato", 8, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Situação do contato", "SituacaoContato", 8, Models.Grid.Align.left, false, false, false, true, false);
            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioTitulo ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioTitulo()
            {
                Tipo = Request.GetNullableEnumParam<TipoTitulo>("TipoTitulo"),
                Status = Request.GetListEnumParam<StatusTitulo>("StatusTitulo"),
                Renegociado = Request.GetEnumParam<TituloRenegociado>("TituloRenegociado"),
                TipoDocumento = Request.GetNullableEnumParam<TipoDocumentoPesquisaTitulo>("TipoDocumento"),
                CnpjPessoas = Request.GetListParam<double>("Pessoa"),
                CnpjPortador = Request.GetDoubleParam("Portador"),
                CodigosTipoMovimento = Request.GetListParam<int>("TipoMovimento"),
                Adiantado = Request.GetIntParam("Adiantado"),
                CodigoCTe = Request.GetIntParam("ConhecimentoDeTransporte"),
                CodigoTitulo = Request.GetIntParam("Titulo"),
                CodigoDocumentoEntrada = Request.GetIntParam("DocumentoEntrada"),
                CodigoFatura = Request.GetIntParam("Fatura"),
                DocumentoOriginal = Request.GetStringParam("DocumentoOriginal"),
                NumeroDocumentoOriginario = Request.GetIntParam("NumeroDocumentoOriginario"),
                NumeroOcorrencia = Request.GetIntParam("NumeroOcorrencia"),
                CodigoBordero = Request.GetIntParam("Bordero"),
                TipoBoleto = Request.GetEnumParam<TipoBoletoPesquisaTitulo>("TipoBoleto"),
                CodigoTipoPagamentoRecebimento = Request.GetIntParam("TipoPagamentoRecebimento"),
                CodigoPagamentoEletronico = Request.GetIntParam("PagamentoEletronico"),
                GruposPessoas = Request.GetListParam<int>("GrupoPessoa"),
                DataInicialEmissao = Request.GetDateTimeParam("DataInicialEmissao"),
                DataFinalEmissao = Request.GetDateTimeParam("DataFinalEmissao"),
                DataInicialVencimento = Request.GetDateTimeParam("DataInicialVencimento"),
                DataFinalVencimento = Request.GetDateTimeParam("DataFinalVencimento"),
                DataInicialQuitacao = Request.GetDateTimeParam("DataInicialQuitacao"),
                DataFinalQuitacao = Request.GetDateTimeParam("DataFinalQuitacao"),
                DataInicialEmissaoDocumentoEntrada = Request.GetDateTimeParam("DataInicialEmissaoDocumentoEntrada"),
                DataFinalEmissaoDocumentoEntrada = Request.GetDateTimeParam("DataFinalEmissaoDocumentoEntrada"),
                DataInicialCancelamento = Request.GetDateTimeParam("DataInicialCancelamento"),
                DataFinalCancelamento = Request.GetDateTimeParam("DataFinalCancelamento"),
                DataBaseInicial = Request.GetDateTimeParam("DataBaseInicial"),
                DataBaseFinal = Request.GetDateTimeParam("DataBaseFinal"),
                DataPosicaoFinal = Request.GetDateTimeParam("DataPosicaoFinal"),
                ValorInicial = Request.GetDecimalParam("ValorInicial"),
                ValorFinal = Request.GetDecimalParam("ValorFinal"),
                NovoModeloFatura = Request.GetNullableBoolParam("ModeloFatura"),
                NumeroPedidoCliente = Request.GetStringParam("NumeroPedidoCliente"),
                NumeroOcorrenciaCliente = Request.GetStringParam("NumeroOcorrenciaCliente"),
                FormaTitulo = Request.GetListEnumParam<FormaTitulo>("FormaTitulo"),
                ProvisaoPesquisaTitulo = Request.GetEnumParam("Provisao", ProvisaoPesquisaTitulo.ComProvisao),
                CodigosEmpresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? new List<int> { Empresa.Codigo } : Request.GetListParam<int>("Empresa"),
                TipoAmbiente = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? Empresa.TipoAmbiente : Dominio.Enumeradores.TipoAmbiente.Nenhum,
                TipoServicoMultisoftware = TipoServicoMultisoftware,
                CodigoTipoOperacao = Request.GetIntParam("TipoOperacao"),
                CodigoRemessa = Request.GetIntParam("Remessa"),
                CodigoCheque = Request.GetIntParam("Cheque"),
                DataInicialEntradaDocumentoEntrada = Request.GetDateTimeParam("DataInicialEntradaDocumentoEntrada"),
                DataFinalEntradaDocumentoEntrada = Request.GetDateTimeParam("DataFinalEntradaDocumentoEntrada"),
                DataAutorizacaoInicial = Request.GetDateTimeParam("DataAutorizacaoInicial"),
                DataAutorizacaoFinal = Request.GetDateTimeParam("DataAutorizacaoFinal"),
                DataProgramacaoPagamentoInicial = Request.GetDateTimeParam("DataProgramacaoPagamentoInicial"),
                DataProgramacaoPagamentoFinal = Request.GetDateTimeParam("DataProgramacaoPagamentoFinal"),
                CodigoCategoria = Request.GetIntParam("Categoria"),
                Moeda = Request.GetEnumParam<MoedaCotacaoBancoCentral>("Moeda"),
                Autorizados = Request.GetEnumParam<OpcaoSimNao>("Autorizados"),
                ModelosDocumento = Request.GetListParam<int>("ModeloDocumento"),
                CodigoPagamentoMotoristaTipo = Request.GetIntParam("PagamentoMotoristaTipo"),
                DataInicialLancamento = Request.GetDateTimeParam("DataInicialLancamento"),
                DataFinalLancamento = Request.GetDateTimeParam("DataFinalLancamento"),
                CodigoComandoBanco = Request.GetIntParam("ComandoBanco"),
                CodigoVeiculo = Request.GetIntParam("Veiculo"),
                VisualizarTitulosPagamentoSalario = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? Usuario.PermiteVisualizarTitulosPagamentoSalario : true,
                TipoProposta = Request.GetListEnumParam<TipoPropostaMultimodal>("TipoProposta"),
                TiposContato = Request.GetListParam<int>("TipoContato"),
                SituacoesContato = Request.GetListParam<int>("SituacaoContato"),
            };
        }

        #endregion
    }
}
