/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/Empresa.js" />
/// <reference path="../../Consultas/CFOP.js" />
/// <reference path="../../Consultas/NaturezaOperacao.js" />
/// <reference path="../../Consultas/ModeloDocumentoFiscal.js" />
/// <reference path="../../Consultas/EspecieDocumentoFiscal.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Consultas/OrdemServico.js" />
/// <reference path="../../Consultas/Equipamento.js" />
/// <reference path="../../Consultas/OrdemCompra.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/ContratoFinanciamento.js" />
/// <reference path="../../Consultas/SituacaoLancamentoDocumentoEntrada.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/Servico.js" />
/// <reference path="../../Consultas/Produto.js" />
/// <reference path="../../Enumeradores/EnumSituacaoDocumentoEntrada.js" />
/// <reference path="../../Enumeradores/EnumIndicadorPagamentoDoucumentoEntrada.js" />
/// <reference path="../../Enumeradores/EnumFinalidadeTipoMovimento.js" />
/// <reference path="../../Enumeradores/EnumSituacaoOrdemCompra.js" />
/// <reference path="../../Enumeradores/EnumModalidadeFrete.js" />
/// <reference path="../../Enumeradores/EnumMoedaCotacaoBancoCentral.js" />
/// <reference path="../../Enumeradores/EnumTipoDocumentoServico.js" />
/// <reference path="../../Enumeradores/EnumCSTServico.js" />
/// <reference path="Veiculo.js" />
/// <reference path="Item.js" />
/// <reference path="Duplicata.js" />
/// <reference path="Guia.js" />
/// <reference path="CentroCusto.js" />
/// <reference path="DuplicataGeracaoAutomatica.js" />
/// <reference path="QualificaoFornecedor.js" />
/// <reference path="CentroResultadoTipoDespesa.js" />
/// <reference path="Integracoes.js" />
/// <reference path="../RateioDespesaVeiculo/RateioDespesaVeiculo.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridDocumentoEntrada, _documentoEntrada, _pesquisaDocumentoEntrada, _crudDocumentoEntrada, _PermissoesPersonalizadas;
var _buscaCFOP;

var PesquisaDocumentoEntrada = function () {
    this.DataEmissaoInicial = PropertyEntity({ text: "Data de Emissão Inicial:", getType: typesKnockout.date });
    this.DataEmissaoFinal = PropertyEntity({ text: "Data de Emissão Final:", getType: typesKnockout.date });
    this.DataEntradaInicial = PropertyEntity({ text: "Data de Entrada Inicial:", getType: typesKnockout.date });
    this.DataEntradaFinal = PropertyEntity({ text: "Data de Entrada Final:", getType: typesKnockout.date });
    this.NumeroLancamentoInicial = PropertyEntity({ text: "Número do Lançamento Inicial:", getType: typesKnockout.int, maxlength: 10 });
    this.NumeroLancamentoFinal = PropertyEntity({ text: "Número do Lançamento Final:", getType: typesKnockout.int, maxlength: 10 });
    this.NumeroDocumentoInicial = PropertyEntity({ text: "Número do Documento Inicial:", getType: typesKnockout.int, maxlength: 100 });
    this.NumeroDocumentoFinal = PropertyEntity({ text: "Número do Documento Final:", getType: typesKnockout.int, maxlength: 100 });
    this.ValorInicial = PropertyEntity({ text: "Valor Inicial:", getType: typesKnockout.decimal, maxlength: 15 });
    this.ValorFinal = PropertyEntity({ text: "Valor Final:", getType: typesKnockout.decimal, maxlength: 15 });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoDocumentoEntrada.Todos), options: EnumSituacaoDocumentoEntrada.obterOpcoesPesquisa(), def: EnumSituacaoDocumentoEntrada.Todos, text: "Situação: " });
    this.Fornecedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Fornecedor:", idBtnSearch: guid() });
    this.NumeroTitulo = PropertyEntity({ text: "Número do Título: ", getType: typesKnockout.int, maxlength: 16 });
    this.Chave = PropertyEntity({ text: "Chave:", maxlength: 44 });
    this.StatusFinanceiro = PropertyEntity({ val: ko.observable(""), options: EnumStatusFinanceiroDocumentoEntrada.obterOpcoesPesquisa(), def: "", text: "Status Financeiro: " });
    this.Produto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Produto:", idBtnSearch: guid() });
    this.NumeroFogo = PropertyEntity({ text: "Núm. de Fogo:", getType: typesKnockout.int });

    this.NaturezaOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Natureza da Operação:", idBtnSearch: guid() });
    this.CFOP = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "CFOP:", idBtnSearch: guid() });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid() });
    this.TipoMovimento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Movimento:", idBtnSearch: guid() });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destinatário:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Categoria = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Categoria:", idBtnSearch: guid() });
    this.PesquisaStatusLancamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Status Lancamento:", idBtnSearch: guid() });

    this.DataEmissaoInicial.dateRangeLimit = this.DataEmissaoFinal;
    this.DataEmissaoFinal.dateRangeInit = this.DataEmissaoInicial;
    this.DataEntradaInicial.dateRangeLimit = this.DataEntradaFinal;
    this.DataEntradaFinal.dateRangeInit = this.DataEntradaInicial;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridDocumentoEntrada.CarregarGrid();
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

var DocumentoEntrada = function () {
    var dataAtual = moment().format("DD/MM/YYYY");

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.NumeroLancamento = PropertyEntity({ text: "Número do Lançamento:", val: ko.observable("Automático"), def: "Automático", enable: ko.observable(false) });
    this.DataEntrada = PropertyEntity({ text: "*Data de Entrada:", getType: typesKnockout.date, val: ko.observable(dataAtual), def: dataAtual, required: true, enable: ko.observable(true) });
    this.DataEmissao = PropertyEntity({ text: "*Data de Emissão:", getType: typesKnockout.dateTime, required: true, enable: ko.observable(true) });
    this.Numero = PropertyEntity({ text: "*Número:", getType: typesKnockout.int, maxlength: 15, required: true, enable: ko.observable(true) });
    this.Serie = PropertyEntity({ text: ko.observable("Série:"), maxlength: 10, required: ko.observable(false), enable: ko.observable(true) });
    this.Chave = PropertyEntity({ text: ko.observable("Chave:"), maxlength: 50, required: ko.observable(false), enable: ko.observable(true) });
    this.ChaveNotaAnulacao = PropertyEntity({ text: "*Chave da NF-e de Anulação:", maxlength: 44, required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(false) });
    this.Observacao = PropertyEntity({ text: "Observação:", maxlength: 5000, enable: ko.observable(true), visible: ko.observable(true) });

    this.MoedaCotacaoBancoCentral = PropertyEntity({ val: ko.observable(EnumMoedaCotacaoBancoCentral.Real), options: EnumMoedaCotacaoBancoCentral.obterOpcoes(), def: EnumMoedaCotacaoBancoCentral.Real, text: "Moeda: ", visible: ko.observable(false), enable: ko.observable(true) });
    this.DataBaseCRT = PropertyEntity({ text: "Data Base CRT: ", required: false, getType: typesKnockout.dateTime, enable: ko.observable(true), visible: ko.observable(false) });
    this.ValorMoedaCotacao = PropertyEntity({ text: "Valor Moeda: ", required: false, getType: typesKnockout.decimal, enable: ko.observable(true), visible: ko.observable(false), configDecimal: { precision: 10, allowZero: false, allowNegative: false }, maxlength: 22 });

    //Aba Totais Gerais
    this.ValorTotal = PropertyEntity({ text: "*Valor Total:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,00"), def: "0,00", required: true, configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true) });
    this.ValorTotalDesconto = PropertyEntity({ text: "Valor Total de Descontos:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true) });
    this.ValorTotalOutrasDespesas = PropertyEntity({ text: "Valor Total Outras Despesas:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true) });
    this.ValorTotalFrete = PropertyEntity({ text: "Valor Total do Frete:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true) });
    this.BaseCalculoICMS = PropertyEntity({ text: "Base de Cálculo ICMS:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true) });
    this.ValorTotalICMS = PropertyEntity({ text: "Valor Total ICMS:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true) });
    this.BaseCalculoICMSST = PropertyEntity({ text: "Base de Cálculo ICMS ST:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true) });
    this.ValorTotalICMSST = PropertyEntity({ text: "Valor Total ICMS ST:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true) });
    this.ValorTotalIPI = PropertyEntity({ text: "Valor Total IPI:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true) });
    this.ValorTotalPIS = PropertyEntity({ text: "Valor Total PIS:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true) });
    this.ValorTotalCOFINS = PropertyEntity({ text: "Valor Total COFINS:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true) });
    this.ValorTotalCreditoPresumido = PropertyEntity({ text: "Valor Total Crédito Presumido:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true) });
    this.ValorTotalDiferencial = PropertyEntity({ text: "Valor Total Diferencial:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true) });
    this.ValorTotalSeguro = PropertyEntity({ text: "Valor Total Seguro:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true) });
    this.ValorTotalFreteFora = PropertyEntity({ text: "Valor Total Frete Fora:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true) });
    this.ValorTotalOutrasDespesasFora = PropertyEntity({ text: "Valor Total Outras Despesas Fora:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true) });
    this.ValorTotalDescontoFora = PropertyEntity({ text: "Valor Total Desconto Fora:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true) });
    this.ValorTotalImpostosFora = PropertyEntity({ text: "Valor Total Impostos Fora:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true) });
    this.ValorTotalDiferencialFreteFora = PropertyEntity({ text: "Valor Total Diferencial do Frete Fora:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true) });
    this.ValorTotalICMSFreteFora = PropertyEntity({ text: "Valor Total ICMS do Frete Fora:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true) });
    this.ValorTotalCusto = PropertyEntity({ text: "Valor Total Custo:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true) });
    this.ValorProdutos = PropertyEntity({ text: "Valor dos Produtos:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true) });
    this.ValorBruto = PropertyEntity({ text: "Valor Bruto:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true) });
    this.BaseSTRetido = PropertyEntity({ text: "Base ST Retido:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true) });
    this.ValorSTRetido = PropertyEntity({ text: "Valor ST Retido:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true) });

    //Aba Retenções
    this.ValorTotalRetencaoPIS = PropertyEntity({ text: "Total Retenção PIS:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true) });
    this.ValorTotalRetencaoCOFINS = PropertyEntity({ text: "Total Retenção COFINS:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true) });
    this.ValorTotalRetencaoINSS = PropertyEntity({ text: "Total Retenção INSS:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true) });
    this.ValorTotalRetencaoIPI = PropertyEntity({ text: "Total Retenção IPI:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true) });
    this.ValorTotalRetencaoCSLL = PropertyEntity({ text: "Total Retenção CSLL:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true) });
    this.ValorTotalRetencaoOutras = PropertyEntity({ text: "Total Outras Retenções:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true) });
    this.ValorTotalRetencaoIR = PropertyEntity({ text: "Total Retenção IR:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true) });
    this.ValorTotalRetencaoISS = PropertyEntity({ text: "Total Retenção ISS:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true) });

    //Aba Serviço
    this.Servico = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Serviço:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.LocalidadePrestacaoServico = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Localidade da Prestação do Serviço:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.TipoDocumento = PropertyEntity({ text: "Tipo do Documento: ", val: ko.observable(EnumTipoDocumentoServico.Todas), options: EnumTipoDocumentoServico.obterOpcoesCadastro(), def: EnumTipoDocumentoServico.Todas, enable: ko.observable(true) });
    this.CSTServico = PropertyEntity({ text: "CST do Serviço: ", val: ko.observable(EnumCSTServico.Todas), options: EnumCSTServico.obterOpcoesCadastro(), def: EnumCSTServico.Todas, enable: ko.observable(true) });
    this.AliquotaSimplesNacional = PropertyEntity({ text: "Alíquota Simples Nacional:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true) });
    this.DocumentoFiscalProvenienteSimplesNacional = PropertyEntity({ text: "Documento Fiscal Proveniente de Optante Simples Nacional?", val: ko.observable(false), getType: typesKnockout.bool, def: false, enable: ko.observable(true) });
    this.TributaISSNoMunicipio = PropertyEntity({ text: "Tributa ISS no Município?", val: ko.observable(false), getType: typesKnockout.bool, def: false, enable: ko.observable(true) });

    this.Especie = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Espécie:", idBtnSearch: guid(), required: true, enable: ko.observable(true) });
    this.Modelo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Modelo:", idBtnSearch: guid(), required: true, enable: ko.observable(true) });
    this.NaturezaOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Natureza da Operação:", idBtnSearch: guid(), required: true, enable: ko.observable(true) });
    this.CFOP = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*CFOP:", idBtnSearch: guid(), required: true, enable: ko.observable(true) });
    this.Fornecedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Fornecedor:", idBtnSearch: guid(), required: true, enable: ko.observable(true) });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Destinatário:", idBtnSearch: guid(), required: ko.observable(true), enable: ko.observable(true), visible: ko.observable(true) });
    this.Equipamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Equipamento:", idBtnSearch: guid(), enable: ko.observable(true), cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-2 col-lg-2") });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.Horimetro = PropertyEntity({ text: "Horímetro:", enable: ko.observable(true), getType: typesKnockout.int, configInt: { precision: 0, allowZero: false }, maxlength: 11, cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-1 col-lg-1") });
    this.KMAbastecimento = PropertyEntity({ text: "KM Abastec.:", enable: ko.observable(true), getType: typesKnockout.int, configInt: { precision: 0, allowZero: false }, maxlength: 11, cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-1 col-lg-1") });
    this.DataAbastecimento = PropertyEntity({ text: "Data Abastec.:", getType: typesKnockout.dateTime, val: ko.observable(""), def: "", required: false, enable: ko.observable(true) });
    this.OrdemServico = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Ordem de Serviço:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.TipoMovimento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Tipo de Movimento:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true), required: ko.observable(true) });
    this.OrdemCompra = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Ordem de Compra:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });

    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoDocumentoEntrada.Aberto), options: EnumSituacaoDocumentoEntrada.obterOpcoes(), def: EnumSituacaoDocumentoEntrada.Aberto, text: "*Situação: ", enable: ko.observable(true) });
    this.IndicadorPagamento = PropertyEntity({ val: ko.observable(EnumIndicadorPagamentoDocumentoEntrada.APrazo), options: EnumIndicadorPagamentoDocumentoEntrada.obterOpcoes(), def: EnumIndicadorPagamentoDocumentoEntrada.APrazo, text: "*Indicador de Pagamento: ", enable: ko.observable(true) });
    this.TipoFrete = PropertyEntity({ text: "Tipo Frete: ", val: ko.observable(""), options: EnumModalidadeFrete.obterOpcoes(), def: "", enable: ko.observable(true), visible: ko.observable(true) });
    this.EncerrarOrdemServico = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Deseja encerrar a O.S. após finalizar esta nota?", def: false, visible: ko.observable(false), enable: ko.observable(true) });

    this.Expedidor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Expedidor:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(false) });
    this.Recebedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Recebedor:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(false) });
    this.LocalidadeInicioPrestacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Início da Prestação:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(false) });
    this.LocalidadeTerminoPrestacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Término da Prestação:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(false) });
    this.ContratoFinanciamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Contrato Financiamento:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.StatusLancamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Status Lancamento:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });

    this.Itens = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.Duplicatas = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.Guias = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.CentroCustos = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.CentrosResultadoTiposDespesa = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.Veiculos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.ItensAbastecimentos = PropertyEntity({ type: types.map, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.ItensOrdensServico = PropertyEntity({ type: types.map, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });

    this.ImportarOrdemCompra = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Imp. Ordem de Compra", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true), icon: ko.observable("fal fa-plus") });
    this.ImportarNFe = PropertyEntity({ eventClick: ImportarNFeClick, type: types.event, text: "Importar NF-e", visible: ko.observable(false), icon: ko.observable("fal fa-plus") });
    this.NFe = PropertyEntity({ type: types.file, eventChange: EnviarNFe, codEntity: ko.observable(0), text: "NF-e:", val: ko.observable(""), visible: ko.observable(false) });
    this.ImportarCTe = PropertyEntity({ eventClick: ImportarCTeClick, type: types.event, text: "Importar CT-e", visible: ko.observable(false), icon: ko.observable("fal fa-plus") });
    this.ImportarNFSeCuritiba = PropertyEntity({ eventClick: ImportarNFSeCuritibaClick, type: types.event, text: "Imp. NFS-e Curitiba", visible: ko.observable(true), icon: ko.observable("fal fa-plus") });
    this.NFSeCuritiba = PropertyEntity({ type: types.file, eventChange: EnviarNFSeCuritiba, codEntity: ko.observable(0), text: "NFS-e:", val: ko.observable(""), visible: ko.observable(false) });
    this.CTe = PropertyEntity({ type: types.file, eventChange: EnviarCTe, codEntity: ko.observable(0), text: "CT-e:", val: ko.observable(""), visible: ko.observable(false) });

    this.TipoEmissao = PropertyEntity({ val: ko.observable(0), def: 0, visible: ko.observable(false) }); //Usado apenas para buscar natureza tipo entrada

    this.Motivo = PropertyEntity({ text: "*Motivo:", val: ko.observable(""), def: "", maxlength: 5000, enable: ko.observable(false), visible: ko.observable(false), required: ko.observable(false) });
    this.DocumentoImportadoXML = PropertyEntity({ val: ko.observable(""), def: "" });

    this.MoedaCotacaoBancoCentral.val.subscribe(function () {
        CalcularMoedaEstrangeira();
    });

    this.DataBaseCRT.val.subscribe(function () {
        CalcularMoedaEstrangeira();
    });

    this.Situacao.val.subscribe(function (novoValor) {
        _documentoEntrada.ChaveNotaAnulacao.required(false);
        _documentoEntrada.ChaveNotaAnulacao.visible(false);

        _documentoEntrada.Motivo.enable(false);
        _documentoEntrada.Motivo.visible(false);
        _documentoEntrada.Motivo.required(false);
        _documentoEntrada.Motivo.val("");

        if (novoValor === EnumSituacaoDocumentoEntrada.Anulado) {
            _documentoEntrada.ChaveNotaAnulacao.required(true);
            _documentoEntrada.ChaveNotaAnulacao.visible(true);

            _documentoEntrada.Motivo.enable(true);
            _documentoEntrada.Motivo.visible(true);
            _documentoEntrada.Motivo.required(true);
        }

        if (novoValor === EnumSituacaoDocumentoEntrada.Cancelado) {
            _documentoEntrada.Motivo.enable(true);
            _documentoEntrada.Motivo.visible(true);
            _documentoEntrada.Motivo.required(true);
        }
    });

    this.OrdemServico.codEntity.subscribe(function () {
        ControleVisibilidadeOrdemServico();
    });
};

var CRUDDocumentoEntrada = function () {
    this.Adicionar = PropertyEntity({ eventClick: salvarDocumentoEntrada, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: salvarDocumentoEntrada, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
    this.ReverterFaturamento = PropertyEntity({ eventClick: ReverterFaturamentoClick, type: types.event, text: "Reverter Faturamento", visible: ko.observable(false) });
};

//*******EVENTOS*******

function LoadDocumentoEntrada() {

    _documentoEntrada = new DocumentoEntrada();
    KoBindings(_documentoEntrada, "knockoutDetalhes");

    HeaderAuditoria("DocumentoEntradaTMS", _documentoEntrada);

    _crudDocumentoEntrada = new CRUDDocumentoEntrada();
    KoBindings(_crudDocumentoEntrada, "knockoutCRUDDocumentoEntrada");

    _pesquisaDocumentoEntrada = new PesquisaDocumentoEntrada();
    KoBindings(_pesquisaDocumentoEntrada, "knockoutPesquisaDocumentoEntrada", false, _pesquisaDocumentoEntrada.Pesquisar.id);

    new BuscarClientes(_documentoEntrada.Fornecedor, RetornoFornecedor, true);
    new BuscarEmpresa(_documentoEntrada.Destinatario);
    new BuscarVeiculos(_documentoEntrada.Veiculo, RetornoBuscarVeiculos);
    new BuscarEquipamentos(_documentoEntrada.Equipamento, RetornoEquipamento, null, _documentoEntrada.Veiculo);
    new BuscarModeloDocumentoFiscal(_documentoEntrada.Modelo, RetornoModeloDocumentoFiscal);
    new BuscarEspecieDocumentoFiscal(_documentoEntrada.Especie);
    new BuscarClientes(_documentoEntrada.Expedidor, null, true);
    new BuscarClientes(_documentoEntrada.Recebedor, null, true);
    new BuscarLocalidades(_documentoEntrada.LocalidadeInicioPrestacao);
    new BuscarLocalidades(_documentoEntrada.LocalidadeTerminoPrestacao);
    new BuscarContratoFinanciamento(_documentoEntrada.ContratoFinanciamento, RetornoContratoFinanciamento);
    new BuscarSituacaoLancamentoDocumentoEntrada(_documentoEntrada.StatusLancamento);//, RetornoSituacaoStatusLancamento);
    new BuscarSituacaoLancamentoDocumentoEntrada(_pesquisaDocumentoEntrada.PesquisaStatusLancamento);//, RetornoSituacaoStatusLancamento);


    if (_CONFIGURACAO_TMS.PermiteSelecionarQualquerNaturezaNFEntrada)
        new BuscarNaturezasOperacoesNotaFiscal(_documentoEntrada.NaturezaOperacao, null, null, RetornoConsultaNaturezaOperacao);
    else
        new BuscarNaturezasOperacoesNotaFiscal(_documentoEntrada.NaturezaOperacao, null, null, RetornoConsultaNaturezaOperacao, _documentoEntrada.Fornecedor, _documentoEntrada.Destinatario, null, null, _documentoEntrada.TipoEmissao, _documentoEntrada.LocalidadeTerminoPrestacao);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiNFe)
        _buscaCFOP = new BuscarCFOPNotaFiscal(_documentoEntrada.CFOP, function (r) { RetornoConsultaCFOP(r.Codigo); }, EnumTipoCFOP.Entrada, null, _documentoEntrada.Fornecedor, null, _documentoEntrada.Destinatario, null, null, _documentoEntrada.NaturezaOperacao, _documentoEntrada.LocalidadeInicioPrestacao, _documentoEntrada.LocalidadeTerminoPrestacao);
    else
        _buscaCFOP = new BuscarCFOPNotaFiscal(_documentoEntrada.CFOP, function (r) { RetornoConsultaCFOP(r.Codigo); }, EnumTipoCFOP.Entrada, null, _documentoEntrada.Fornecedor, null, _documentoEntrada.Destinatario, null, null, null);

    new BuscarOrdemCompra(_documentoEntrada.OrdemCompra, RetornoConsultaOrdemCompra, null, EnumSituacaoOrdemCompra.Aprovada);
    new BuscarOrdemCompra(_documentoEntrada.ImportarOrdemCompra, RetornoImportarOrdemCompra, null, EnumSituacaoOrdemCompra.Aprovada);
    new BuscarOrdemServico(_documentoEntrada.OrdemServico, RetornoConsultaOrdemServico, null, [EnumSituacaoOrdemServicoFrota.EmManutencao, EnumSituacaoOrdemServicoFrota.AgNotaFiscal, _CONFIGURACAO_TMS.PermitirSelecionarOSFinalizadaDocumentoEntrada ? EnumSituacaoOrdemServicoFrota.Finalizada : null]);
    new BuscarTipoMovimento(_documentoEntrada.TipoMovimento, null, null, RetornoConsultaTipoMovimento, null, EnumFinalidadeTipoMovimento.DocumentoEntrada);
    new BuscarServicoTMS(_documentoEntrada.Servico);
    new BuscarLocalidades(_documentoEntrada.LocalidadePrestacaoServico);
    new BuscarClientes(_pesquisaDocumentoEntrada.Fornecedor);
    new BuscarEmpresa(_pesquisaDocumentoEntrada.Destinatario);
    new BuscarVeiculos(_pesquisaDocumentoEntrada.Veiculo);
    new BuscarNaturezasOperacoesNotaFiscal(_pesquisaDocumentoEntrada.NaturezaOperacao);
    new BuscarCFOPNotaFiscal(_pesquisaDocumentoEntrada.CFOP, null, EnumTipoCFOP.Entrada);
    new BuscarTipoMovimento(_pesquisaDocumentoEntrada.TipoMovimento, null, null, null, null, EnumFinalidadeTipoMovimento.DocumentoEntrada);
    new BuscarProdutoTMS(_pesquisaDocumentoEntrada.Produto);
    new BuscarCategoriaPessoa(_pesquisaDocumentoEntrada.Categoria);

    LoadItem();
    LoadDuplicata();
    LoadGuia();
    LoadCentroCusto();
    LoadDuplicataGeracaoAutomatica();
    LayoutMultiNFe();
    LoadQualificacao();
    LoadVeiculo();
    LoadCentroResultadoTipoDespesa();
    LoadDocumentoEntradaIntegracoes();
    carregarDespesaVeiculo("conteudoDespesaVeiculo", BuscarDocumentosEntrada);
    $("#liTabGuias").hide();

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiNFe) {
        if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.DocumentoEntrada_LancarDuplicata, _PermissoesPersonalizadas)) {
            SetarEnableCamposKnockout(_duplicata, false);
            SetarEnableCamposKnockout(_duplicataGeracaoAutomatica, false);
        }

        if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.DocumentoEntrada_ImportarXML, _PermissoesPersonalizadas) || _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador) {
            _documentoEntrada.ImportarNFe.visible(true);
            _documentoEntrada.ImportarCTe.visible(true);
        }
    } else {
        _documentoEntrada.ImportarNFe.visible(true);
        _documentoEntrada.ImportarCTe.visible(true);
        _documentoEntrada.ImportarOrdemCompra.visible(true);
        _documentoEntrada.TipoMovimento.visible(false);
        _documentoEntrada.TipoMovimento.required(false);
        _documentoEntrada.KMAbastecimento.cssClass("col col-xs-12 col-sm-12 col-md-2 col-lg-2");
        _documentoEntrada.Equipamento.cssClass("col col-xs-12 col-sm-12 col-md-3 col-lg-3");
        _documentoEntrada.Horimetro.cssClass("col col-xs-12 col-sm-12 col-md-2 col-lg-2");

        if (_CONFIGURACAO_TMS.DeixarPadraoFinalizadoDocumentoEntrada) {
            _documentoEntrada.Situacao.val(EnumSituacaoDocumentoEntrada.Finalizado);
        }
    }
}

function RetornoContratoFinanciamento(data) {

    _documentoEntrada.ContratoFinanciamento.codEntity(data.Codigo);
    _documentoEntrada.ContratoFinanciamento.val(data.Numero);
}

function RetornoSituacaoStatusLancamento(data) {

    _pesquisaDocumentoEntrada.PesquisaStatusLancamento.codEntity(data.Codigo);
    _pesquisaDocumentoEntrada.PesquisaStatusLancamento.val(data.Descricao);
}

function RetornoFornecedor(data) {
    _documentoEntrada.Fornecedor.codEntity(data.Codigo);
    _documentoEntrada.Fornecedor.val(data.Descricao);
    _duplicata.FormaTitulo.val(data.FormaTituloFornecedor);
    ValidarDocumentoDuplicado();
}

function RetornoModeloDocumentoFiscal(data) {
    _documentoEntrada.Modelo.val(data.Descricao);
    _documentoEntrada.Modelo.codEntity(data.Codigo);

    if (data.DocumentoComMoedaEstrangeira) {
        _documentoEntrada.MoedaCotacaoBancoCentral.visible(true);
        _documentoEntrada.DataBaseCRT.visible(true);
        _documentoEntrada.ValorMoedaCotacao.visible(true);
        _documentoEntrada.MoedaCotacaoBancoCentral.val(data.MoedaCotacaoBancoCentral);
    }
    else {
        _documentoEntrada.MoedaCotacaoBancoCentral.visible(false);
        _documentoEntrada.DataBaseCRT.visible(false);
        _documentoEntrada.ValorMoedaCotacao.visible(false);
        _documentoEntrada.MoedaCotacaoBancoCentral.val(EnumMoedaCotacaoBancoCentral.Real);
    }

    ControleCamposPorModelo(data.Numero);
}

function ValidarDocumentoDuplicado() {
    var data = {
        Numero: _documentoEntrada.Numero.val().toString().replace('.', ''),
        Fornecedor: _documentoEntrada.Fornecedor.codEntity(),
        Codigo: _documentoEntrada.Codigo.val(),
        Serie: _documentoEntrada.Serie.val()
    };
    if (data.Numero > 0 && data.Fornecedor > 0) {
        executarReST("DocumentoEntrada/ValidarLancamentoDocumento", data, function (arg) {
            if (arg.Success) {
                if (arg.Data != true && arg.Msg != "") {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    }
}

function ImportarNFeClick(e, sender) {
    $("#" + _documentoEntrada.NFe.id).trigger("click");
}

function ImportarNFSeCuritibaClick(e, sender) {
    $("#" + _documentoEntrada.NFSeCuritiba.id).trigger("click");
}

function ImportarCTeClick(e, sender) {
    $("#" + _documentoEntrada.CTe.id).trigger("click");
}

function EnviarNFe(e, sender) {
    if (_documentoEntrada.NFe.val() != "") {
        var file = document.getElementById(_documentoEntrada.NFe.id);

        var formData = new FormData();
        formData.append("upload", file.files[0]);

        enviarArquivo("DocumentoEntrada/ObterDadosNFe", {}, formData, function (arg) {
            var fileControl = $("#" + _documentoEntrada.NFe.id);
            fileControl.replaceWith(fileControl = fileControl.clone(true));

            LimparCamposDocumentoEntrada();

            if (arg.Success) {
                if (arg.Data) {
                    PreencherObjetoKnout(_documentoEntrada, arg);
                    RecarregarGridDuplicata();
                    RecarregarGridVeiculo();
                    RecarregarGridGuia();
                    RecarregarGridCentroCusto();
                    RecarregarGridItem();
                    ControleCamposPorModelo("55");
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    }
}

function EnviarNFSeCuritiba(e, sender) {
    if (_documentoEntrada.NFSeCuritiba.val() != "") {
        var file = document.getElementById(_documentoEntrada.NFSeCuritiba.id);

        var formData = new FormData();
        formData.append("upload", file.files[0]);

        enviarArquivo("DocumentoEntrada/ObterDadosNFSeCuritiba", {}, formData, function (arg) {
            var fileControl = $("#" + _documentoEntrada.NFSeCuritiba.id);
            fileControl.replaceWith(fileControl = fileControl.clone(true));

            LimparCamposDocumentoEntrada();

            if (arg.Success) {
                if (arg.Data) {
                    PreencherObjetoKnout(_documentoEntrada, arg);
                    RecarregarGridDuplicata();
                    RecarregarGridVeiculo();
                    RecarregarGridGuia();
                    RecarregarGridCentroCusto();
                    RecarregarGridItem();
                    ControleCamposPorModelo("39");
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    }
}

function EnviarCTe(e, sender) {
    if (_documentoEntrada.CTe.val() != "") {
        var file = document.getElementById(_documentoEntrada.CTe.id);

        var formData = new FormData();
        formData.append("upload", file.files[0]);

        enviarArquivo("DocumentoEntrada/ObterDadosCTe", {}, formData, function (arg) {
            var fileControl = $("#" + _documentoEntrada.CTe.id);
            fileControl.replaceWith(fileControl = fileControl.clone(true));

            LimparCamposDocumentoEntrada();

            if (arg.Success) {
                if (arg.Data) {
                    console.log(_documentoEntrada.DocumentoImportadoXML);
                    PreencherObjetoKnout(_documentoEntrada, arg);
                    RecarregarGridDuplicata();
                    RecarregarGridVeiculo();
                    RecarregarGridGuia();
                    RecarregarGridCentroCusto();
                    RecarregarGridItem();
                    ControleCamposPorModelo("57");
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    }
}

function adicionarClick(e, sender) {
    PreencherListasSelecao();

    Salvar(_documentoEntrada, "DocumentoEntrada/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                var data = arg.Data;

                if (!string.IsNullOrWhiteSpace(data.CodigosTitulos))
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso.<br>Títulos gerados: " + data.CodigosTitulos, 16000);
                else
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso.");

                if (_CONFIGURACAO_TMS.AbrirRateioDespesaVeiculoAutomaticamente && _documentoEntrada.Situacao.val() === EnumSituacaoDocumentoEntrada.Finalizado && data.RealizarRateioDespesaVeiculo && data.RealizarRateioSomenteQuandoNaoTiverOS)
                    AbrirTelaReteioDespesaVeiculo(arg.Data);

                _gridDocumentoEntrada.CarregarGrid();

                if (data.MostrarMensagemOSNaoFinalizada)
                    exibirMensagem(tipoMensagem.aviso, "Aviso", "Ordem de Serviço não foi encerrada devido o valor da nota ser menor do que o valor orçado!", 16000);
                if (data.ExibirQualificaoFornecedor)
                    ExibirQualificaoFornecedor(_documentoEntrada.OrdemCompra.codEntity());

                LimparCamposDocumentoEntrada();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg, 16000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    PreencherListasSelecao();

    Salvar(_documentoEntrada, "DocumentoEntrada/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                var data = arg.Data;

                if (!string.IsNullOrWhiteSpace(data.CodigosTitulos))
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso.<br>Títulos gerados: " + data.CodigosTitulos, 16000);
                else
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso.");

                if (_CONFIGURACAO_TMS.AbrirRateioDespesaVeiculoAutomaticamente && _documentoEntrada.Situacao.val() === EnumSituacaoDocumentoEntrada.Finalizado && data.RealizarRateioDespesaVeiculo && data.RealizarRateioSomenteQuandoNaoTiverOS)
                    AbrirTelaReteioDespesaVeiculo(arg.Data);

                _gridDocumentoEntrada.CarregarGrid();

                if (data.MostrarMensagemOSNaoFinalizada)
                    exibirMensagem(tipoMensagem.aviso, "Aviso", "Ordem de Serviço não foi encerrada devido o valor da nota ser menor do que o valor orçado!", 16000);
                if (data.ExibirQualificaoFornecedor)
                    ExibirQualificaoFornecedor(_documentoEntrada.OrdemCompra.codEntity());

                LimparCamposDocumentoEntrada();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg, 16000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function salvarDocumentoEntrada(e, sender) {
    if (_documentoEntrada.Situacao.val() === EnumSituacaoDocumentoEntrada.Finalizado) {
        if (_documentoEntrada.Veiculo.codEntity() > 0 && (_documentoEntrada.KMAbastecimento.val() <= 0 || _documentoEntrada.KMAbastecimento.val() === "0" || _documentoEntrada.KMAbastecimento.val() === "" || _documentoEntrada.KMAbastecimento.val() === undefined)) {
            exibirConfirmacao("Confirmação", "Deseja finalizar a nota sem o KM informado?", function () {
                salvarClick();
            });
        } else
            salvarClick();
    } else
        salvarClick();
}

function salvarClick(e, sender) {
    var valido = ValidarDocumentoEntrada();

    if (valido) {
        if (_documentoEntrada.Situacao.val() == EnumSituacaoDocumentoEntrada.Finalizado) {
            var data = {
                TipoMovimento: _documentoEntrada.TipoMovimento.codEntity(),
                DataEmissao: _documentoEntrada.DataEmissao.val(),
                NaturezaOperacao: _documentoEntrada.NaturezaOperacao.codEntity(),
            };

            executarReST("PlanoOrcamentario/ValidaPlanoOrcamentarioEmpresa", data, function (arg) {
                if (arg.Success) {
                    if (arg.Data != true && arg.Data.Mensagem != "") {
                        if (arg.Data.TipoLancamentoFinanceiroSemOrcamento == EnumTipoLancamentoFinanceiroSemOrcamento.Avisar) {
                            exibirConfirmacao("Confirmação", arg.Data.Mensagem, function () {
                                if (_documentoEntrada.Codigo.val() > 0)
                                    atualizarClick(e, sender);
                                else
                                    adicionarClick(e, sender);
                            });
                        } else
                            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                    } else {
                        if (_documentoEntrada.Codigo.val() > 0)
                            atualizarClick(e, sender);
                        else
                            adicionarClick(e, sender);
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                }
            });
        } else {
            if (_documentoEntrada.Codigo.val() > 0)
                atualizarClick(e, sender);
            else
                adicionarClick(e, sender);
        }
    }
}

function ExcluirClick(e, sender) {
    exibirConfirmacao("Atenção!", "Deseja realmente excluir o documento de entrada " + _documentoEntrada.Numero.val() + "?", function () {
        ExcluirPorCodigo(_documentoEntrada, "DocumentoEntrada/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso.");
                    _gridDocumentoEntrada.CarregarGrid();
                    LimparCamposDocumentoEntrada();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }

        }, null);
    });
}

function ReverterFaturamentoClick(e, sender) {
    exibirConfirmacao("Atenção!", "Deseja realmente reverter o faturamento do documento de entrada " + _documentoEntrada.Numero.val() + "?", function () {
        executarReST("DocumentoEntrada/ReverterFaturamento", { Codigo: _documentoEntrada.Codigo.val() }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Faturamento revertido com sucesso.");
                    _gridDocumentoEntrada.CarregarGrid();
                    EditarDocumentoEntrada({ Codigo: _documentoEntrada.Codigo.val() });
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function CancelarClick(e) {
    LimparCamposDocumentoEntrada();
}

//*******MÉTODOS*******

function RetornoConsultaOrdemCompra(dados) {
    _documentoEntrada.OrdemCompra.val(dados.Numero);
    _documentoEntrada.OrdemCompra.codEntity(dados.Codigo);

    _item.OrdemCompra.val(dados.Numero);
    _item.OrdemCompra.codEntity(dados.Codigo);

    if (dados.CodigoVeiculo > 0) {
        _documentoEntrada.Veiculo.codEntity(dados.CodigoVeiculo);
        _documentoEntrada.Veiculo.val(dados.Veiculo);
        _item.Veiculo.codEntity(dados.CodigoVeiculo);
        _item.Veiculo.val(dados.Veiculo);

        for (var i = 0; i < _documentoEntrada.Itens.list.length; i++) {
            var item = _documentoEntrada.Itens.list[i];

            item.Veiculo.codEntity = _documentoEntrada.Veiculo.codEntity();
            item.Veiculo.val = _documentoEntrada.Veiculo.val();

            _documentoEntrada.Itens.list[i] = item;
        }

        RecarregarGridItem();
    }
}

function RetornoImportarOrdemCompra(dados) {
    LimparCamposDocumentoEntrada();
    _documentoEntrada.Codigo.val(dados.Codigo);

    BuscarPorCodigo(_documentoEntrada, "DocumentoEntrada/ImportarOrdemCompraPorCodigo", function (arg) {
        _pesquisaDocumentoEntrada.ExibirFiltros.visibleFade(false);
        _documentoEntrada.Codigo.val(guid);

        _documentoEntrada.ImportarNFe.visible(false);
        _documentoEntrada.ImportarCTe.visible(false);
        _documentoEntrada.ImportarNFSeCuritiba.visible(false);
        _documentoEntrada.ImportarOrdemCompra.visible(false);
        $("#liTabGuias").hide();

        if (_documentoEntrada.Veiculo.codEntity() > 0) {
            _item.Veiculo.codEntity(_documentoEntrada.Veiculo.codEntity());
            _item.Veiculo.val(_documentoEntrada.Veiculo.val());
        }

        RecarregarGridDuplicata();
        RecarregarGridVeiculo();
        RecarregarGridGuia();
        RecarregarGridCentroCusto();
        RecarregarGridItem();
    }, null);
}

function RetornoConsultaOrdemServico(dados) {
    var descricaoOrdemServico = dados.Numero + `(${dados.VeiculoEquipamento}) ${dados.TipoOrdemServico}`;

    _documentoEntrada.OrdemServico.val(descricaoOrdemServico);
    _documentoEntrada.OrdemServico.codEntity(dados.Codigo);
    _documentoEntrada.Veiculo.codEntity(dados.CodigoVeiculo);
    _documentoEntrada.Veiculo.val(dados.Veiculo);
    if (dados.QuilometragemVeiculo != "0")
        _documentoEntrada.KMAbastecimento.val(dados.QuilometragemVeiculo);
    _documentoEntrada.Equipamento.codEntity(dados.CodigoEquipamento);
    _documentoEntrada.Equipamento.val(dados.Equipamento);

    _item.OrdemServico.codEntity(dados.Codigo);
    _item.OrdemServico.val(descricaoOrdemServico);
    _item.Veiculo.codEntity(_documentoEntrada.Veiculo.codEntity());
    _item.Veiculo.val(_documentoEntrada.Veiculo.val());
    _item.KMAbastecimento.val(_documentoEntrada.KMAbastecimento.val());
    _item.Equipamento.codEntity(_documentoEntrada.Equipamento.codEntity());
    _item.Equipamento.val(_documentoEntrada.Equipamento.val());
    _item.CentroResultado.codEntity(dados.CodigoCentroResultado);
    _item.CentroResultado.val(dados.CentroResultado);

    for (var i = 0; i < _documentoEntrada.Itens.list.length; i++) {
        var item = _documentoEntrada.Itens.list[i];

        item.OrdemServico.codEntity = dados.Codigo;
        item.OrdemServico.val = descricaoOrdemServico;

        item.Veiculo.codEntity = _documentoEntrada.Veiculo.codEntity();
        item.Veiculo.val = _documentoEntrada.Veiculo.val();
        item.KMAbastecimento.val = _documentoEntrada.KMAbastecimento.val();
        item.Equipamento.codEntity = _documentoEntrada.Equipamento.codEntity();
        item.Equipamento.val = _documentoEntrada.Equipamento.val();

        item.CentroResultado.codEntity = dados.CodigoCentroResultado;
        item.CentroResultado.val = dados.CentroResultado;

        _documentoEntrada.Itens.list[i] = item;
    }

    RecarregarGridItem();
}

function RetornoEquipamento(dados) {
    _documentoEntrada.Equipamento.codEntity(dados.Codigo);
    _documentoEntrada.Equipamento.val(dados.DescricaoComMarcaModelo);
    _item.Equipamento.codEntity(dados.Codigo);
    _item.Equipamento.val(dados.DescricaoComMarcaModelo);
    ReplicarHorimetroAbastecimento();
}

function ReplicarHorimetroAbastecimento() {
    if (_documentoEntrada.Equipamento.codEntity() > 0 && Boolean(_documentoEntrada.Horimetro.val()) && Globalize.parseInt(_documentoEntrada.Horimetro.val()) > 0) {
        _item.Horimetro.val(_documentoEntrada.Horimetro.val());
        if (_documentoEntrada.Itens.list.length > 0) {
            for (var i = 0; i < _documentoEntrada.Itens.list.length; i++) {
                var item = _documentoEntrada.Itens.list[i];

                item.Equipamento.codEntity = _documentoEntrada.Equipamento.codEntity();
                item.Equipamento.val = _documentoEntrada.Equipamento.val();
                item.Horimetro.val = _documentoEntrada.Horimetro.val();

                _documentoEntrada.Itens.list[i] = item;
            }
        }
    }
}

function RetornoBuscarVeiculos(dados) {
    _documentoEntrada.Veiculo.codEntity(dados.Codigo);
    _documentoEntrada.Veiculo.val(dados.DescricaoComMarcaModelo);

    _item.Veiculo.codEntity(dados.Codigo);
    _item.Veiculo.val(dados.DescricaoComMarcaModelo);
    _item.CentroResultado.codEntity(dados.CodigoCentroResultado);
    _item.CentroResultado.val(dados.CentroResultado);

    ReplicarKMAbastecimento();
}

function ReplicarKMAbastecimento() {
    var kmAbastecimento = _documentoEntrada.KMAbastecimento.val();

    if (_documentoEntrada.Veiculo.codEntity() > 0 && Boolean(kmAbastecimento) && Globalize.parseInt(kmAbastecimento) > 0) {
        _item.KMAbastecimento.val(kmAbastecimento);
        if (_documentoEntrada.Itens.list.length > 0) {
            for (var i = 0; i < _documentoEntrada.Itens.list.length; i++) {
                var item = _documentoEntrada.Itens.list[i];

                item.Veiculo.codEntity = _documentoEntrada.Veiculo.codEntity();
                item.Veiculo.val = _documentoEntrada.Veiculo.val();
                item.KMAbastecimento.val = kmAbastecimento;
                item.DataAbastecimento.val = _documentoEntrada.DataAbastecimento.val();

                item.CentroResultado.codEntity = _item.CentroResultado.codEntity();
                item.CentroResultado.val = _item.CentroResultado.val();

                _documentoEntrada.Itens.list[i] = item;
            }

            RecarregarGridItem();
        }
    }
}

function ReplicarVeiculoAosItens() {
    var kmAbastecimento = _documentoEntrada.KMAbastecimento.val();

    if (_documentoEntrada.Veiculo.codEntity() > 0 || (Boolean(kmAbastecimento) && Globalize.parseInt(kmAbastecimento) > 0) || _documentoEntrada.Equipamento.codEntity() > 0) {

        if (_documentoEntrada.Itens.list.length > 0) {
            for (var i = 0; i < _documentoEntrada.Itens.list.length; i++) {
                var item = _documentoEntrada.Itens.list[i];

                if ((_documentoEntrada.Veiculo.codEntity() > 0) && (item.Veiculo.codEntity == null || item.Veiculo.codEntity == undefined || item.Veiculo.codEntity == "" || item.Veiculo.codEntity <= 0 || item.Veiculo.codEntity == "0")) {
                    item.Veiculo.codEntity = _documentoEntrada.Veiculo.codEntity();
                    item.Veiculo.val = _documentoEntrada.Veiculo.val();
                }

                if ((_documentoEntrada.Equipamento.codEntity() > 0) && (item.Equipamento.codEntity == null || item.Equipamento.codEntity == undefined || item.Equipamento.codEntity == "" || item.Equipamento.codEntity <= 0 || item.Equipamento.codEntity == "0")) {
                    item.Equipamento.codEntity = _documentoEntrada.Equipamento.codEntity();
                    item.Equipamento.val = _documentoEntrada.Equipamento.val();
                }

                if ((Boolean(_documentoEntrada.Horimetro.val()) && Globalize.parseInt(_documentoEntrada.Horimetro.val()) > 0) && (!Boolean(item.Horimetro.val) || item.Horimetro.val <= 0))
                    item.Horimetro.val = _documentoEntrada.Horimetro.val();

                if ((Boolean(kmAbastecimento) && Globalize.parseInt(kmAbastecimento) > 0) && (!Boolean(item.KMAbastecimento.val) || item.KMAbastecimento.val <= 0))
                    item.KMAbastecimento.val = kmAbastecimento;

                if ((_documentoEntrada.DataAbastecimento.val() != "") && (item.DataAbastecimento.val == null || item.DataAbastecimento.val == undefined || item.DataAbastecimento.val == ""))
                    item.DataAbastecimento.val = _documentoEntrada.DataAbastecimento.val();

                _documentoEntrada.Itens.list[i] = item;
            }

            RecarregarGridItem();
        }
    }
}

function RetornoConsultaNaturezaOperacao(dados) {

    _documentoEntrada.NaturezaOperacao.codEntity(dados.Codigo);
    _documentoEntrada.NaturezaOperacao.val(dados.Descricao);
    _item.NaturezaOperacaoNF.codEntity(dados.Codigo);
    _item.NaturezaOperacaoNF.val(dados.Descricao);
    _item.NaturezaOperacao.codEntity(dados.Codigo);
    _item.NaturezaOperacao.val(dados.Descricao);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiNFe) {
        LimparCampoEntity(_documentoEntrada.CFOP);
        LimparCampoEntity(_item.CFOPNF);
    }

    if (_documentoEntrada.Itens.list.length > 0) {
        for (var i = 0; i < _documentoEntrada.Itens.list.length; i++) {
            var item = _documentoEntrada.Itens.list[i];

            item.NaturezaOperacao.codEntity = dados.Codigo;
            item.NaturezaOperacao.val = dados.Descricao;

            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiNFe) {
                item.CFOP.codEntity = 0;
                item.CFOP.val = "";
            }

            _documentoEntrada.Itens.list[i] = item;
        }
    }

    ObterDetalhesNaturezaOperacao(dados);
}

function ObterDetalhesNaturezaOperacao(dados) {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiNFe)
        return;

    executarReST("NaturezaDaOperacao/ObterDetalhes", { Codigo: dados.Codigo }, function (r) {
        if (r.Success) {
            if (r.Data) {
                var data = r.Data;
                if (data.CodigoPrimeiraCFOP > 0)
                    RetornoConsultaCFOP(data.CodigoPrimeiraCFOP);

                TrocaComponenteModalBuscaCFOPDocumentoEntrada(data.QuantidadeCFOPs);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function RetornoConsultaTipoMovimento(dados) {
    _documentoEntrada.TipoMovimento.codEntity(dados.Codigo);
    _documentoEntrada.TipoMovimento.val(dados.Descricao);
    _item.TipoMovimentoNF.codEntity(dados.Codigo);
    _item.TipoMovimentoNF.val(dados.Descricao);
    _item.TipoMovimento.codEntity(dados.Codigo);
    _item.TipoMovimento.val(dados.Descricao);

    if (_documentoEntrada.Itens.list.length > 0) {
        for (var i = 0; i < _documentoEntrada.Itens.list.length; i++) {
            var item = _documentoEntrada.Itens.list[i];

            item.TipoMovimento.codEntity = dados.Codigo;
            item.TipoMovimento.val = dados.Descricao;

            _documentoEntrada.Itens.list[i] = item;
        }
    }
}

function RetornoConsultaCFOP(codigo) {

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiNFe) {
        if (_documentoEntrada.NaturezaOperacao.codEntity() <= 0) {
            exibirMensagem(tipoMensagem.atencao, "Atenção", "É necessário selecionar a natureza de operação antes de selecionar a CFOP.");
            return;
        }

        if (_documentoEntrada.Fornecedor.codEntity() <= 0) {
            exibirMensagem(tipoMensagem.atencao, "Atenção", "É necessário selecionar o fornecedor antes de selecionar a CFOP.");
            return;
        }

        if (_documentoEntrada.Destinatario.codEntity() <= 0) {
            exibirMensagem(tipoMensagem.atencao, "Atenção", "É necessário selecionar o destinatário antes de selecionar a CFOP.");
            return;
        }
    }

    executarReST("CFOPNotaFiscal/BuscarPorCodigo", { Codigo: codigo }, function (r) {
        if (r.Success) {
            if (r.Data) {
                executarReST("DocumentoEntrada/ObterDetalhesParticipantes", { Fornecedor: _documentoEntrada.Fornecedor.codEntity(), Destinatario: _documentoEntrada.Destinatario.codEntity() }, function (rp) {
                    if (rp.Success) {
                        if (rp.Data) {
                            var data = r.Data;
                            _documentoEntrada.CFOP.codEntity(data.Codigo);
                            _item.CFOPNF.codEntity(data.Codigo);
                            _item.CFOP.codEntity(data.Codigo);
                            _item.CfopFornecedor.codEntity(data.Codigo);

                            if (!string.IsNullOrWhiteSpace(r.Data.Extensao)) {
                                _documentoEntrada.CFOP.val(r.Data.CodigoCFOP + "." + r.Data.Extensao + " - " + (r.Data.Descricao || ''));
                                _item.CFOPNF.val(r.Data.CodigoCFOP + "." + r.Data.Extensao + " - " + (r.Data.Descricao || ''));
                                _item.CFOP.val(r.Data.CodigoCFOP + "." + r.Data.Extensao + " - " + (r.Data.Descricao || ''));
                                _item.CfopFornecedor.val(r.Data.CodigoCFOP + "." + r.Data.Extensao + " - " + (r.Data.Descricao || ''));
                            }
                            else {
                                _documentoEntrada.CFOP.val(r.Data.CodigoCFOP + " - " + (r.Data.Descricao || ''));
                                _item.CFOPNF.val(r.Data.CodigoCFOP + " - " + (r.Data.Descricao || ''));
                                _item.CFOP.val(r.Data.CodigoCFOP + " - " + (r.Data.Descricao || ''));
                                _item.CfopFornecedor.val(r.Data.CodigoCFOP + " - " + (r.Data.Descricao || ''));
                            }

                            _documentoEntrada.CFOP.entityDescription(_documentoEntrada.CFOP.val());

                            if (r.Data.TipoMovimentoUso.Codigo > 0) {
                                _documentoEntrada.TipoMovimento.codEntity(r.Data.TipoMovimentoUso.Codigo);
                                _documentoEntrada.TipoMovimento.val(r.Data.TipoMovimentoUso.Descricao);
                                _item.TipoMovimentoNF.codEntity(r.Data.TipoMovimentoUso.Codigo);
                                _item.TipoMovimentoNF.val(r.Data.TipoMovimentoUso.Descricao);
                                _item.TipoMovimento.codEntity(r.Data.TipoMovimentoUso.Codigo);
                                _item.TipoMovimento.val(r.Data.TipoMovimentoUso.Descricao);
                            }

                            if (_documentoEntrada.Itens.list.length > 0) {
                                for (var i = 0; i < _documentoEntrada.Itens.list.length; i++) {
                                    var item = _documentoEntrada.Itens.list[i];

                                    var baseCalculoImposto = 0;
                                    var valorTotal = Globalize.parseFloat(item.ValorTotal.val);
                                    var desconto = Globalize.parseFloat(item.Desconto.val);
                                    var outrasDespesas = Globalize.parseFloat(item.ValorOutrasDespesas.val);
                                    var valorFrete = Globalize.parseFloat(item.ValorFrete.val);
                                    var valorSeguro = Globalize.parseFloat(item.ValorSeguro.val);

                                    if (isNaN(valorTotal))
                                        valorTotal = 0;
                                    if (isNaN(desconto))
                                        desconto = 0;
                                    if (isNaN(outrasDespesas))
                                        outrasDespesas = 0;
                                    if (isNaN(valorFrete))
                                        valorFrete = 0;
                                    if (isNaN(valorSeguro))
                                        valorSeguro = 0;

                                    baseCalculoImposto = valorTotal - desconto + outrasDespesas + valorFrete + valorSeguro;

                                    item.CFOP.codEntity = r.Data.Codigo;
                                    if (!string.IsNullOrWhiteSpace(r.Data.Extensao))
                                        item.CFOP.val = r.Data.CodigoCFOP + "." + r.Data.Extensao + " - " + r.Data.Descricao;
                                    else
                                        item.CFOP.val = r.Data.CodigoCFOP + " - " + r.Data.Descricao;

                                    if (r.Data.TipoMovimentoUso.Codigo > 0) {
                                        item.TipoMovimento.codEntity = r.Data.TipoMovimentoUso.Codigo;
                                        item.TipoMovimento.val = r.Data.TipoMovimentoUso.Descricao;
                                    }

                                    //Aba PIS
                                    item.CSTPIS.val = ObterCSTPISCOFINS(r.Data.CSTPIS);

                                    if (r.Data.AliquotaPIS > 0)
                                        item.BaseCalculoPIS.val = Globalize.format(baseCalculoImposto, "n2");
                                    else
                                        item.BaseCalculoPIS.val = "0,00";

                                    item.PercentualReducaoBaseCalculoPIS.val = Globalize.format(r.Data.ReducaoBCPIS, "n2");
                                    item.AliquotaPIS.val = Globalize.format(r.Data.AliquotaPIS, "n2");
                                    item.ValorPIS.val = Globalize.format(CalcularImposto(item.BaseCalculoPIS.val, item.AliquotaPIS.val, item.PercentualReducaoBaseCalculoPIS.val), "n2");

                                    //Aba COFINS
                                    item.CSTCOFINS.val = ObterCSTPISCOFINS(r.Data.CSTCOFINS);

                                    if (r.Data.AliquotaCOFINS > 0)
                                        item.BaseCalculoCOFINS.val = Globalize.format(baseCalculoImposto, "n2");
                                    else
                                        item.BaseCalculoCOFINS.val = "0,00";

                                    item.PercentualReducaoBaseCalculoCOFINS.val = Globalize.format(r.Data.ReducaoBCCOFINS, "n2");
                                    item.AliquotaCOFINS.val = Globalize.format(r.Data.AliquotaCOFINS, "n2");
                                    item.ValorCOFINS.val = Globalize.format(CalcularImposto(item.BaseCalculoCOFINS.val, item.AliquotaCOFINS.val, item.PercentualReducaoBaseCalculoCOFINS.val), "n2");

                                    //Aba IPI
                                    item.CSTIPI.val = ObterCSTIPI(r.Data.CSTIPI);

                                    if (r.Data.AliquotaIPI > 0)
                                        item.BaseCalculoIPI.val = Globalize.format(baseCalculoImposto, "n2");
                                    else
                                        item.BaseCalculoIPI.val = "0,00";

                                    item.PercentualReducaoBaseCalculoIPI.val = Globalize.format(r.Data.ReducaoBCIPI, "n2");
                                    item.AliquotaIPI.val = Globalize.format(r.Data.AliquotaIPI, "n2");
                                    item.ValorIPI.val = Globalize.format(CalcularImposto(item.BaseCalculoIPI.val, item.AliquotaIPI.val, item.PercentualReducaoBaseCalculoIPI.val), "n2");

                                    //Aba ICMS
                                    item.CSTICMS.val = ObterCSTICMS(r.Data.CSTICMS);

                                    var aliquotaICMS = 0;
                                    if (rp.Data.Interestadual)
                                        aliquotaICMS = r.Data.AliquotaInterestadual;
                                    else
                                        aliquotaICMS = r.Data.AliquotaInterna;

                                    if (aliquotaICMS > 0)
                                        item.BaseCalculoICMS.val = Globalize.format(baseCalculoImposto, "n2");
                                    else
                                        item.BaseCalculoICMS.val = "0,00";

                                    item.AliquotaICMS.val = Globalize.format(aliquotaICMS, "n4");
                                    item.ValorICMS.val = Globalize.format(CalcularImposto(item.BaseCalculoICMS.val, item.AliquotaICMS.val, 0), "n2");

                                    //Aba Credito Presumido
                                    item.AliquotaCreditoPresumido.val = Globalize.format(r.Data.AliquotaParaCredito, "n2");

                                    if (r.Data.AliquotaParaCredito > 0)
                                        item.BaseCalculoCreditoPresumido.val = Globalize.format(baseCalculoImposto, "n2");
                                    else
                                        item.BaseCalculoCreditoPresumido.val = "0,00";

                                    item.ValorCreditoPresumido.val = Globalize.format(CalcularImposto(item.BaseCalculoCreditoPresumido.val, item.AliquotaCreditoPresumido.val, 0), "n2");


                                    //Aba Diferencial
                                    item.AliquotaDiferencial.val = Globalize.format(r.Data.AliquotaDiferencial, "n2");

                                    if (r.Data.AliquotaDiferencial > 0)
                                        item.BaseCalculoDiferencial.val = Globalize.format(baseCalculoImposto, "n2");
                                    else
                                        item.BaseCalculoDiferencial.val = "0,00";

                                    item.ValorDiferencial.val = Globalize.format(CalcularImposto(item.BaseCalculoDiferencial.val, item.AliquotaDiferencial.val, 0), "n2");

                                    //Cálculos de Retenções
                                    if (data.AliquotaRetencaoPIS > 0)
                                        item.ValorRetencaoPIS.val = Globalize.format(CalcularImposto(baseCalculoImposto, Globalize.format(data.AliquotaRetencaoPIS, "n2"), 0), "n2");
                                    else
                                        item.ValorRetencaoPIS.val = "0,00";

                                    if (data.AliquotaRetencaoCOFINS > 0)
                                        item.ValorRetencaoCOFINS.val = Globalize.format(CalcularImposto(baseCalculoImposto, Globalize.format(data.AliquotaRetencaoCOFINS, "n2"), 0), "n2");
                                    else
                                        item.ValorRetencaoCOFINS.val = "0,00";

                                    if (data.AliquotaRetencaoINSS > 0)
                                        item.ValorRetencaoINSS.val = Globalize.format(CalcularImposto(baseCalculoImposto, Globalize.format(data.AliquotaRetencaoINSS, "n2"), 0), "n2");
                                    else
                                        item.ValorRetencaoINSS.val = "0,00";

                                    if (data.AliquotaRetencaoIPI > 0)
                                        item.ValorRetencaoIPI.val = Globalize.format(CalcularImposto(baseCalculoImposto, Globalize.format(data.AliquotaRetencaoIPI, "n2"), 0), "n2");
                                    else
                                        item.ValorRetencaoIPI.val = "0,00";

                                    if (data.AliquotaRetencaoCSLL > 0)
                                        item.ValorRetencaoCSLL.val = Globalize.format(CalcularImposto(baseCalculoImposto, Globalize.format(data.AliquotaRetencaoCSLL, "n2"), 0), "n2");
                                    else
                                        item.ValorRetencaoCSLL.val = "0,00";

                                    if (data.AliquotaRetencaoOutras > 0)
                                        item.ValorRetencaoOutras.val = Globalize.format(CalcularImposto(baseCalculoImposto, Globalize.format(data.AliquotaRetencaoOutras, "n2"), 0), "n2");
                                    else
                                        item.ValorRetencaoOutras.val = "0,00";

                                    if (data.AliquotaRetencaoIR > 0)
                                        item.ValorRetencaoIR.val = Globalize.format(CalcularImposto(baseCalculoImposto, Globalize.format(data.AliquotaRetencaoIR, "n2"), 0), "n2");
                                    else
                                        item.ValorRetencaoIR.val = "0,00";

                                    if (data.AliquotaRetencaoISS > 0)
                                        item.ValorRetencaoISS.val = Globalize.format(CalcularImposto(baseCalculoImposto, Globalize.format(data.AliquotaRetencaoISS, "n2"), 0), "n2");
                                    else
                                        item.ValorRetencaoISS.val = "0,00";

                                    //Regras iguais as da importação de xml
                                    if (data.CreditoSobreTotalParaItensSujeitosICMSST) {

                                        item.BaseCalculoDiferencial.val = item.BaseCalculoICMS.val;
                                        item.AliquotaDiferencial.val = item.AliquotaICMS.val;
                                        item.ValorDiferencial.val = item.ValorICMS.val;

                                        item.BaseCalculoICMS.val = "0,00";
                                        item.AliquotaICMS.val = "0,0000";
                                        item.ValorICMS.val = "0,00";

                                        item.BaseSTRetido.val = "0,00";
                                        item.ValorSTRetido.val = "0,00";
                                    }

                                    _documentoEntrada.Itens.list[i] = item;
                                }

                                RecarregarGridItem();
                                AtualizarTotaisImpostos();
                            }
                        } else {
                            exibirMensagem(tipoMensagem.atencao, "Atenção", rp.Msg);
                        }
                    } else {
                        exibirMensagem(tipoMensagem.falha, "Falha", rp.Msg);
                    }
                });
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function ValidarDocumentoEntrada() {

    if (_documentoEntrada.Situacao.val() === EnumSituacaoDocumentoEntrada.Finalizado) {
        ReplicarVeiculoAosItens();

        var totalCentroCusto = 0,
            totalItens = 0,
            totalICMSItens = 0,
            totalPISItens = 0,
            totalCOFINSItens = 0,
            totalDescontoItens = 0,
            totalOutrasDespesasItens = 0,
            totalSeguroItens = 0,
            totalFreteItens = 0,
            totalICMSSTItens = 0,
            totalIPIItens = 0,
            baseCalculoICMSItens = 0,
            baseCalculoICMSSTItens = 0,
            totalRetencaoCOFINSItens = 0,
            totalRetencaoCSLLItens = 0,
            totalRetencaoINSSItens = 0,
            totalRetencaoIPIItens = 0,
            totalRetencaoOutrasItens = 0,
            totalRetencaoISSItens = 0,
            totalRetencaoIRItens = 0,
            totalRetencaoPISItens = 0,
            totalDocumento = Globalize.parseFloat(_documentoEntrada.ValorTotal.val()),
            totalICMSDocumento = Globalize.parseFloat(_documentoEntrada.ValorTotalICMS.val()),
            totalPISDocumento = Globalize.parseFloat(_documentoEntrada.ValorTotalPIS.val()),
            totalCOFINSDocumento = Globalize.parseFloat(_documentoEntrada.ValorTotalCOFINS.val()),
            totalDescontoDocumento = Globalize.parseFloat(_documentoEntrada.ValorTotalDesconto.val()),
            totalOutrasDespesasDocumento = Globalize.parseFloat(_documentoEntrada.ValorTotalOutrasDespesas.val()),
            totalFreteDocumento = Globalize.parseFloat(_documentoEntrada.ValorTotalFrete.val()),
            totalICMSSTDocumento = Globalize.parseFloat(_documentoEntrada.ValorTotalICMSST.val()),
            totalIPIDocumento = Globalize.parseFloat(_documentoEntrada.ValorTotalIPI.val()),
            baseCalculoICMSSTDocumento = Globalize.parseFloat(_documentoEntrada.BaseCalculoICMSST.val()),
            baseCalculoICMSDocumento = Globalize.parseFloat(_documentoEntrada.BaseCalculoICMS.val()),
            totalRetencaoCOFINSDocumento = Globalize.parseFloat(_documentoEntrada.ValorTotalRetencaoCOFINS.val()),
            totalRetencaoCSLLDocumento = Globalize.parseFloat(_documentoEntrada.ValorTotalRetencaoCSLL.val()),
            totalRetencaoINSSDocumento = Globalize.parseFloat(_documentoEntrada.ValorTotalRetencaoINSS.val()),
            totalRetencaoIPIDocumento = Globalize.parseFloat(_documentoEntrada.ValorTotalRetencaoIPI.val()),
            totalRetencaoOutrasDocumento = Globalize.parseFloat(_documentoEntrada.ValorTotalRetencaoOutras.val()),
            totalRetencaoISSDocumento = Globalize.parseFloat(_documentoEntrada.ValorTotalRetencaoISS.val()),
            totalRetencaoIRDocumento = Globalize.parseFloat(_documentoEntrada.ValorTotalRetencaoIR.val()),
            totalRetencaoPISDocumento = Globalize.parseFloat(_documentoEntrada.ValorTotalRetencaoPIS.val());

        var qtdProdutosVinculados = 0;
        var qtdTotalItens = _documentoEntrada.Itens.list.length;

        for (var i = 0; i < qtdTotalItens; i++) {
            var item = _documentoEntrada.Itens.list[i];

            totalItens += Globalize.parseFloat(item.ValorTotal.val);
            totalICMSItens += Globalize.parseFloat(item.ValorICMS.val);
            totalPISItens += Globalize.parseFloat(item.ValorPIS.val);
            totalCOFINSItens += Globalize.parseFloat(item.ValorCOFINS.val);
            totalDescontoItens += Globalize.parseFloat(item.Desconto.val);
            totalOutrasDespesasItens += Globalize.parseFloat(item.ValorOutrasDespesas.val);
            totalSeguroItens += Globalize.parseFloat(item.ValorSeguro.val);
            totalFreteItens += Globalize.parseFloat(item.ValorFrete.val);
            totalICMSSTItens += Globalize.parseFloat(item.ValorICMSST.val);
            totalIPIItens += Globalize.parseFloat(item.ValorIPI.val);
            baseCalculoICMSItens += Globalize.parseFloat(item.BaseCalculoICMS.val);
            baseCalculoICMSSTItens += Globalize.parseFloat(item.BaseCalculoICMSST.val);
            totalRetencaoCOFINSItens += Globalize.parseFloat(item.ValorRetencaoCOFINS.val);
            totalRetencaoCSLLItens += Globalize.parseFloat(item.ValorRetencaoCSLL.val);
            totalRetencaoINSSItens += Globalize.parseFloat(item.ValorRetencaoINSS.val);
            totalRetencaoIPIItens += Globalize.parseFloat(item.ValorRetencaoIPI.val);
            totalRetencaoOutrasItens += Globalize.parseFloat(item.ValorRetencaoOutras.val);
            totalRetencaoIRItens += Globalize.parseFloat(item.ValorRetencaoIR.val);
            totalRetencaoISSItens += Globalize.parseFloat(item.ValorRetencaoISS.val);
            totalRetencaoPISItens += Globalize.parseFloat(item.ValorRetencaoPIS.val);

            if (item.Produto.codEntity > 0)
                qtdProdutosVinculados += 1;
        }

        var mensagem = "";

        var totalGeralItens = totalItens + totalICMSSTItens + totalIPIItens - totalDescontoItens + totalFreteItens + totalOutrasDespesasItens + totalSeguroItens;

        if (totalGeralItens.toFixed(2) != totalDocumento.toFixed(2))
            mensagem = "O valor total dos itens (Valor Total + ICMS ST + IPI + Frete + Seguro + Outras Despesas - Desconto: " + Globalize.format(totalGeralItens, "n2") + ") difere do valor total do documento (" + Globalize.format(totalDocumento, "n2") + "). <br/>";
        else if (totalICMSItens.toFixed(2) != totalICMSDocumento.toFixed(2))
            mensagem = "O valor total de ICMS dos itens (" + Globalize.format(totalICMSItens, "n2") + ") difere do valor total de ICMS do documento (" + Globalize.format(totalICMSDocumento, "n2") + "). <br/>";
        else if (totalPISItens.toFixed(2) != totalPISDocumento.toFixed(2))
            mensagem = "O valor total de PIS dos itens (" + Globalize.format(totalPISItens, "n2") + ") difere do valor total de PIS do documento (" + Globalize.format(totalPISDocumento, "n2") + "). <br/>";
        else if (totalCOFINSItens.toFixed(2) != totalCOFINSDocumento.toFixed(2))
            mensagem = "O valor total de COFINS dos itens (" + Globalize.format(totalCOFINSItens, "n2") + ") difere do valor total de COFINS do documento (" + Globalize.format(totalCOFINSDocumento, "n2") + "). <br/>";
        else if (totalICMSSTItens.toFixed(2) != totalICMSSTDocumento.toFixed(2))
            mensagem = "O valor total de ICMS ST dos itens (" + Globalize.format(totalICMSSTItens, "n2") + ") difere do valor total de ICMS ST do documento (" + Globalize.format(totalICMSSTDocumento, "n2") + "). <br/>";
        else if (totalIPIItens.toFixed(2) != totalIPIDocumento.toFixed(2))
            mensagem = "O valor total de IPI dos itens (" + Globalize.format(totalIPIItens, "n2") + ") difere do valor total de IPI do documento (" + Globalize.format(totalIPIDocumento, "n2") + "). <br/>";
        else if (totalDescontoItens.toFixed(2) != totalDescontoDocumento.toFixed(2))
            mensagem = "O valor total de Desconto dos itens (" + Globalize.format(totalDescontoItens, "n2") + ") difere do valor total de Desconto do documento (" + Globalize.format(totalDescontoDocumento, "n2") + "). <br/>";
        else if (totalOutrasDespesasItens.toFixed(2) != totalOutrasDespesasDocumento.toFixed(2))
            mensagem = "O valor total de Outras Despesas dos itens (" + Globalize.format(totalOutrasDespesasItens, "n2") + ") difere do valor total de Outras Despesas do documento (" + Globalize.format(totalOutrasDespesasDocumento, "n2") + "). <br/>";
        else if (totalFreteItens.toFixed(2) != totalFreteDocumento.toFixed(2))
            mensagem = "O valor total de Frete dos itens (" + Globalize.format(totalFreteItens, "n2") + ") difere do valor total de Frete do documento (" + Globalize.format(totalFreteDocumento, "n2") + "). <br/>";
        else if (baseCalculoICMSSTItens.toFixed(2) != baseCalculoICMSSTDocumento.toFixed(2))
            mensagem = "O valor total de B. C. do ICMS ST dos itens (" + Globalize.format(baseCalculoICMSSTItens, "n2") + ") difere do valor total de B. C. do ICMS ST do documento (" + Globalize.format(baseCalculoICMSSTDocumento, "n2") + "). <br/>";
        else if (baseCalculoICMSItens.toFixed(2) != baseCalculoICMSDocumento.toFixed(2))
            mensagem = "O valor total de B. C. do ICMS dos itens (" + Globalize.format(baseCalculoICMSItens, "n2") + ") difere do valor total de B. C. do ICMS do documento (" + Globalize.format(baseCalculoICMSDocumento, "n2") + "). <br/>";
        else if (totalRetencaoCOFINSDocumento.toFixed(2) != totalRetencaoCOFINSItens.toFixed(2))
            mensagem = "O valor total de retenção de COFINS dos itens (" + Globalize.format(totalRetencaoCOFINSItens, "n2") + ") difere do valor total de retenção de COFINS do documento (" + Globalize.format(totalRetencaoCOFINSDocumento, "n2") + "). <br/>";
        else if (totalRetencaoCSLLDocumento.toFixed(2) != totalRetencaoCSLLItens.toFixed(2))
            mensagem = "O valor total de retenção de CSLL dos itens (" + Globalize.format(totalRetencaoCSLLItens, "n2") + ") difere do valor total de retenção de CSLL do documento (" + Globalize.format(totalRetencaoCSLLDocumento, "n2") + "). <br/>";
        else if (totalRetencaoINSSDocumento.toFixed(2) != totalRetencaoINSSItens.toFixed(2))
            mensagem = "O valor total de retenção de INSS dos itens (" + Globalize.format(totalRetencaoINSSItens, "n2") + ") difere do valor total de retenção de INSS do documento (" + Globalize.format(totalRetencaoINSSDocumento, "n2") + "). <br/>";
        else if (totalRetencaoIPIDocumento.toFixed(2) != totalRetencaoIPIItens.toFixed(2))
            mensagem = "O valor total de retenção de IPI dos itens (" + Globalize.format(totalRetencaoIPIItens, "n2") + ") difere do valor total de retenção de IPI do documento (" + Globalize.format(totalRetencaoIPIDocumento, "n2") + "). <br/>";
        else if (totalRetencaoOutrasItens.toFixed(2) != totalRetencaoOutrasDocumento.toFixed(2))
            mensagem = "O valor total de outras retenções dos itens (" + Globalize.format(totalRetencaoOutrasItens, "n2") + ") difere do valor total de outras retenções do documento (" + Globalize.format(totalRetencaoOutrasDocumento, "n2") + "). <br/>";
        else if (totalRetencaoPISItens.toFixed(2) != totalRetencaoPISDocumento.toFixed(2))
            mensagem = "O valor total de retenção de PIS dos itens (" + Globalize.format(totalRetencaoPISItens, "n2") + ") difere do valor total de retenção de PIS do documento (" + Globalize.format(totalRetencaoPISDocumento, "n2") + "). <br/>";
        else if (totalRetencaoIRItens.toFixed(2) != totalRetencaoIRDocumento.toFixed(2))
            mensagem = "O valor total de outras retenções dos itens (" + Globalize.format(totalRetencaoIRItens, "n2") + ") difere do valor total de outras retenções do documento (" + Globalize.format(totalRetencaoIRDocumento, "n2") + "). <br/>";
        else if (totalRetencaoISSItens.toFixed(2) != totalRetencaoISSDocumento.toFixed(2))
            mensagem = "O valor total de outras retenções dos itens (" + Globalize.format(totalRetencaoISSItens, "n2") + ") difere do valor total de outras retenções do documento (" + Globalize.format(totalRetencaoISSDocumento, "n2") + "). <br/>";

        else if (_CONFIGURACAO_TMS.NaoCadastrarProdutoAutomaticamenteDocumentoEntrada && qtdProdutosVinculados != qtdTotalItens)
            mensagem = "Favor vincular todos os itens da nota! Possui ainda " + Globalize.format(qtdTotalItens - qtdProdutosVinculados) + " sem seleção. <br/>";
        else if ((_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe) && !_CONFIGURACAO_TMS.Transportador.CadastrarProdutoAutomaticamenteDocumentoEntrada && qtdProdutosVinculados != qtdTotalItens)
            mensagem = "Favor vincular todos os itens da nota! Possui ainda " + Globalize.format(qtdTotalItens - qtdProdutosVinculados) + " sem seleção. <br/>";

        if (mensagem != "") {
            exibirMensagem(tipoMensagem.aviso, "Atenção!", mensagem);
            return false;
        }

        for (var j = 0; j < _documentoEntrada.CentroCustos.list.length; j++) {
            var centro = _documentoEntrada.CentroCustos.list[j];
            totalCentroCusto += Globalize.parseFloat(centro.Percentual.val);
        }
        if (totalCentroCusto > 100) {
            mensagem = "A soma do percentual do centro de custo ultrapassa os 100%. <br/>";
        }
        if (mensagem !== "") {
            exibirMensagem(tipoMensagem.aviso, "Atenção!", mensagem);
            return false;
        }
    } else if (_documentoEntrada.Situacao.val() === EnumSituacaoDocumentoEntrada.Anulado) {
        var mensagem = "";
        var chaveNotaAnulacao = _documentoEntrada.ChaveNotaAnulacao.val().trim().replace(/\s/g, "");

        if (chaveNotaAnulacao == _documentoEntrada.Chave.val())
            mensagem = "Favor verificar a chave da nota de anulação, ela deve ser diferente da chave dessa nota.";
        else if (chaveNotaAnulacao.length !== 44)
            mensagem = "Favor verificar a chave da nota de anulação, ela deve possuir 44 dígitos.";
        else if (!ValidarChaveAcesso(chaveNotaAnulacao))
            mensagem = "A chave da NF-e de anulação é inválida.";

        if (_documentoEntrada.Motivo.val() == null || _documentoEntrada.Motivo.val() == undefined || _documentoEntrada.Motivo.val().length < 20)
            mensagem = "O motivo da anulação deve ter no mínimo 20 caracteres.";

        if (mensagem != "") {
            exibirMensagem(tipoMensagem.aviso, "Atenção!", mensagem);
            return false;
        }

    } else if (_documentoEntrada.Situacao.val() === EnumSituacaoDocumentoEntrada.Cancelado) {
        var mensagem = "";

        if (_documentoEntrada.Motivo.val() == null || _documentoEntrada.Motivo.val() == undefined || _documentoEntrada.Motivo.val().length < 20)
            mensagem = "O motivo do cancelamento deve ter no mínimo 20 caracteres.";

        if (mensagem != "") {
            exibirMensagem(tipoMensagem.aviso, "Atenção!", mensagem);
            return false;
        }
    }

    return true;
}

function EditarDocumentoEntrada(documentoEntradaGrid) {
    LimparCamposDocumentoEntrada();
    _documentoEntrada.Codigo.val(documentoEntradaGrid.Codigo);
    BuscarPorCodigo(_documentoEntrada, "DocumentoEntrada/BuscarPorCodigo", function (arg) {


        _pesquisaDocumentoEntrada.ExibirFiltros.visibleFade(false);

        _documentoEntrada.ImportarNFe.visible(false);
        _documentoEntrada.ImportarCTe.visible(false);
        _documentoEntrada.ImportarNFSeCuritiba.visible(false);
        _documentoEntrada.ImportarOrdemCompra.visible(false);

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe)
            $("#liTabGuias").hide();
        else
            $("#liTabGuias").show();

        var data = arg.Data;
        if (data.Situacao == EnumSituacaoDocumentoEntrada.Finalizado) {
            _crudDocumentoEntrada.Atualizar.visible(false);
            _crudDocumentoEntrada.Excluir.visible(false);
            _crudDocumentoEntrada.Adicionar.visible(false);

            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe)
                _crudDocumentoEntrada.ReverterFaturamento.visible(true);
            else if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.ReAbrir, _PermissoesPersonalizadas))
                _crudDocumentoEntrada.ReverterFaturamento.visible(true);

            SetarEnableCamposKnockout(_documentoEntrada, false);
            SetarEnableCamposKnockout(_item, false);
            _item.RegraEntradaDocumento.enable(false);
            SetarEnableCamposKnockout(_duplicata, false);
            SetarEnableCamposKnockout(_guia, false);
            SetarEnableCamposKnockout(_centroCusto, false);
            SetarEnableCamposKnockout(_duplicataGeracaoAutomatica, false);
            SetarEnableCamposKnockout(_centroResultadoTipoDespesa, false);
            SetarEnableCamposKnockout(_veiculo, false);
        } else {
            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe)
                _crudDocumentoEntrada.Atualizar.visible(true);
            else if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Alterar, _PermissoesPersonalizadas)) {
                _crudDocumentoEntrada.Atualizar.visible(true);
            } else {
                SetarEnableCamposKnockout(_documentoEntrada, false);
                SetarEnableCamposKnockout(_item, false);
                _item.RegraEntradaDocumento.enable(false);
                SetarEnableCamposKnockout(_duplicata, false);
                SetarEnableCamposKnockout(_guia, false);
                SetarEnableCamposKnockout(_centroCusto, false);
                SetarEnableCamposKnockout(_duplicataGeracaoAutomatica, false);
                SetarEnableCamposKnockout(_centroResultadoTipoDespesa, false);
                SetarEnableCamposKnockout(_veiculo, false);
            }
            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe)
                _crudDocumentoEntrada.Excluir.visible(true);
            else if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Excluir, _PermissoesPersonalizadas))
                _crudDocumentoEntrada.Excluir.visible(true);

            _crudDocumentoEntrada.Adicionar.visible(false);
            _crudDocumentoEntrada.ReverterFaturamento.visible(false);

            TrocaComponenteModalBuscaCFOPDocumentoEntrada(data.NaturezaOperacao.QuantidadeCFOPs);
        }
        _item.CFOPNF.codEntity(_documentoEntrada.CFOP.codEntity());
        _item.CFOPNF.val(_documentoEntrada.CFOP.val());
        _item.TipoMovimentoNF.codEntity(_documentoEntrada.TipoMovimento.codEntity());
        _item.TipoMovimentoNF.val(_documentoEntrada.TipoMovimento.val());
        _item.NaturezaOperacaoNF.codEntity(_documentoEntrada.NaturezaOperacao.codEntity());
        _item.NaturezaOperacaoNF.val(_documentoEntrada.NaturezaOperacao.val());

        _item.CFOP.codEntity(_documentoEntrada.CFOP.codEntity());
        _item.CFOP.val(_documentoEntrada.CFOP.val());
        _item.TipoMovimento.codEntity(_documentoEntrada.TipoMovimento.codEntity());
        _item.TipoMovimento.val(_documentoEntrada.TipoMovimento.val());
        _item.NaturezaOperacao.codEntity(_documentoEntrada.NaturezaOperacao.codEntity());
        _item.NaturezaOperacao.val(_documentoEntrada.NaturezaOperacao.val());

        if (_documentoEntrada.Equipamento.codEntity() > 0) {
            _item.Equipamento.codEntity(_documentoEntrada.Equipamento.codEntity());
            _item.Equipamento.val(_documentoEntrada.Equipamento.val());
        }
        if (_documentoEntrada.Horimetro.val() > 0) {
            _item.Horimetro.val(_documentoEntrada.Horimetro.val());
        }
        if (_documentoEntrada.Veiculo.codEntity() > 0) {
            _item.Veiculo.codEntity(_documentoEntrada.Veiculo.codEntity());
            _item.Veiculo.val(_documentoEntrada.Veiculo.val());
        }
        if (_documentoEntrada.KMAbastecimento.val() > 0) {
            _item.KMAbastecimento.val(_documentoEntrada.KMAbastecimento.val());
        }
        if (_documentoEntrada.DataAbastecimento.val() != "") {
            _item.DataAbastecimento.val(_documentoEntrada.DataAbastecimento.val());
        }

        RecarregarGridDuplicata();
        RecarregarGridGuia();
        RecarregarGridCentroCusto();
        RecarregarGridItem();
        MontarGridItemAbastecimento();
        MontarGridItemOrdensServico();
        RecarregarGridVeiculo();
        RecarregarGridCentroResultadoTipoDespesa();

        if (data.DocumentoComMoedaEstrangeira) {
            _documentoEntrada.MoedaCotacaoBancoCentral.visible(true);
            _documentoEntrada.DataBaseCRT.visible(true);
            _documentoEntrada.ValorMoedaCotacao.visible(true);
            _documentoEntrada.ValorMoedaCotacao.val(data.ValorMoedaCotacao);
        }
        else {
            _documentoEntrada.MoedaCotacaoBancoCentral.visible(false);
            _documentoEntrada.DataBaseCRT.visible(false);
            _documentoEntrada.ValorMoedaCotacao.visible(false);
        }

        ControleCamposPorModelo(data.Modelo.Numero);

        if (data.SaldoContaAdiantamento != "")
            _duplicata.SaldoContaAdiantamento.val(data.SaldoContaAdiantamento);

    }, null);
}

function baixarDANFENotaFiscalEletronica(documentoEntradaGrid) {
    executarDownload("DocumentoEntrada/DownloadDANFENFeDestinados", { Codigo: documentoEntradaGrid.Codigo });
}

function baixarXMLNotaFiscalEletronica(documentoEntradaGrid) {
    executarDownload("DocumentoEntrada/DownloadXMLNFe", { Codigo: documentoEntradaGrid.Codigo });
}

function BuscarDocumentosEntrada() {
    var editar = { descricao: "Editar", id: guid(), metodo: EditarDocumentoEntrada, tamanho: "8", icone: "" };
    var baixarDANFE = { descricao: "Baixar DANFE Destinados", id: guid(), metodo: baixarDANFENotaFiscalEletronica, icone: "" };
    var baixarXML = { descricao: "Baixar XML NFe", id: guid(), metodo: baixarXMLNotaFiscalEletronica, icone: "" };
    var mostrarIntegracoes = { descricao: "Integrações", id: guid(), metodo: mostrarIntegracoesDocumentoEntrada, icone: ""};

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 10, opcoes: [editar, baixarDANFE, baixarXML, mostrarIntegracoes] };

    var configExportacao = {
        url: "DocumentoEntrada/ExportarPesquisa",
        titulo: "Documentos de Entrada"
    };

    var quantidadeRegistros = _CONFIGURACAO_TMS.QuantidadeRegistrosGridDocumentoEntrada;

    if (quantidadeRegistros == null || quantidadeRegistros <= 0)
        quantidadeRegistros = 5;

    _gridDocumentoEntrada = new GridViewExportacao(_pesquisaDocumentoEntrada.Pesquisar.idGrid, "DocumentoEntrada/Pesquisa", _pesquisaDocumentoEntrada, menuOpcoes, configExportacao, null, quantidadeRegistros);
    _gridDocumentoEntrada.CarregarGrid();
}

function LimparCamposDocumentoEntrada() {
    $("#liTabGuias").hide();
    $("#" + _documentoEntrada.NFe.id).val('');
    $("#" + _documentoEntrada.CTe.id).val('');
    $("#" + _documentoEntrada.NFSeCuritiba.id).val('');

    _crudDocumentoEntrada.Atualizar.visible(false);
    _crudDocumentoEntrada.Excluir.visible(false);
    _crudDocumentoEntrada.Adicionar.visible(true);
    _crudDocumentoEntrada.ReverterFaturamento.visible(false);

    LimparCampos(_documentoEntrada);
    LimparCamposDuplicata();
    LimparCamposCentroCusto();
    LimparCamposItem();
    LimparCamposCentroResultadoTipoDespesa();

    RecarregarGridDuplicata();
    RecarregarGridVeiculo();
    RecarregarGridGuia();
    RecarregarGridCentroCusto();
    RecarregarGridItem();
    Global.ResetarAbas();

    LayoutMultiNFe();

    SetarEnableCamposKnockout(_documentoEntrada, true);
    SetarEnableCamposKnockout(_item, true);
    SetarEnableCamposKnockout(_veiculo, true);
    _item.RegraEntradaDocumento.enable(false);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe || VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.DocumentoEntrada_LancarDuplicata, _PermissoesPersonalizadas)) {
        SetarEnableCamposKnockout(_duplicata, true);
        SetarEnableCamposKnockout(_guia, true);
        SetarEnableCamposKnockout(_centroCusto, true);
        SetarEnableCamposKnockout(_duplicataGeracaoAutomatica, true);
        SetarEnableCamposKnockout(_centroResultadoTipoDespesa, true);
    } else {
        SetarEnableCamposKnockout(_duplicata, false);
        SetarEnableCamposKnockout(_guia, false);
        SetarEnableCamposKnockout(_centroCusto, false);
        SetarEnableCamposKnockout(_duplicataGeracaoAutomatica, false);
        SetarEnableCamposKnockout(_centroResultadoTipoDespesa, false);
    }

    _documentoEntrada.ImportarNFe.visible(true);
    _documentoEntrada.ImportarCTe.visible(true);
    _documentoEntrada.ImportarNFSeCuritiba.visible(true);
    _documentoEntrada.ImportarOrdemCompra.visible(true);

    _documentoEntrada.NumeroLancamento.enable(false);
    _documentoEntrada.EncerrarOrdemServico.visible(false);

    _documentoEntrada.MoedaCotacaoBancoCentral.visible(false);
    _documentoEntrada.DataBaseCRT.visible(false);
    _documentoEntrada.ValorMoedaCotacao.visible(false);
    _documentoEntrada.MoedaCotacaoBancoCentral.val(EnumMoedaCotacaoBancoCentral.Real);

    _item.Sequencial.enable(false);
    _item.ValorCustoUnitario.enable(false);
    _item.ValorCustoTotal.enable(false);

    if (_CONFIGURACAO_TMS.DeixarPadraoFinalizadoDocumentoEntrada) {
        _documentoEntrada.Situacao.val(EnumSituacaoDocumentoEntrada.Finalizado);
    }

    RetornarSemCalculo();
    _documentoEntrada.ItensAbastecimentos.list = new Array();
    _documentoEntrada.ItensOrdensServico.list = new Array();
    ControleCamposPorModelo();
    TrocaComponenteModalBuscaCFOPDocumentoEntrada(0);
}

function ControleCamposPorModelo(modelo) {
    _documentoEntrada.Chave.text("Chave:");
    _documentoEntrada.Chave.required(false);
    _documentoEntrada.Expedidor.visible(false);
    _documentoEntrada.Recebedor.visible(false);
    _documentoEntrada.Serie.text("Série:");
    _documentoEntrada.Serie.required(false);
    _documentoEntrada.LocalidadeInicioPrestacao.visible(false);
    _documentoEntrada.LocalidadeTerminoPrestacao.visible(false);

    if (!Boolean(modelo))
        return;

    if (modelo === "55") {
        _documentoEntrada.Chave.text("*Chave:");
        _documentoEntrada.Chave.required(true);
        _documentoEntrada.Serie.text("*Série:");
        _documentoEntrada.Serie.required(true);
    } else if (modelo === "57") {
        _documentoEntrada.Chave.text("*Chave:");
        _documentoEntrada.Chave.required(true);
        _documentoEntrada.Expedidor.visible(true);
        _documentoEntrada.Recebedor.visible(true);
        _documentoEntrada.Serie.text("*Série:");
        _documentoEntrada.Serie.required(true);
        _documentoEntrada.LocalidadeInicioPrestacao.visible(true);
        _documentoEntrada.LocalidadeTerminoPrestacao.visible(true);
    }
}

function TrocaComponenteModalBuscaCFOPDocumentoEntrada(quantidadeCFOPs) {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiNFe)
        return;

    _buscaCFOP.Destroy();

    if (quantidadeCFOPs > 0)
        _buscaCFOP = new BuscarCFOPNotaFiscal(_documentoEntrada.CFOP, function (r) { RetornoConsultaCFOP(r.Codigo); }, EnumTipoCFOP.Entrada, null, _documentoEntrada.Fornecedor, null, _documentoEntrada.Destinatario, null, null, _documentoEntrada.NaturezaOperacao);
    else
        _buscaCFOP = new BuscarCFOPNotaFiscal(_documentoEntrada.CFOP, function (r) { RetornoConsultaCFOP(r.Codigo); }, EnumTipoCFOP.Entrada, null, _documentoEntrada.Fornecedor, null, _documentoEntrada.Destinatario, null, null, null);

    TrocaComponenteModalBuscaCFOPItemDocumentoEntrada(quantidadeCFOPs);
}

function ControleVisibilidadeOrdemServico() {
    if (_documentoEntrada.OrdemServico.codEntity() > 0)
        _documentoEntrada.EncerrarOrdemServico.visible(true);
    else {
        _documentoEntrada.EncerrarOrdemServico.visible(false);
        _documentoEntrada.EncerrarOrdemServico.val(false);
    }
}

function CalcularMoedaEstrangeira() {
    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) {
        executarReST("Cotacao/ConverterMoedaEstrangeira", { MoedaCotacaoBancoCentral: _documentoEntrada.MoedaCotacaoBancoCentral.val(), DataBaseCRT: _documentoEntrada.DataBaseCRT.val() }, function (r) {
            if (r.Success) {
                if (r.Data != null && r.Data != undefined && r.Data > 0)
                    _documentoEntrada.ValorMoedaCotacao.val(Globalize.format(r.Data, "n10"));
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    }
}

function LayoutMultiNFe() {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe) {

        _pesquisaDocumentoEntrada.Destinatario.visible(false);

        executarReST("DocumentoEntrada/BuscarEmpresaUsuario", { Codigo: 0 }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    _documentoEntrada.Destinatario.codEntity(arg.Data.Codigo);
                    _documentoEntrada.Destinatario.val(arg.Data.Nome);
                    _documentoEntrada.Destinatario.enable(false);
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);

    }
}

function AbrirTelaReteioDespesaVeiculo(data) {
    LimparCamposRateioDespesa();
    Global.abrirModal('divModalDespesaVeiculo');

    if (_CONFIGURACAO_TMS.UtilizarCustoParaRealizarRateiosSobreDocumentoEntrada)
        _rateioDespesa.Valor.val(data.ValorTotalCusto);
    else
        _rateioDespesa.Valor.val(data.Valor);

    _rateioDespesa.DocumentoEntrada.val(data.Codigo);
    _rateioDespesa.NumeroDocumento.val(data.NumeroDocumento);
    _rateioDespesa.TipoDocumento.val(data.TipoDocumento);
    _rateioDespesa.Pessoa.val(data.Pessoa.Descricao);
    _rateioDespesa.Pessoa.codEntity(data.Pessoa.Codigo);
}

function PreencherListasSelecao() {
    _documentoEntrada.ItensAbastecimentos.val(JSON.stringify(_documentoEntrada.ItensAbastecimentos.list));
    _documentoEntrada.ItensOrdensServico.val(JSON.stringify(_documentoEntrada.ItensOrdensServico.list));

    var veiculosSelecao = new Array();
    $.each(_veiculo.Veiculo.basicTable.BuscarRegistros(), function (i, veiculo) {
        veiculosSelecao.push(veiculo.Codigo);
    });

    _documentoEntrada.Veiculos.val(JSON.stringify(veiculosSelecao));
}

function mostrarIntegracoesDocumentoEntrada(_documentoEntrada) {
        recarregarDocumentoEntradaIntegracoes(_documentoEntrada);
}