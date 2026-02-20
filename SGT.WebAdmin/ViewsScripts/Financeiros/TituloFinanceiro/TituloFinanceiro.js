/// <reference path="../../Enumeradores/EnumFormaTitulo.js" />
/// <reference path="../../Enumeradores/EnumTipoTitulo.js" />
/// <reference path="../../Enumeradores/EnumPermissaoPersonalizada.js" />
/// <reference path="../../Enumeradores/EnumSituacaoTitulo.js" />
/// <reference path="../../Enumeradores/EnumCodigoControleImportacao.js" />
/// <reference path="../../Enumeradores/EnumProvisaoPesquisaTitulo.js" />
/// <reference path="../../Enumeradores/EnumTipoPropostaMultimodal.js" />
/// <reference path="../../Enumeradores/EnumTipoBoletoPesquisaTitulo.js" />
/// <reference path="../../Enumeradores/EnumTipoDocumentoPesquisaTitulo.js" />
/// <reference path="../../Enumeradores/EnumTipoChavePix.js" />
/// <reference path="../RateioDespesaVeiculo/RateioDespesaVeiculo.js" />
/// <reference path="../../Consultas/TipoMovimento.js" />
/// <reference path="../../Consultas/TipoTerminalImportacao.js" />
/// <reference path="../../Consultas/PedidoViagemNavio.js" />
/// <reference path="CentroResultadoTipoDespesa.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridTituloFinanceiro, _tituloFinanceiro, _pesquisaTituloFinanceiro, _gridConhecimentosFatura, _gridDocumentos, _contatoClienteTitulo, _crudTituloFinanceiro, _PermissoesPersonalizadas;

var _adiantado = [
    { text: "Todos", value: -1 },
    { text: "Sim", value: 1 },
    { text: "Não", value: 0 }
];

var _provisaoPesquisaTitulo = [
    { text: "Com Provisão", value: EnumProvisaoPesquisaTitulo.ComProvisao },
    { text: "Sem Provisão", value: EnumProvisaoPesquisaTitulo.SemProvisao },
    { text: "Somente Provisão", value: EnumProvisaoPesquisaTitulo.SomenteProvisao }
];

var PesquisaTituloFinanceiro = function () {
    this.ValorMovimento = PropertyEntity({ text: "Valor Original: ", getType: typesKnockout.decimal });
    this.ValorPago = PropertyEntity({ text: "Valor Inicial pago: ", val: ko.observable("0,00"), def: ko.observable("0,00"), getType: typesKnockout.decimal });
    this.ValorPagoAte = PropertyEntity({ text: "Até: ", val: ko.observable("0,00"), def: ko.observable("0,00"), getType: typesKnockout.decimal });
    this.DataInicialVencimento = PropertyEntity({ text: "Data Vencimento de: ", getType: typesKnockout.date });
    this.DataFinalVencimento = PropertyEntity({ text: "Até: ", getType: typesKnockout.date });
    this.DataInicialEmissao = PropertyEntity({ text: "Data Emissão de: ", getType: typesKnockout.date });
    this.DataFinalEmissao = PropertyEntity({ text: "Até: ", getType: typesKnockout.date });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Empresa:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoMovimento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Movimento:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.GrupoPessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Pessoa:", idBtnSearch: guid() });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Pessoa:", idBtnSearch: guid() });
    this.Conhecimento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "CT-e:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Fatura = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Fatura:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Titulo = PropertyEntity({ text: "Cód. Titulo: ", getType: typesKnockout.int });
    this.StatusTitulo = PropertyEntity({ val: ko.observable([]), def: [], getType: typesKnockout.selectMultiple, text: "Situação:", options: EnumSituacaoTitulo.obterOpcoes() });
    this.TipoTitulo = PropertyEntity({ val: ko.observable(EnumTipoTitulo.Todos), options: EnumTipoTitulo.obterOpcoesPesquisa(), text: "Tipo: ", def: EnumTipoTitulo.Todos });
    this.NumeroPedido = PropertyEntity({ text: "Número Pedido: ", getType: typesKnockout.string, visible: ko.observable(true) });
    this.NumeroOcorrencia = PropertyEntity({ text: "Número Ocorrência: ", getType: typesKnockout.string, visible: ko.observable(true) });
    this.NumeroDocumentoOriginario = PropertyEntity({ text: "Nº Doc. Originário: ", getType: typesKnockout.int, visible: ko.observable(true), configInt: { precision: 0, allowZero: false, thousands: '' } });
    this.DataBaseLiquidacaoInicial = PropertyEntity({ text: "Data base quitação de: ", getType: typesKnockout.date });
    this.DataBaseLiquidacaoFinal = PropertyEntity({ text: "Até: ", getType: typesKnockout.date });
    this.DocumentoOriginal = PropertyEntity({ text: "Nº Doc. Original: " });
    this.NossoNumero = PropertyEntity({ text: "Nº Boleto:" });
    this.DataProgramacaoPagamentoInicial = PropertyEntity({ text: "Data Programação de: ", getType: typesKnockout.date });
    this.DataProgramacaoPagamentoFinal = PropertyEntity({ text: "Até: ", getType: typesKnockout.date });
    this.RaizCnpjPessoa = PropertyEntity({ text: "Raiz Cnpj Pessoa:" });

    this.Portador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Portador:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Adiantado = PropertyEntity({ val: ko.observable(-1), options: _adiantado, text: "Adiantado: ", def: -1, required: false, visible: ko.observable(false) });
    this.FormaTitulo = PropertyEntity({ val: ko.observable(EnumFormaTitulo.Todos), options: EnumFormaTitulo.obterOpcoesPesquisa(), text: "Forma do Título: ", def: EnumFormaTitulo.Todos, required: false });
    this.Provisao = PropertyEntity({ val: ko.observable(EnumProvisaoPesquisaTitulo.ComProvisao), options: _provisaoPesquisaTitulo, def: EnumProvisaoPesquisaTitulo.ComProvisao, text: "Provisão:", visible: ko.observable(true) });
    this.TipoBoleto = PropertyEntity({ val: ko.observable(EnumTipoBoletoPesquisaTitulo.Todos), options: EnumTipoBoletoPesquisaTitulo.obterOpcoesPesquisa(), def: EnumTipoBoletoPesquisaTitulo.Todos, text: "Tipo Boleto:", visible: ko.observable(true) });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Remessa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Remessa:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.MoedaCotacaoBancoCentral = PropertyEntity({ val: ko.observable(EnumMoedaCotacaoBancoCentral.Todos), options: EnumMoedaCotacaoBancoCentral.obterOpcoesPesquisa(), def: EnumMoedaCotacaoBancoCentral.Todos, text: "Moeda: ", visible: ko.observable(false), enable: ko.observable(true) });
    this.TipoDeDocumento = PropertyEntity({ val: ko.observable(EnumTipoDocumentoPesquisaTitulo.Todos), options: EnumTipoDocumentoPesquisaTitulo.obterOpcoesPesquisa(), def: EnumTipoDocumentoPesquisaTitulo.Todos, text: "Tipo de Documento:", visible: ko.observable(true) });

    //Emissão Multimodal
    this.NumeroBooking = PropertyEntity({ text: "Número Booking:" });
    this.NumeroOS = PropertyEntity({ text: "Número OS:" });
    this.NumeroCarga = PropertyEntity({ text: "Número Carga:" });
    this.NumeroNota = PropertyEntity({ text: "Número Nota:", getType: typesKnockout.int });
    this.NumeroControleCliente = PropertyEntity({ text: "Número Controle Cliente:" });
    this.NumeroControle = PropertyEntity({ text: "Número Controle:" });
    this.TipoProposta = PropertyEntity({ val: ko.observable(EnumTipoPropostaMultimodal.Todos), def: EnumTipoPropostaMultimodal.Todos, text: "Tipo Proposta:", options: EnumTipoPropostaMultimodal.obterOpcoesPesquisa() });
    this.TerminalOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Terminal Origem:", idBtnSearch: guid() });
    this.TerminalDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Terminal Destino:", idBtnSearch: guid() });
    this.Viagem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Viagem:", idBtnSearch: guid() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridTituloFinanceiro.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (e.BuscaAvancada.visibleFade()) {
                e.BuscaAvancada.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                e.BuscaAvancada.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var TituloFinanceiro = function () {
    this.CodigoRecebidoIntegracao = PropertyEntity({ text: "Código da Integração: ", val: ko.observable(0), def: 0, getType: typesKnockout.int, enable: ko.observable(false), required: false, visible: ko.observable(false) });
    this.Codigo = PropertyEntity({ text: "Código: ", val: ko.observable(0), def: 0, getType: typesKnockout.int, enable: ko.observable(false), required: false });
    this.Fatura = PropertyEntity({ text: "Fatura: ", val: ko.observable(""), def: "", getType: typesKnockout.int, enable: ko.observable(false), required: false, visible: ko.observable(true) });
    this.BoletoStatusTitulo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, enable: ko.observable(true) });
    this.DataEmissao = PropertyEntity({ text: "*Data Emissão: ", required: true, getType: typesKnockout.date, enable: ko.observable(true) });
    this.DataVencimento = PropertyEntity({ text: "*Data Vencimento: ", required: true, getType: typesKnockout.date, enable: ko.observable(true) });
    this.DataLiquidacao = PropertyEntity({ text: "Data Liquidação: ", required: false, getType: typesKnockout.date, enable: ko.observable(false), cssClass: ko.observable("col col-xs-2 col-sm-12 col-md-2 col-lg-2") });
    this.DataBaseLiquidacao = PropertyEntity({ text: "Data Base Liquidação: ", required: false, getType: typesKnockout.date, enable: ko.observable(false) });
    this.DataAutorizacao = PropertyEntity({ text: "Data Autorização: ", getType: typesKnockout.date, enable: ko.observable(false) });
    this.NumeroDocumento = PropertyEntity({ text: "Nº Documeto: ", required: false, enable: ko.observable(true), visible: ko.observable(false) });
    this.DataProgramacaoPagamento = PropertyEntity({ text: "Programação de Pagamento: ", required: false, getType: typesKnockout.date, enable: ko.observable(true) });

    this.StatusTitulo = PropertyEntity({ val: ko.observable(EnumSituacaoTitulo.EmAberto), options: EnumSituacaoTitulo.obterOpcoes(), text: "*Situação: ", def: EnumSituacaoTitulo.EmAberto, required: true, enable: ko.observable(false) });
    this.TipoTitulo = PropertyEntity({ val: ko.observable(""), options: EnumTipoTitulo.obterOpcoes(), text: "*Tipo: ", def: "", required: true, enable: ko.observable(true), eventChange: TipoTituloChange });
    this.FormaTitulo = PropertyEntity({ val: ko.observable(EnumFormaTitulo.Outros), options: EnumFormaTitulo.obterOpcoes(), text: "*Forma do Título: ", def: EnumFormaTitulo.Outros, required: false, enable: ko.observable(true), cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-2 col-lg-2"), eventChange: FormaTituloChange });

    this.TipoDocumentoTituloOriginal = PropertyEntity({ text: "Tipo de Documento:", required: false, maxlength: 300, enable: ko.observable(true), visible: ko.observable(true) });
    this.NumeroDocumentoTituloOriginal = PropertyEntity({ text: ko.observable("Número do Documento:"), required: ko.observable(false), maxlength: 300, enable: ko.observable(true), visible: ko.observable(true) });
    this.CodigoFavorecido = PropertyEntity({ text: "Código Favorecido:", required: ko.observable(false), maxlength: 15, enable: ko.observable(true), visible: ko.observable(false) });
    this.DescontoAplicadoNegociacao = PropertyEntity({ text: "Desconto aplicado na negociação:", visible: ko.observable(false) });

    this.Sequencia = PropertyEntity({ text: "Sequência: ", required: false, getType: typesKnockout.int, enable: ko.observable(true) });
    this.ValorOriginal = PropertyEntity({ text: "*Valor Original: ", required: true, getType: typesKnockout.decimal, enable: ko.observable(true) });
    this.ValorPendente = PropertyEntity({ text: "Valor Pendente: ", required: false, getType: typesKnockout.decimal, enable: ko.observable(false) });
    this.ValorPago = PropertyEntity({ text: "Valor Pago: ", required: false, getType: typesKnockout.decimal, enable: ko.observable(false) });
    this.Desconto = PropertyEntity({ text: "Desconto: ", required: false, getType: typesKnockout.decimal, enable: ko.observable(true) });
    this.Acrescimo = PropertyEntity({ text: "Acréscimo: ", required: false, getType: typesKnockout.decimal, enable: ko.observable(true) });
    this.ValorSaldo = PropertyEntity({ text: "Saldo: ", required: false, getType: typesKnockout.decimal, enable: ko.observable(false) });

    this.Portador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Portador:"), idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(false) });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Pessoa:", idBtnSearch: guid(), required: true, enable: ko.observable(true), cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-4 col-lg-4") });
    this.GrupoPessoas = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Pessoa:", idBtnSearch: guid(), required: false, enable: ko.observable(true), cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-4 col-lg-4") });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Empresa:"), idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });
    this.TipoMovimento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Tipo de Movimento:", idBtnSearch: guid(), required: ko.observable(true), enable: ko.observable(true), visible: ko.observable(true) });

    this.HabilitarLinhaDigitavelBoleto = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Habilitar a linha digitável do boleto", def: false, visible: ko.observable(true), enable: ko.observable(false) });
    this.LinhaDigitavelBoleto = PropertyEntity({ text: "Linha Digitável do Boleto: ", required: false, maxlength: 100, enable: ko.observable(false), visible: ko.observable(true), val: ko.observable("") });
    this.NossoNumero = PropertyEntity({ text: "Código de Barras / Número do Boleto:", required: false, maxlength: 44, enable: ko.observable(false), visible: ko.observable(true), val: ko.observable("") });
    this.CodigoBarrasParaLinhaDigitavel = PropertyEntity({
        eventClick: CodigoBarrasParaLinhaDigitavelClick, type: types.event, text: ko.observable("Código de Barras para Linha Digitável"), visible: ko.observable(true), enable: ko.observable(true), icon: "fal fa-cogs"
    });
    this.LinhaDigitavelParaCodigoBarras = PropertyEntity({
        eventClick: LinhaDigitavelParaCodigoBarrasClick, type: types.event, text: ko.observable("Linha Digitável para Código de Barras"), visible: ko.observable(true), enable: ko.observable(true), icon: "fal fa-cogs"
    });
    this.ValidarValorBruto = PropertyEntity({
        eventClick: ValidarValorBrutoClick, type: types.event, text: ko.observable("Validar Valor Bruto"), visible: ko.observable(true), enable: ko.observable(true), icon: "fal fa-check"
    });

    this.MoedaCotacaoBancoCentral = PropertyEntity({ val: ko.observable(EnumMoedaCotacaoBancoCentral.Real), options: EnumMoedaCotacaoBancoCentral.obterOpcoes(), def: EnumMoedaCotacaoBancoCentral.Real, text: "Moeda: ", visible: ko.observable(false), enable: ko.observable(true) });
    this.DataBaseCRT = PropertyEntity({ text: "Data Base CRT: ", required: false, getType: typesKnockout.dateTime, enable: ko.observable(true), visible: ko.observable(false) });
    this.ValorMoedaCotacao = PropertyEntity({ text: "Valor Moeda: ", required: false, getType: typesKnockout.decimal, enable: ko.observable(true), visible: ko.observable(false), configDecimal: { precision: 10, allowZero: false, allowNegative: false }, maxlength: 22 });
    this.ValorOriginalMoedaEstrangeira = PropertyEntity({ text: "Valor Original Moeda: ", required: false, getType: typesKnockout.decimal, enable: ko.observable(false), visible: ko.observable(false) });

    this.Observacao = PropertyEntity({ text: "Observação: ", required: false, maxlength: 300, enable: ko.observable(true) });
    this.ObservacaoInterna = PropertyEntity({ text: "Observação Interna: ", required: false, maxlength: 300, enable: ko.observable(true) });
    this.Historico = PropertyEntity({ text: "Histórico: ", required: false, maxlength: 5000, enable: ko.observable(false) });

    this.LiberadoPagamento = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Liberar o pagamento do título?", def: false, visible: ko.observable(false) });
    this.Adiantado = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Este título foi adiantado?", def: false, visible: ko.observable(false), eventChange: AdiantadoChange });
    this.Provisao = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Este título é uma Provisão?", def: false, enable: ko.observable(true), visible: ko.observable(true), eventChange: ProvisaoChange });
    this.AdiantamentoFornecedor = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Este título é referente a adiantamento fornecedor?", def: false, visible: ko.observable(false) });

    this.DocumentoFaturamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Documento para Faturar:", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(false), visible: ko.observable(false) });

    this.CentrosResultado = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.CentrosResultadoTiposDespesa = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.Veiculos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.TipoChavePixPessoa = PropertyEntity({ text: "*Tipo Chave: ", val: ko.observable(EnumTipoChavePix.Nenhum), options: EnumTipoChavePix.obterOpcoesComVazio(), def: EnumTipoChavePix.Nenhum, enable: ko.observable(false), visible: ko.observable(false), required: ko.observable(false) });
    this.ChavePixPessoa = PropertyEntity({ text: "*Chave PIX: ", enable: ko.observable(false), visible: ko.observable(false), required: ko.observable(false) });

    this.MoedaCotacaoBancoCentral.val.subscribe(function (novoValor) {
        CalcularMoedaEstrangeira();
    });

    this.DataBaseCRT.val.subscribe(function (novoValor) {
        CalcularMoedaEstrangeira();
    });

    this.ValorMoedaCotacao.val.subscribe(function (novoValor) {
        ConverterValor();
    });

    this.HabilitarLinhaDigitavelBoleto.val.subscribe(function (novoValor) {
        _tituloFinanceiro.LinhaDigitavelBoleto.enable(novoValor);
    });

    //Variáveis globais
    this.TituloDeInfracao = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false });

    //aba anexo
    this.ListaAnexos = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.ListaAnexosNovos = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.ListaAnexosExcluidos = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
};

var CRUDTituloFinanceiro = function () {
    this.Adicionar = PropertyEntity({ eventClick: salvarClick, type: types.event, text: "Adicionar", visible: ko.observable(true), enable: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: salvarClick, type: types.event, text: "Atualizar", visible: ko.observable(false), enable: ko.observable(true) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Cancelar", visible: ko.observable(false), enable: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Limpar", visible: ko.observable(false), enable: ko.observable(true) });
    this.MultiploTitulo = PropertyEntity({ eventClick: multiploTituloClick, type: types.event, text: "Múltiplos Títulos", visible: ko.observable(true), enable: ko.observable(true) });
    this.GerarAutorizacaoPagamento = PropertyEntity({ eventClick: gerarAutorizacaoPagamentoClick, type: types.event, text: "Autorização Pagamento", visible: ko.observable(false), enable: ko.observable(true) });
    this.LimparRemessaPagamento = PropertyEntity({ eventClick: LimparRemessaPagamentoClick, type: types.event, text: "Limpar Remessa Pagamento", visible: ko.observable(false), enable: ko.observable(true) });

    this.GerarBoleto = PropertyEntity({ type: types.event, text: "Gerar Boleto", visible: ko.observable(false), enable: ko.observable(true), idBtnSearch: guid() });
    this.VisualizarBoleto = PropertyEntity({ eventClick: VisualizarBoletoClick, type: types.event, text: "Visualizar Boleto", visible: ko.observable(false), enable: ko.observable(true) });

    this.LimparDadosBoleto = PropertyEntity({ eventClick: LimparDadosBoletoClick, type: types.event, text: "Limpar dados Boleto", visible: ko.observable(false), enable: ko.observable(true) });
    this.LimparDadosRemessa = PropertyEntity({ eventClick: LimparDadosRemessaClick, type: types.event, text: "Limpar dados Remessa", visible: ko.observable(false), enable: ko.observable(true) });

    this.Importar = PropertyEntity({
        type: types.local,
        text: "Importar",
        visible: ko.observable(true),
        accept: ".xls,.xlsx,.csv",
        cssClass: "btn-default",
        UrlImportacao: "TituloFinanceiro/Importar",
        UrlConfiguracao: "TituloFinanceiro/ConfiguracaoImportacao",
        CodigoControleImportacao: EnumCodigoControleImportacao.O026_TituloFinanceiro,
        ParametrosRequisicao: function () {
            return {
                Inserir: true,
                Atualizar: false
            };
        },
        CallbackImportacao: function () {
            _gridTituloFinanceiro.CarregarGrid();
        }
    });

    this.ImportarLiquidosFolha = PropertyEntity({
        type: types.local,
        text: "Importar Líquidos de Folha",
        visible: ko.observable(true),
        accept: ".xls,.xlsx,.csv",
        cssClass: "btn-default",
        UrlImportacao: "TituloFinanceiro/ImportarLiquidosFolha",
        UrlConfiguracao: "TituloFinanceiro/ConfiguracaoImportacaoLiquidosFolha",
        CodigoControleImportacao: EnumCodigoControleImportacao.O027_LiquidoFolha,
        ParametrosRequisicao: function () {
            return {
                Inserir: true,
                Atualizar: false
            };
        },
        CallbackImportacao: function () {
            _gridTituloFinanceiro.CarregarGrid();
        }
    });
};

//*******EVENTOS*******

function loadTituloFinanceiro() {
    _tituloFinanceiro = new TituloFinanceiro();
    KoBindings(_tituloFinanceiro, "knockoutCadastroTituloFinanceiro");

    _pesquisaTituloFinanceiro = new PesquisaTituloFinanceiro();
    KoBindings(_pesquisaTituloFinanceiro, "knockoutPesquisaTituloFinanceiro", false, _pesquisaTituloFinanceiro.Pesquisar.id);

    _crudTituloFinanceiro = new CRUDTituloFinanceiro();
    KoBindings(_crudTituloFinanceiro, "knockoutCRUDTituloFinanceiro");

    LoadImpostoTituloFinanceiro();
    LoadDocumentoTituloFinanceiro();

    HeaderAuditoria("Titulo", _tituloFinanceiro);

    new BuscarClientes(_tituloFinanceiro.Pessoa, RetornoPessoaTitulo);
    new BuscarClientes(_tituloFinanceiro.Portador);
    new BuscarClientes(_tituloFinanceiro.Contribuinte);
    new BuscarGruposPessoas(_tituloFinanceiro.GrupoPessoas, null, null, null, EnumTipoGrupoPessoas.Ambos);
    new BuscarTipoMovimento(_tituloFinanceiro.TipoMovimento, null, null, RetornoTipoMovimentoTitulo, null, EnumFinalidadeTipoMovimento.TituloFinanceiro);
    new BuscarCTesSemCarga(_tituloFinanceiro.ConhecimentoEletronico, RetornoConhecimentoEletronico);
    new BuscarDocumentosFaturamentoEmAberto(_tituloFinanceiro.DocumentoFaturamento, RetornoConsultaDocumentoFaturamento);
    new BuscarEmpresa(_tituloFinanceiro.Empresa, null, true);

    new BuscarTributoTipoImposto(_tituloFinanceiro.TributoTipoImposto, RetornoTributoTipoImposto);
    new BuscarTributoCodigoReceita(_tituloFinanceiro.TributoCodigoReceita, RetornoTributoCodigoReceita);
    new BuscarTributoVariacaoImposto(_tituloFinanceiro.TributoVariacaoImposto);
    new BuscarTributoTipoDocumento(_tituloFinanceiro.TributoTipoDocumento, RetornoTributoTipoDocumento);

    new BuscarClientes(_pesquisaTituloFinanceiro.Pessoa);
    new BuscarClientes(_pesquisaTituloFinanceiro.Portador);
    new BuscarGruposPessoas(_pesquisaTituloFinanceiro.GrupoPessoa, null, null, null, EnumTipoGrupoPessoas.Ambos);
    new BuscarConhecimentoNotaReferencia(_pesquisaTituloFinanceiro.Conhecimento, RetornoCTe);
    new BuscarFatura(_pesquisaTituloFinanceiro.Fatura, RetornoFatura);
    new BuscarEmpresa(_pesquisaTituloFinanceiro.Empresa, null, true);
    new BuscarTipoMovimento(_pesquisaTituloFinanceiro.TipoMovimento, null, null, null, null, EnumFinalidadeTipoMovimento.TituloFinanceiro);
    new BuscarVeiculos(_pesquisaTituloFinanceiro.Veiculo);
    new BuscarRemessa(_pesquisaTituloFinanceiro.Remessa);
    new BuscarTipoTerminalImportacao(_pesquisaTituloFinanceiro.TerminalOrigem);
    new BuscarTipoTerminalImportacao(_pesquisaTituloFinanceiro.TerminalDestino);
    new BuscarPedidoViagemNavio(_pesquisaTituloFinanceiro.Viagem);

    new BuscarBoletoConfiguracao(_crudTituloFinanceiro.GerarBoleto, GerarBoletoClick);

    _tituloFinanceiro.DataEmissao.val(Global.DataAtual());

    carregarDespesaVeiculo("conteudoDespesaVeiculo", buscarTituloFinanceiros);

    _tituloFinanceiro.Historico.enable(false);
    _tituloFinanceiro.ValorOriginalMoedaEstrangeira.enable(false);
    _tituloFinanceiro.ValorPendente.enable(false);
    HabilitarTipoMovimento(true);

    $("#" + _pesquisaTituloFinanceiro.RaizCnpjPessoa.id).mask("00.000.000", { selectOnFocus: true, clearIfNotMatch: true });

    if (_CONFIGURACAO_TMS.ModificarTimelineDeAcordoComTipoServicoDocumento)
        _tituloFinanceiro.CodigoRecebidoIntegracao.visible(true);

    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) {
        _tituloFinanceiro.MoedaCotacaoBancoCentral.visible(true);
        _tituloFinanceiro.DataBaseCRT.visible(true);
        _tituloFinanceiro.ValorMoedaCotacao.visible(true);
        _tituloFinanceiro.ValorOriginalMoedaEstrangeira.visible(true);
        _pesquisaTituloFinanceiro.MoedaCotacaoBancoCentral.visible(true);
    }

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe) {
        _tituloFinanceiro.TipoMovimento.visible(true);
        _tituloFinanceiro.TipoMovimento.required(false);
        _tituloFinanceiro.Fatura.visible(false);
        _pesquisaTituloFinanceiro.Veiculo.visible(false);
        _pesquisaTituloFinanceiro.Fatura.visible(false);
        _pesquisaTituloFinanceiro.NumeroPedido.visible(false);
        _tituloFinanceiro.LiberadoPagamento.visible(false);
        _tituloFinanceiro.ConhecimentosFatura.visible(false);
        _tituloFinanceiro.DocumentoFaturamento.visible(false);
        _tituloFinanceiro.Portador.visible(true);
        _tituloFinanceiro.Portador.required(false);
        _tituloFinanceiro.Portador.text("Portador");
        _tituloFinanceiro.Adiantado.visible(true);
        _pesquisaTituloFinanceiro.Portador.visible(true);
        _pesquisaTituloFinanceiro.Adiantado.visible(true);
        _tituloFinanceiro.NumeroDocumento.visible(false);
        _tituloFinanceiro.Empresa.visible(false);
        _pesquisaTituloFinanceiro.Empresa.visible(false);
        _pesquisaTituloFinanceiro.TipoMovimento.visible(false);
        _tituloFinanceiro.Pessoa.cssClass("col col-xs-12 col-sm-12 col-md-6 col-lg-6");
        _tituloFinanceiro.GrupoPessoas.cssClass("col col-xs-12 col-sm-12 col-md-6 col-lg-6");
        _tituloFinanceiro.DataLiquidacao.cssClass("col col-xs-2 col-sm-12 col-md-2 col-lg-2");
        _tituloFinanceiro.FormaTitulo.cssClass("col col-xs-2 col-sm-12 col-md-4 col-lg-4");
    }

    if (_CONFIGURACAO_TMS.ExigirEmpresaTituloFinanceiro) {
        _tituloFinanceiro.Empresa.text("*Empresa:");
        _tituloFinanceiro.Empresa.required(true);
    }

    if (_CONFIGURACAO_TMS.ExigirNumeroDocumentoTituloFinanceiro) {
        _tituloFinanceiro.NumeroDocumentoTituloOriginal.text("*Número do Documento:");
        _tituloFinanceiro.NumeroDocumentoTituloOriginal.required(true);
    }

    if (_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal)
        $("#liFiltrosEmissaoMultimodal").show();

    buscarConhecimentosFatura();
    loadDropZoneTitulo();
    CarregarGridDocumentosTitulo();
    LoadCentroResultado();
    LoadCentroResultadoTipoDespesa();
    LoadVeiculo();
    loadTituloFinanceiroIntegracoes();
    loadTituloFinanceiroAnexo();

    $("#liTabImposto").hide();

    if (!_CONFIGURACAO_TMS.UsuarioAdministrador && !VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Titulo_GerarMultiplosTitulos, _PermissoesPersonalizadas)) {
        _crudTituloFinanceiro.MultiploTitulo.visible(false);
    } else {
        _crudTituloFinanceiro.MultiploTitulo.visible(true);
    }

    _contatoClienteTitulo = new ContatoCliente("btnContatoCliente", _tituloFinanceiro.Codigo, EnumTipoDocumentoContatoCliente.Titulo);

    $('.nav-tabs a').click(function (e) {
        e.preventDefault();
        $('#tabsTituloFinanceiro .tab-content').each(function (i, tabContent) {
            $(tabContent).children().each(function (z, el) {
                $(el).removeClass('active');
            });
        });
        $(this).tab('show');
    });
}

function RetornoTipoMovimentoTitulo(data) {
    _tituloFinanceiro.TipoMovimento.codEntity(data.Codigo);
    _tituloFinanceiro.TipoMovimento.val(data.Descricao);

    PreencherCentroResultadoTipoDespesa(data);
}

function RetornoTributoTipoDocumento(data) {
    _tituloFinanceiro.TributoTipoDocumento.codEntity(data.Codigo);
    _tituloFinanceiro.TributoTipoDocumento.val(data.Descricao);

    if (data.CodigoTributoTipoImposto > 0) {
        _tituloFinanceiro.TributoTipoImposto.codEntity(data.CodigoTributoTipoImposto);
        _tituloFinanceiro.TributoTipoImposto.val(data.DescricaoTributoTipoImposto);
    }
    if (data.CodigoTributoCodigoReceita > 0) {
        _tituloFinanceiro.TributoCodigoReceita.codEntity(data.CodigoTributoCodigoReceita);
        _tituloFinanceiro.TributoCodigoReceita.val(data.DescricaoTributoCodigoReceita);
    }
    if (data.CodigoTributoVariacaoImposto > 0) {
        _tituloFinanceiro.TributoVariacaoImposto.codEntity(data.CodigoTributoVariacaoImposto);
        _tituloFinanceiro.TributoVariacaoImposto.val(data.DescricaoTributoVariacaoImposto);
    }
}

function RetornoTributoTipoImposto(data) {
    _tituloFinanceiro.TributoTipoImposto.codEntity(data.Codigo);
    _tituloFinanceiro.TributoTipoImposto.val(data.Descricao);

    if (data.CodigoTributoCodigoReceita > 0) {
        _tituloFinanceiro.TributoCodigoReceita.codEntity(data.CodigoTributoCodigoReceita);
        _tituloFinanceiro.TributoCodigoReceita.val(data.DescricaoTributoCodigoReceita);
    }
    if (data.CodigoTributoVariacaoImposto > 0) {
        _tituloFinanceiro.TributoVariacaoImposto.codEntity(data.CodigoTributoVariacaoImposto);
        _tituloFinanceiro.TributoVariacaoImposto.val(data.DescricaoTributoVariacaoImposto);
    }
}

function RetornoTributoCodigoReceita(data) {
    _tituloFinanceiro.TributoCodigoReceita.codEntity(data.Codigo);
    _tituloFinanceiro.TributoCodigoReceita.val(data.Descricao);

    if (data.CodigoTributoVariacaoImposto > 0) {
        _tituloFinanceiro.TributoVariacaoImposto.codEntity(data.CodigoTributoVariacaoImposto);
        _tituloFinanceiro.TributoVariacaoImposto.val(data.DescricaoTributoVariacaoImposto);
    }
}

function CalcularMoedaEstrangeira() {
    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira && _tituloFinanceiro.DataBaseCRT.val() != null && _tituloFinanceiro.DataBaseCRT.val() != undefined && _tituloFinanceiro.DataBaseCRT.val() != "") {
        executarReST("Cotacao/ConverterMoedaEstrangeira", { MoedaCotacaoBancoCentral: _tituloFinanceiro.MoedaCotacaoBancoCentral.val(), DataBaseCRT: _tituloFinanceiro.DataBaseCRT.val() }, function (r) {
            if (r.Success) {
                if (r.Data != null && r.Data != undefined && r.Data > 0)
                    _tituloFinanceiro.ValorMoedaCotacao.val(Globalize.format(r.Data, "n10"));
                ConverterValor();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    }
}

function ConverterValor() {
    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) {
        var valorMoedaCotacao = Globalize.parseFloat(_tituloFinanceiro.ValorMoedaCotacao.val());
        var valorOriginal = Globalize.parseFloat(_tituloFinanceiro.ValorOriginal.val());
        if (valorOriginal > 0 && valorMoedaCotacao > 0) {
            _tituloFinanceiro.ValorOriginalMoedaEstrangeira.val(Globalize.format(valorOriginal / valorMoedaCotacao, "n2"));
        }
        else
            _tituloFinanceiro.ValorOriginalMoedaEstrangeira.val("0,00");
    }
}

function RetornoPessoaTitulo(pessoa) {
    _tituloFinanceiro.Pessoa.codEntity(pessoa.Codigo);
    _tituloFinanceiro.Pessoa.val(pessoa.Descricao);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe && _tituloFinanceiro.TipoTitulo.val() === EnumTipoTitulo.APagar && _tituloFinanceiro.Codigo.val() <= 0) {
        executarReST("Pessoa/BuscarDadosPessoaPorEmpresa", { Codigo: pessoa.Codigo }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    if (r.Data.TipoMovimento != null) {
                        _tituloFinanceiro.TipoMovimento.codEntity(r.Data.TipoMovimento.Codigo);
                        _tituloFinanceiro.TipoMovimento.val(r.Data.TipoMovimento.Descricao);
                    }
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    }

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS && _tituloFinanceiro.FormaTitulo.val() === EnumFormaTitulo.Pix) {
        executarReST("Pessoa/BuscarPorCodigo", { Codigo: pessoa.Codigo }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    _tituloFinanceiro.TipoChavePixPessoa.val(r.Data.DadosBancarios.TipoChavePix);
                    _tituloFinanceiro.ChavePixPessoa.val(r.Data.DadosBancarios.ChavePix);
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    }
}

function RetornoConsultaDocumentoFaturamento(documento) {
    executarReST("DocumentoFaturamento/BuscarPorCodigo", { Codigo: documento.Codigo }, function (r) {
        if (r.Success) {
            if (r.Data) {
                limparCamposTituloFinanceiro();

                _tituloFinanceiro.DocumentoFaturamento.codEntity(r.Data.Codigo);
                _tituloFinanceiro.DocumentoFaturamento.val(r.Data.Documento + " (" + r.Data.Tipo + ")");

                _tituloFinanceiro.Pessoa.codEntity(r.Data.Tomador.Codigo);
                _tituloFinanceiro.Pessoa.val(r.Data.Tomador.Descricao);

                _tituloFinanceiro.GrupoPessoas.codEntity(r.Data.GrupoPessoas.Codigo);
                _tituloFinanceiro.GrupoPessoas.val(r.Data.GrupoPessoas.Descricao);

                _tituloFinanceiro.ValorOriginal.val(r.Data.ValorAFaturar);
                _tituloFinanceiro.DataEmissao.val(r.Data.DataEmissao);

                _tituloFinanceiro.TipoMovimento.codEntity(r.Data.TipoMovimentoUso.Codigo);
                _tituloFinanceiro.TipoMovimento.val(r.Data.TipoMovimentoUso.Descricao);

                _tituloFinanceiro.Sequencia.val("1");

                _tituloFinanceiro.TipoMovimento.enable(false);
                _tituloFinanceiro.TipoMovimento.required(false);
                _tituloFinanceiro.DataEmissao.enable(false);
                _tituloFinanceiro.DataAutorizacao.enable(false);
                _tituloFinanceiro.GrupoPessoas.enable(false);
                _tituloFinanceiro.Pessoa.enable(false);
                _tituloFinanceiro.Sequencia.enable(false);
                _tituloFinanceiro.Desconto.enable(false);
                _tituloFinanceiro.Acrescimo.enable(false);
                _tituloFinanceiro.TipoTitulo.enable(false);
                _tituloFinanceiro.ValorSaldo.enable(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function CodigoBarrasParaLinhaDigitavelClick(e, sender) {
    if (_tituloFinanceiro.NossoNumero.val() !== "") {
        var linhaDigitavel = calcula_linha(_tituloFinanceiro.NossoNumero.val());
        if (linhaDigitavel !== undefined && linhaDigitavel !== "") {
            _tituloFinanceiro.HabilitarLinhaDigitavelBoleto.val(true);
            _tituloFinanceiro.LinhaDigitavelBoleto.val(linhaDigitavel);
        }
    }
}

function LinhaDigitavelParaCodigoBarrasClick(e, sender) {
    if (_tituloFinanceiro.LinhaDigitavelBoleto.val() !== "") {
        var codigoBarras = calcula_barra(_tituloFinanceiro.LinhaDigitavelBoleto.val());
        if (codigoBarras !== undefined && codigoBarras !== "")
            _tituloFinanceiro.NossoNumero.val(codigoBarras);
    }
}

function ValidarValorBrutoClick(e, sender) {
    if (_tituloFinanceiro.NossoNumero.val() !== "" && _tituloFinanceiro.ValorOriginal.val() !== "") {
        validar_valor(_tituloFinanceiro.NossoNumero.val(), _tituloFinanceiro.ValorOriginal.val(), _tituloFinanceiro.ValorSaldo.val());
    }
}

function AdiantadoChange(e, sender) {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe) {
        if (_tituloFinanceiro.TipoTitulo.val() === EnumTipoTitulo.AReceber) {
            if ($("#" + _tituloFinanceiro.Adiantado.id).is(':checked')) {
                _tituloFinanceiro.Portador.text("*Portador");
                _tituloFinanceiro.Portador.visible(true);
                _tituloFinanceiro.Portador.required(true);
            } else {
                _tituloFinanceiro.Portador.visible(true);
                _tituloFinanceiro.Portador.required(false);
                _tituloFinanceiro.Portador.text("Portador");
            }
        }
    }
}

function ProvisaoChange(e, sender) {
    if (_tituloFinanceiro.Codigo.val() > 0) {
        if (_tituloFinanceiro.StatusTitulo.val() == EnumSituacaoTitulo.EmAberto && $("#" + _tituloFinanceiro.Provisao.id).is(':checked')) {
            _tituloFinanceiro.ValorOriginal.enable(true);
            _tituloFinanceiro.Desconto.enable(true);
            _tituloFinanceiro.Acrescimo.enable(true);
        } else if (_tituloFinanceiro.StatusTitulo.val() == EnumSituacaoTitulo.EmAberto) {
            _tituloFinanceiro.ValorOriginal.enable(false);
            _tituloFinanceiro.Desconto.enable(false);
            _tituloFinanceiro.Acrescimo.enable(false);
        }
    }

    if (!_CONFIGURACAO_TMS.MovimentacaoFinanceiraParaTitulosDeProvisao && _tituloFinanceiro.Provisao) {
        _tituloFinanceiro.DataEmissao.enable(true);
    }
}

function TipoTituloChange(e, sender) {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe) {
        _tituloFinanceiro.TipoMovimento.visible(true);
        _tituloFinanceiro.TipoMovimento.required(false);
        _tituloFinanceiro.LiberadoPagamento.visible(false);
        _tituloFinanceiro.Adiantado.val(false);
        if (_tituloFinanceiro.TipoTitulo.val() === EnumTipoTitulo.AReceber) {//receber            
            $("#liTabDocumentos").show();
            $("#liTabImposto").hide();
            LimparCampoEntity(_tituloFinanceiro.ConhecimentoEletronico);
            LimparDropzone();
            DropZoneCompleteTitulo();
            _tituloFinanceiro.Portador.required(false);
            _tituloFinanceiro.Portador.visible(true);
            _tituloFinanceiro.Portador.text("Portador");
            _tituloFinanceiro.Adiantado.visible(true);
            _tituloFinanceiro.NossoNumero.enable(false);
            _tituloFinanceiro.LinhaDigitavelBoleto.visible(false);
            _tituloFinanceiro.AdiantamentoFornecedor.visible(false);
        } else {
            $("#liTabDocumentos").hide();
            $("#liTabImposto").show();
            LimparCampoEntity(_tituloFinanceiro.ConhecimentoEletronico);
            LimparDropzone();
            DropZoneCompleteTitulo();
            _tituloFinanceiro.Portador.required(false);
            _tituloFinanceiro.Portador.visible(false);
            _tituloFinanceiro.Adiantado.visible(false);
            _tituloFinanceiro.NossoNumero.enable(true);
            _tituloFinanceiro.LinhaDigitavelBoleto.visible(true);
            _tituloFinanceiro.HabilitarLinhaDigitavelBoleto.enable(true);
            _tituloFinanceiro.AdiantamentoFornecedor.visible(true);
        }
        //AdiantadoChange(e, sender);
        _tituloFinanceiro.DocumentoFaturamento.visible(false);
    } else if (_tituloFinanceiro.TipoTitulo.val() === EnumTipoTitulo.AReceber) {//receber
        $("#liTabDocumentos").show();
        $("#liTabImposto").hide();
        LimparCampoEntity(_tituloFinanceiro.ConhecimentoEletronico);
        LimparDropzone();
        DropZoneCompleteTitulo();
        buscarConhecimentosFatura();
        _tituloFinanceiro.LiberadoPagamento.visible(false);
        _tituloFinanceiro.DocumentoFaturamento.visible(true);
        _tituloFinanceiro.NossoNumero.enable(false);
        _tituloFinanceiro.LinhaDigitavelBoleto.visible(false);
        _tituloFinanceiro.Portador.required(false);
        _tituloFinanceiro.Portador.visible(false);
        _tituloFinanceiro.CodigoFavorecido.visible(false);
        _tituloFinanceiro.AdiantamentoFornecedor.visible(false);
        AlterarVisibilidadeEObrigatoriedadeCamposPix(false);
    } else {
        $("#liTabDocumentos").hide();
        $("#liTabImposto").show();
        LimparCampoEntity(_tituloFinanceiro.ConhecimentoEletronico);
        LimparDropzone();
        DropZoneCompleteTitulo();
        _tituloFinanceiro.LiberadoPagamento.visible(false);
        _tituloFinanceiro.DocumentoFaturamento.visible(false);
        _tituloFinanceiro.NossoNumero.enable(true);
        _tituloFinanceiro.LinhaDigitavelBoleto.visible(true);
        _tituloFinanceiro.HabilitarLinhaDigitavelBoleto.enable(true);
        _tituloFinanceiro.Portador.required(false);
        _tituloFinanceiro.Portador.visible(true);
        _tituloFinanceiro.CodigoFavorecido.visible(true);
        _tituloFinanceiro.AdiantamentoFornecedor.visible(true);

        if (_tituloFinanceiro.FormaTitulo.val() === EnumFormaTitulo.Pix)
            AlterarVisibilidadeEObrigatoriedadeCamposPix(true);
    }
}

function FormaTituloChange(e, sender) {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
        if (_tituloFinanceiro.FormaTitulo.val() === EnumFormaTitulo.Pix && _tituloFinanceiro.TipoTitulo.val() === EnumTipoTitulo.APagar)
            AlterarVisibilidadeEObrigatoriedadeCamposPix(true);
        else
            AlterarVisibilidadeEObrigatoriedadeCamposPix(false);
    } 
}

function RetornoCTe(data) {
    _pesquisaTituloFinanceiro.Conhecimento.val(data.Numero + "-" + data.Serie);
    _pesquisaTituloFinanceiro.Conhecimento.codEntity(data.Codigo);
}

function RetornoFatura(data) {
    _pesquisaTituloFinanceiro.Fatura.val(data.Numero);
    _pesquisaTituloFinanceiro.Fatura.codEntity(data.Codigo);
}

function RetornoConhecimentoEletronico(data) {
    _tituloFinanceiro.ConhecimentoEletronico.val(data.Chave);
    _tituloFinanceiro.ConhecimentoEletronico.codEntity(data.Codigo);
}

function adicionarClick(e, sender) {
    PreencherListasSelecao();
    Salvar(e, "TituloFinanceiro/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                if (_tituloFinanceiro.ListaAnexosNovos.list.length > 0)
                    EnviarTituloFinanceiroAnexos(_tituloFinanceiro.Codigo.val(), false);

                if (_CONFIGURACAO_TMS.AbrirRateioDespesaVeiculoAutomaticamente && _tituloFinanceiro.TipoTitulo.val() === EnumTipoTitulo.APagar) {

                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso, favor digite o lançamento de rateio.");
                    _gridTituloFinanceiro.CarregarGrid();
                    limparCamposTituloFinanceiro();

                    LimparCamposRateioDespesa();
                    Global.abrirModal('divModalDespesaVeiculo');
                    _rateioDespesa.Titulo.val(arg.Data.Codigo);
                    _rateioDespesa.NumeroDocumento.val(arg.Data.Codigo);
                    _rateioDespesa.TipoDocumento.val(arg.Data.TipoDocumento);
                    _rateioDespesa.Valor.val(arg.Data.Valor);
                    _rateioDespesa.Pessoa.val(arg.Data.Pessoa.Descricao);
                    _rateioDespesa.Pessoa.codEntity(arg.Data.Pessoa.Codigo);

                } else {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                    _gridTituloFinanceiro.CarregarGrid();
                    limparCamposTituloFinanceiro();
                }
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    PreencherListasSelecao();
    Salvar(e, "TituloFinanceiro/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                if (_tituloFinanceiro.ListaAnexosNovos.list.length > 0)
                    EnviarTituloFinanceiroAnexos(_tituloFinanceiro.Codigo.val(), false);

                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridTituloFinanceiro.CarregarGrid();
                limparCamposTituloFinanceiro();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function EnviarTituloFinanceiroAnexos(codigoTituloFinanceiro, adicionar) {
    var formData = obterFormDataTituloFinanceiroAnexo();

    if (formData) {
        enviarArquivo("TituloFinanceiro/EnviarAnexos?callback=?", { Codigo: codigoTituloFinanceiro }, formData, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    }
}
function obterFormDataTituloFinanceiroAnexo() {
    var anexos = _tituloFinanceiro.ListaAnexosNovos.list;

    if (anexos.length > 0) {
        var formData = new FormData();

        anexos.forEach(function (anexo) {
            formData.append("Arquivo", anexo.Arquivo);
            formData.append("Descricao", anexo.DescricaoAnexo.val);
        });

        return formData;
    }

    return undefined;
}
function salvarClick(e, sender) {

    var valido = ValidarCamposObrigatorios(_tituloFinanceiro);

    if (_tituloFinanceiro.FormaTitulo.val() === EnumFormaTitulo.Pix && (_tituloFinanceiro.TipoChavePixPessoa.val() === EnumTipoChavePix.Nenhum || _tituloFinanceiro.ChavePixPessoa.val() == null || _tituloFinanceiro.ChavePixPessoa.val() == "")) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Configure as informações de PIX relacionadas a pessoa!");
        return;
    }

    if (valido) {
        var data = {
            TipoMovimento: _tituloFinanceiro.TipoMovimento.codEntity(),
            DataEmissao: _tituloFinanceiro.DataEmissao.val()
        };

        executarReST("PlanoOrcamentario/ValidaPlanoOrcamentarioEmpresa", data, function (arg) {
            if (arg.Success) {
                if (arg.Data != true && arg.Data.Mensagem != "") {
                    if (arg.Data.TipoLancamentoFinanceiroSemOrcamento == EnumTipoLancamentoFinanceiroSemOrcamento.Avisar) {
                        exibirConfirmacao("Confirmação", arg.Data.Mensagem, function () {
                            if (_tituloFinanceiro.Codigo.val() > 0)
                                atualizarClick(_tituloFinanceiro, sender);
                            else
                                adicionarClick(_tituloFinanceiro, sender);
                        });
                    } else
                        exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                } else {
                    if (_tituloFinanceiro.Codigo.val() > 0)
                        atualizarClick(_tituloFinanceiro, sender);
                    else
                        adicionarClick(_tituloFinanceiro, sender);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    } else
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja cancelar o título selecionado?", function () {
        ExcluirPorCodigo(_tituloFinanceiro, "TituloFinanceiro/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Cancelado com sucesso");
                    _gridTituloFinanceiro.CarregarGrid();
                    limparCamposTituloFinanceiro();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposTituloFinanceiro();
}

function GerarBoletoClick(data) {
    if (data != null) {
        if (data.Codigo > 0 && data.Codigo != "") {
            var data =
            {
                Codigo: data.Codigo,
                CodigoTitulo: _tituloFinanceiro.Codigo.val()
            };
            executarReST("TituloFinanceiro/GerarBoleto", data, function (e) {
                if (!e.Success) {
                    exibirMensagem(tipoMensagem.falha, "Falha", e.Msg);
                } else {
                    limparCamposTituloFinanceiro();
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Boleto enviado ao integrador.");
                    _gridTituloFinanceiro.CarregarGrid();
                }
            });
        }
    }
}

function VisualizarBoletoClick(e) {
    var dados = { Codigo: _tituloFinanceiro.Codigo.val() };
    executarDownload("TituloFinanceiro/DownloadBoleto", dados);
}

function LimparDadosBoletoClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja limpar os dados de boleto do título selecionado?", function () {
        var data =
        {
            CodigoTitulo: _tituloFinanceiro.Codigo.val()
        };
        executarReST("TituloFinanceiro/LimparDadosBoleto", data, function (e) {
            if (!e.Success) {
                exibirMensagem(tipoMensagem.falha, "Falha", e.Msg);
            } else {
                limparCamposTituloFinanceiro();
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Boleto excluído do sistema.");
                _gridTituloFinanceiro.CarregarGrid();
            }
        });
    });
}

function LimparDadosRemessaClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja limpar os dados da remessa do título selecionado?", function () {
        var data =
        {
            CodigoTitulo: _tituloFinanceiro.Codigo.val()
        };
        executarReST("TituloFinanceiro/LimparDadosRemessa", data, function (e) {
            if (!e.Success) {
                exibirMensagem(tipoMensagem.falha, "Falha", e.Msg);
            } else {
                limparCamposTituloFinanceiro();
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Boleto sem remessa.");
                _gridTituloFinanceiro.CarregarGrid();
            }
        });
    });
}

function multiploTituloClick(e, sender) {
    //var instancia = new LancarContas(function () { });
    new LancarContas();
}

function LimparRemessaPagamentoClick(e, sender) {
    exibirConfirmacao("Confirmação", "Deseja limpar todas as remessas de pagamentos vinculadas a este título?", function () {
        var data = { Codigo: _tituloFinanceiro.Codigo.val() };
        executarReST("TituloFinanceiro/LimparRemessaPagamento", data, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Remessa(s) removida(s) com sucesso.");
                    _crudTituloFinanceiro.LimparRemessaPagamento.visible(false);
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}

function gerarAutorizacaoPagamentoClick(e, sender) {
    var data = { Codigo: _tituloFinanceiro.Codigo.val() };
    executarReST("TituloFinanceiro/BaixarRelatorioAutorizacaoPagamento", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                BuscarProcessamentosPendentes();
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Aguarde que seu relatório está sendo gerado.");
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function CalcularValorPendente() {
    var valorOriginal = Globalize.parseFloat(_tituloFinanceiro.ValorOriginal.val());
    if (_tituloFinanceiro.ValorOriginal.val() != "")
        valorOriginal = parseFloat(valorOriginal);
    else
        valorOriginal = 0;

    var valorPago = Globalize.parseFloat(_tituloFinanceiro.ValorPago.val());
    valorPago = parseFloat(valorPago);

    if ((valorOriginal - valorPago) > 0)
        _tituloFinanceiro.ValorPendente.val(Globalize.format(valorOriginal - valorPago, "n2"));
    else
        _tituloFinanceiro.ValorPendente.val(Globalize.format(0, "n2"));

    var valorSaldo = 0;
    if (_tituloFinanceiro.StatusTitulo.val() == EnumSituacaoTitulo.Quitado)
        valorSaldo = 0;
    else {
        var valorDesconto = Globalize.parseFloat(_tituloFinanceiro.Desconto.val());
        if (_tituloFinanceiro.Desconto.val() != "")
            valorDesconto = parseFloat(valorDesconto);
        else
            valorDesconto = 0;

        var valorAcrescimo = Globalize.parseFloat(_tituloFinanceiro.Acrescimo.val());
        if (_tituloFinanceiro.Acrescimo.val() != "")
            valorAcrescimo = parseFloat(valorAcrescimo);
        else
            valorAcrescimo = 0;

        valorSaldo = valorOriginal - valorDesconto + valorAcrescimo;
    }
    _tituloFinanceiro.ValorSaldo.val(Globalize.format(valorSaldo, "n2"));
    ConverterValor();
}

//*******MÉTODOS*******

function CarregarGridDocumentosTitulo() {
    _gridDocumentos = new GridView(_tituloFinanceiro.PesquisarDocumentosTitulo.idGrid, "TituloFinanceiro/ConsultarDocumentos", _tituloFinanceiro, null, { column: 0, dir: orderDir.asc });
    _gridDocumentos.CarregarGrid();
}

function buscarConhecimentosFatura() {
    _gridConhecimentosFatura = new GridView(_tituloFinanceiro.ConhecimentosFatura.idGrid, "TituloFinanceiro/PesquisaConhecimentos", _tituloFinanceiro);
    _gridConhecimentosFatura.CarregarGrid();
}

function buscarTituloFinanceiros() {
    var editar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: editarTituloFinanceiro, tamanho: "7", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    var configExportacao = {
        url: "TituloFinanceiro/ExportarPesquisa",
        titulo: "Título Financeiro"
    };

    _gridTituloFinanceiro = new GridViewExportacao(_pesquisaTituloFinanceiro.Pesquisar.idGrid, "TituloFinanceiro/Pesquisa", _pesquisaTituloFinanceiro, menuOpcoes, configExportacao, { column: 0, dir: orderDir.desc });
    _gridTituloFinanceiro.CarregarGrid();
}

function editarTituloFinanceiro(TituloFinanceiroGrid) {
    limparCamposTituloFinanceiro();
    _tituloFinanceiro.Codigo.val(TituloFinanceiroGrid.Codigo);
    BuscarPorCodigo(_tituloFinanceiro, "TituloFinanceiro/BuscarPorCodigo", function (arg) {

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe) {
            $("#liTabDocumentos").show();
            $("#liTabImposto").hide();
            _tituloFinanceiro.LiberadoPagamento.visible(false);
            if (_tituloFinanceiro.TipoTitulo.val() === EnumTipoTitulo.AReceber) {//receber
                $("#liTabDocumentos").show();
                $("#liTabImposto").hide();
                LimparDropzone();
                DropZoneCompleteTitulo();

                _tituloFinanceiro.Portador.required(false);
                _tituloFinanceiro.Portador.visible(true);
                _tituloFinanceiro.Portador.text("Portador");
                _tituloFinanceiro.Adiantado.visible(true);
                _tituloFinanceiro.NossoNumero.enable(false);
                _tituloFinanceiro.LinhaDigitavelBoleto.visible(false);
                _tituloFinanceiro.AdiantamentoFornecedor.visible(false);
            } else {
                $("#liTabDocumentos").hide();
                $("#liTabImposto").show();
                LimparDropzone();
                DropZoneCompleteTitulo();

                _tituloFinanceiro.Portador.required(false);
                _tituloFinanceiro.Portador.visible(true);
                _tituloFinanceiro.Portador.text("Portador");
                _tituloFinanceiro.Adiantado.visible(false);
                _tituloFinanceiro.NossoNumero.enable(true);
                _tituloFinanceiro.LinhaDigitavelBoleto.visible(true);
                _tituloFinanceiro.HabilitarLinhaDigitavelBoleto.enable(true);
                _tituloFinanceiro.AdiantamentoFornecedor.visible(true);
            }
            AdiantadoChange();
        }
        else if (_tituloFinanceiro.TipoTitulo.val() === EnumTipoTitulo.AReceber) {//receber
            $("#liTabDocumentos").show();
            $("#liTabImposto").hide();
            LimparDropzone();
            DropZoneCompleteTitulo();
            buscarConhecimentosFatura();
            _tituloFinanceiro.LiberadoPagamento.visible(false);
            _tituloFinanceiro.NossoNumero.enable(false);
            _tituloFinanceiro.Portador.visible(false);
            _tituloFinanceiro.Portador.text("Portador");
            _tituloFinanceiro.CodigoFavorecido.visible(false);
            _tituloFinanceiro.DescontoAplicadoNegociacao.visible(false);
            _tituloFinanceiro.AdiantamentoFornecedor.visible(false);
            AlterarVisibilidadeEObrigatoriedadeCamposPix(false);
        } else {
            $("#liTabDocumentos").hide();
            $("#liTabImposto").show();
            LimparDropzone();
            DropZoneCompleteTitulo();
            _tituloFinanceiro.LiberadoPagamento.visible(false);
            _tituloFinanceiro.NossoNumero.enable(true);
            _tituloFinanceiro.Portador.visible(true);
            _tituloFinanceiro.Portador.text("Portador");
            _tituloFinanceiro.CodigoFavorecido.visible(true);
            _tituloFinanceiro.DescontoAplicadoNegociacao.visible(true);
            _tituloFinanceiro.AdiantamentoFornecedor.visible(true);

            if (_tituloFinanceiro.FormaTitulo.val() === EnumFormaTitulo.Pix)
                AlterarVisibilidadeEObrigatoriedadeCamposPix(true);
            else
                AlterarVisibilidadeEObrigatoriedadeCamposPix(false);
        }

        _pesquisaTituloFinanceiro.ExibirFiltros.visibleFade(false);
        _crudTituloFinanceiro.Atualizar.visible(true);
        _crudTituloFinanceiro.Cancelar.visible(true);
        _crudTituloFinanceiro.Excluir.visible(true);
        _crudTituloFinanceiro.Adicionar.visible(false);
        _crudTituloFinanceiro.MultiploTitulo.visible(false);
        _crudTituloFinanceiro.GerarAutorizacaoPagamento.visible(true);

        if (arg.Data.ContemRemessaPagamento)
            _crudTituloFinanceiro.LimparRemessaPagamento.visible(true);
        else
            _crudTituloFinanceiro.LimparRemessaPagamento.visible(false);

        if (_tituloFinanceiro.StatusTitulo.val() == EnumSituacaoTitulo.Quitado || _tituloFinanceiro.StatusTitulo.val() == EnumSituacaoTitulo.Cancelado) {
            DesabilitarCamposInstanciasTitulo();
            _tituloFinanceiro.Observacao.enable(true);
            _tituloFinanceiro.ObservacaoInterna.enable(true);
        } else
            HabilitarCamposInstanciasTitulo();

        _tituloFinanceiro.Historico.enable(false);
        _tituloFinanceiro.ValorOriginalMoedaEstrangeira.enable(false);
        _tituloFinanceiro.ValorPendente.enable(false);

        _crudTituloFinanceiro.LimparDadosBoleto.visible(false);
        _crudTituloFinanceiro.LimparDadosRemessa.visible(false);

        if (_tituloFinanceiro.TipoTitulo.val() === EnumTipoTitulo.AReceber && (_tituloFinanceiro.NossoNumero.val() == "" || _tituloFinanceiro.NossoNumero.val() == null) && (_tituloFinanceiro.StatusTitulo.val() == EnumSituacaoTitulo.EmAberto || _tituloFinanceiro.StatusTitulo.val() == 0 || _tituloFinanceiro.StatusTitulo.val() == ""))
            _crudTituloFinanceiro.GerarBoleto.visible(true);
        else if (_tituloFinanceiro.TipoTitulo.val() === EnumTipoTitulo.AReceber && _tituloFinanceiro.NossoNumero.val() != "" && _tituloFinanceiro.NossoNumero.val() != null)
            _crudTituloFinanceiro.VisualizarBoleto.visible(true);
        else {
            _crudTituloFinanceiro.GerarBoleto.visible(false);
            _crudTituloFinanceiro.VisualizarBoleto.visible(false);
        }

        if (_tituloFinanceiro.BoletoStatusTitulo.val() == 2 || _tituloFinanceiro.BoletoStatusTitulo.val() == 5 || _tituloFinanceiro.BoletoStatusTitulo.val() == 6)
            _crudTituloFinanceiro.LimparDadosBoleto.visible(true);
        if (_tituloFinanceiro.BoletoStatusTitulo.val() == 5 || _tituloFinanceiro.BoletoStatusTitulo.val() == 6)
            _crudTituloFinanceiro.LimparDadosRemessa.visible(true);

        _tituloFinanceiro.Codigo.enable(false);
        _tituloFinanceiro.CodigoRecebidoIntegracao.enable(false);
        _tituloFinanceiro.Fatura.enable(false);
        _tituloFinanceiro.DataEmissao.enable(false);
        _tituloFinanceiro.DataAutorizacao.enable(false);
        _tituloFinanceiro.DataLiquidacao.enable(false);
        _tituloFinanceiro.DataBaseLiquidacao.enable(false);
        _tituloFinanceiro.StatusTitulo.enable(false);
        _tituloFinanceiro.ValorPago.enable(false);
        _tituloFinanceiro.ValorOriginal.enable(false);
        _tituloFinanceiro.Desconto.enable(false);
        _tituloFinanceiro.Acrescimo.enable(false);
        _tituloFinanceiro.ValorSaldo.enable(false);
        _tituloFinanceiro.TipoChavePixPessoa.enable(false);
        _tituloFinanceiro.ChavePixPessoa.enable(false);

        if (_tituloFinanceiro.TipoTitulo.val() === EnumTipoTitulo.AReceber) {
            _tituloFinanceiro.NossoNumero.enable(false);
            _tituloFinanceiro.LinhaDigitavelBoleto.visible(false);
        } else if (_tituloFinanceiro.StatusTitulo.val() === EnumSituacaoTitulo.EmAberto) {
            _tituloFinanceiro.NossoNumero.enable(true);
            _tituloFinanceiro.LinhaDigitavelBoleto.visible(true);
            _tituloFinanceiro.LinhaDigitavelBoleto.enable(_tituloFinanceiro.HabilitarLinhaDigitavelBoleto.val());
            _tituloFinanceiro.HabilitarLinhaDigitavelBoleto.enable(true);
        }

        _tituloFinanceiro.TipoTitulo.enable(false);

        if (_tituloFinanceiro.DocumentoFaturamento.codEntity() > 0) {
            _tituloFinanceiro.Sequencia.enable(false);
            _tituloFinanceiro.Desconto.enable(false);
            _tituloFinanceiro.Acrescimo.enable(false);
            _tituloFinanceiro.ValorSaldo.enable(false);
            _tituloFinanceiro.ConhecimentoEletronico.visible(false);
            _tituloFinanceiro.ConhecimentosFatura.visible(false);
            _tituloFinanceiro.Dropzone.visible(false);
            _tituloFinanceiro.PesquisarDocumentosTitulo.visible(true);
            _gridDocumentos.CarregarGrid();
        }

        _tituloFinanceiro.DocumentoFaturamento.visible(false);
        _tituloFinanceiro.TipoMovimento.enable(false);
        _tituloFinanceiro.TipoMovimento.required(false);

        _contatoClienteTitulo.ShowButton();
        
        if (_tituloFinanceiro.Provisao.val()) {
            ProvisaoChange();
        }
        else {
            _tituloFinanceiro.Provisao.visible(false);
        }

        if (_tituloFinanceiro.TituloDeInfracao.val() && _tituloFinanceiro.StatusTitulo.val() === EnumSituacaoTitulo.EmAberto) {
            _tituloFinanceiro.Desconto.enable(true);
            _tituloFinanceiro.Acrescimo.enable(true);
        }

        RecarregarGridCentrosResultado();
        RecarregarGridCentroResultadoTipoDespesa();
        RecarregarGridVeiculo();
        recarregarTituloFinanceiroIntegracoes();
        RecarregarGridTituloFinanceiroAnexo();
    }, null);
}

function limparCamposTituloFinanceiro() {
    _contatoClienteTitulo.HideButton();
    
    _crudTituloFinanceiro.Atualizar.visible(false);
    _crudTituloFinanceiro.Cancelar.visible(false);
    _crudTituloFinanceiro.Excluir.visible(false);
    _crudTituloFinanceiro.Adicionar.visible(true);
    if (!_CONFIGURACAO_TMS.UsuarioAdministrador && !VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Titulo_GerarMultiplosTitulos, _PermissoesPersonalizadas))
        _crudTituloFinanceiro.MultiploTitulo.visible(false);
    else
        _crudTituloFinanceiro.MultiploTitulo.visible(true);
    _crudTituloFinanceiro.GerarAutorizacaoPagamento.visible(false);
    _crudTituloFinanceiro.LimparRemessaPagamento.visible(false);
    _crudTituloFinanceiro.GerarBoleto.visible(false);
    _crudTituloFinanceiro.VisualizarBoleto.visible(false);
    _crudTituloFinanceiro.LimparDadosBoleto.visible(false);
    _crudTituloFinanceiro.LimparDadosRemessa.visible(false);

    LimparCampos(_tituloFinanceiro);
    HabilitarCamposInstanciasTitulo();
    _tituloFinanceiro.Historico.enable(false);
    _tituloFinanceiro.ValorOriginalMoedaEstrangeira.enable(false);
    _tituloFinanceiro.ValorPendente.enable(false);
    HabilitarTipoMovimento(true);
    _tituloFinanceiro.Dropzone.visible(true);
    _tituloFinanceiro.LiberadoPagamento.visible(false);
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe) {
        $("#liTabDocumentos").show();
    }
    else {
        $("#liTabDocumentos").show();
        _gridConhecimentosFatura.CarregarGrid();
        //_gridDocumentos.CarregarGrid();
    }
    $("#liTabImposto").hide();
    LimparDropzone();
    DropZoneCompleteTitulo();
    _tituloFinanceiro.Codigo.enable(false);
    _tituloFinanceiro.CodigoRecebidoIntegracao.enable(false);
    _tituloFinanceiro.Fatura.enable(false);
    _tituloFinanceiro.DataEmissao.enable(true);
    _tituloFinanceiro.DataLiquidacao.enable(false);
    _tituloFinanceiro.DataBaseLiquidacao.enable(false);
    _tituloFinanceiro.StatusTitulo.enable(false);
    _tituloFinanceiro.ValorPago.enable(false);
    _tituloFinanceiro.ValorOriginal.enable(true);
    _tituloFinanceiro.Desconto.enable(true);
    _tituloFinanceiro.Acrescimo.enable(true);
    if (_tituloFinanceiro.TipoTitulo.val() === EnumTipoTitulo.AReceber) {
        _tituloFinanceiro.NossoNumero.enable(false);
        _tituloFinanceiro.LinhaDigitavelBoleto.visible(false);
        _tituloFinanceiro.Portador.visible(false);
        _tituloFinanceiro.Portador.text("Portador");
        AlterarVisibilidadeEObrigatoriedadeCamposPix(false);
    }
    else {
        _tituloFinanceiro.NossoNumero.enable(true);
        _tituloFinanceiro.LinhaDigitavelBoleto.visible(true);
        _tituloFinanceiro.LinhaDigitavelBoleto.enable(false);
        _tituloFinanceiro.HabilitarLinhaDigitavelBoleto.enable(true);
        _tituloFinanceiro.Portador.visible(true);
        _tituloFinanceiro.Portador.text("Portador");

        if (_tituloFinanceiro.FormaTitulo.val() === EnumFormaTitulo.Pix)
            AlterarVisibilidadeEObrigatoriedadeCamposPix(true);
        else
            AlterarVisibilidadeEObrigatoriedadeCamposPix(false);
    }
    _tituloFinanceiro.CodigoFavorecido.visible(false);
    _tituloFinanceiro.DescontoAplicadoNegociacao.visible(false);
    _tituloFinanceiro.TipoTitulo.enable(true);

    _tituloFinanceiro.DataEmissao.val(Global.DataAtual());
    _tituloFinanceiro.ConhecimentoEletronico.visible(true);
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe) {
        _tituloFinanceiro.DocumentoFaturamento.visible(false);
        _tituloFinanceiro.ConhecimentosFatura.visible(false);
        _tituloFinanceiro.Portador.visible(true);
        _tituloFinanceiro.Portador.required(false);
        _tituloFinanceiro.Portador.text("Portador");
        _tituloFinanceiro.Adiantado.visible(true);
    }
    else {
        _tituloFinanceiro.DocumentoFaturamento.visible(true);
        _tituloFinanceiro.ConhecimentosFatura.visible(true);
    }

    _tituloFinanceiro.Provisao.visible(true);
    _tituloFinanceiro.PesquisarDocumentosTitulo.visible(false);
    _tituloFinanceiro.AdiantamentoFornecedor.visible(false);

    _tituloFinanceiro.TipoChavePixPessoa.enable(false);
    _tituloFinanceiro.ChavePixPessoa.enable(false);

    LimparCamposCentroResultadoTipoDespesa();
    
    RecarregarGridCentrosResultado();
    RecarregarGridVeiculo();
    recarregarTituloFinanceiroIntegracoes();
    $("#liTabIntegracoes").show();
    LimparCamposTituloFinanceiroAnexo();


    Global.ResetarMultiplasAbas();
}

function HabilitarCamposInstanciasTitulo() {
    SetarEnableCamposKnockout(_tituloFinanceiro, true);
    _tituloFinanceiro.Dropzone.visible(true);
    _veiculo.Veiculo.visible(true);
    SetarEnableCamposKnockout(_centroResultado, true);
    SetarEnableCamposKnockout(_centroResultadoTipoDespesa, true);
}

function DesabilitarCamposInstanciasTitulo() {
    SetarEnableCamposKnockout(_tituloFinanceiro, false);
    _tituloFinanceiro.Dropzone.visible(false);
    _veiculo.Veiculo.visible(false);
    SetarEnableCamposKnockout(_centroResultado, false);
    SetarEnableCamposKnockout(_centroResultadoTipoDespesa, false);
}

function HabilitarTipoMovimento(v) {
    _tituloFinanceiro.TipoMovimento.required(v);
    _tituloFinanceiro.TipoMovimento.visible(v);
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe)
        _tituloFinanceiro.TipoMovimento.required(false);
}

function LimparDropzone() {
    //dropzone.removeAllFiles(true);
    //clearDropzone();
    //_tituloFinanceiro.Dropzone.removeAllFiles(true);
    //$("#" + _tituloFinanceiro.Dropzone.id).removeAllFiles(true);
    //$('.dropzone')[0].dropzone.files.forEach(function (file) {
    //    file.previewElement.remove();
    //});

    //$('.dropzone').removeClass('dz-started');
}

function PreencherListasSelecao() {
    var veiculosSelecao = new Array();

    $.each(_veiculo.Veiculo.basicTable.BuscarRegistros(), function (i, veiculo) {
        veiculosSelecao.push(veiculo.Codigo);
    });

    _tituloFinanceiro.Veiculos.val(JSON.stringify(veiculosSelecao));
}

function AlterarVisibilidadeEObrigatoriedadeCamposPix(ativo) {
    _tituloFinanceiro.TipoChavePixPessoa.required(ativo);
    _tituloFinanceiro.TipoChavePixPessoa.visible(ativo);
    _tituloFinanceiro.ChavePixPessoa.required(ativo);
    _tituloFinanceiro.ChavePixPessoa.visible(ativo);
}