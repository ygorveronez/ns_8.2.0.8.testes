/// <reference path="../../../../../ViewsScripts/Consultas/DocumentoEntrada.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Conhecimento.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/CTe.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Titulo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/GrupoPessoa.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Fatura.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/TipoOperacao.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/TipoPagamentoRecebimento.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/PagamentoEletronico.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/TipoMovimento.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Remessa.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Cheque.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/ModeloDocumentoFiscal.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/PagamentoMotoristaTipo.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumFormaTitulo.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumProvisaoPesquisaTitulo.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoDocumentoPesquisaTitulo.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoTitulo.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacaoTitulo.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoBoletoPesquisaTitulo.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSimNaoPesquisa.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumMoedaCotacaoBancoCentral.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacaoTitulo.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTituloRenegociado.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/BoletoRetornoComando.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioTitulo, _gridTitulo, _pesquisaTitulo, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _tipoModeloFatura = [
    { text: "Todos", value: "" },
    { text: "Novo", value: true },
    { text: "Antigo", value: false }
];

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

var PesquisaTitulo = function () {
    this.TipoTitulo = PropertyEntity({ val: ko.observable(EnumTipoTitulo.AReceber), options: EnumTipoTitulo.obterOpcoesPesquisa(), def: EnumTipoTitulo.AReceber, text: "Tipo Título:" });
    this.ModeloFatura = PropertyEntity({ val: ko.observable(""), options: _tipoModeloFatura, def: "", text: "Modelo da Fatura:" });

    this.StatusTitulo = PropertyEntity({ val: ko.observable([EnumSituacaoTitulo.EmAberto, EnumSituacaoTitulo.Quitado]), def: [EnumSituacaoTitulo.EmAberto, EnumSituacaoTitulo.Quitado], getType: typesKnockout.selectMultiple, text: "Situação: ", options: EnumSituacaoTitulo.obterOpcoes(), visible: ko.observable(true) });

    this.Pessoa = PropertyEntity({ text: "Cliente / Fornecedor:", type: types.multiplesEntities, codEntity: ko.observable(0), idBtnSearch: guid(), val: ko.observable("") });
    this.Categoria = PropertyEntity({ text: "Categoria:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), val: ko.observable("") });
    this.ConhecimentoDeTransporte = PropertyEntity({ text: "CT-e:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), val: ko.observable(""), visible: ko.observable(true) });
    this.Titulo = PropertyEntity({ text: "Título:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), val: ko.observable("") });

    this.Fatura = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Fatura:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.Bordero = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Borderô:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.GrupoPessoa = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Grupo de Pessoas:", idBtnSearch: guid(), issue: 58, visible: ko.observable(true) });

    this.DataBaseInicial = PropertyEntity({ text: "Data base liquidação: ", val: ko.observable(""), getType: typesKnockout.date });
    this.DataBaseFinal = PropertyEntity({ text: "Até: ", val: ko.observable(""), getType: typesKnockout.date });
    this.DataInicialEmissao = PropertyEntity({ text: "Período inicial de emissão: ", val: ko.observable(""), getType: typesKnockout.date });
    this.DataFinalEmissao = PropertyEntity({ text: "Até: ", val: ko.observable(""), getType: typesKnockout.date });
    this.DataInicialVencimento = PropertyEntity({ text: "Período inicial de vencimento: ", val: ko.observable(""), getType: typesKnockout.date });
    this.DataFinalVencimento = PropertyEntity({ text: "Até: ", val: ko.observable(""), getType: typesKnockout.date });
    this.DataInicialQuitacao = PropertyEntity({ text: "Período inicial da quitação do Título: ", val: ko.observable(""), getType: typesKnockout.date });
    this.DataFinalQuitacao = PropertyEntity({ text: "Até: ", val: ko.observable(""), getType: typesKnockout.date });
    this.DataInicialCancelamento = PropertyEntity({ text: "Data de cancelamento do título: ", val: ko.observable(""), getType: typesKnockout.date });
    this.DataFinalCancelamento = PropertyEntity({ text: "Até: ", val: ko.observable(""), getType: typesKnockout.date });
    this.DataInicialEmissaoDocumentoEntrada = PropertyEntity({ text: "Data emissão Doc. Entrada: ", val: ko.observable(""), getType: typesKnockout.date });
    this.DataFinalEmissaoDocumentoEntrada = PropertyEntity({ text: "Até: ", val: ko.observable(""), getType: typesKnockout.date });
    this.DataInicialEntradaDocumentoEntrada = PropertyEntity({ text: "Data entrada Doc. Entrada: ", val: ko.observable(""), getType: typesKnockout.date });
    this.DataFinalEntradaDocumentoEntrada = PropertyEntity({ text: "Até: ", val: ko.observable(""), getType: typesKnockout.date });
    this.DataAutorizacaoInicial = PropertyEntity({ text: "Data autorização inicial: ", val: ko.observable(""), getType: typesKnockout.date });
    this.DataAutorizacaoFinal = PropertyEntity({ text: "Até: ", val: ko.observable(""), getType: typesKnockout.date });
    this.DataProgramacaoPagamentoInicial = PropertyEntity({ text: "Data programação de: ", val: ko.observable(""), getType: typesKnockout.date });
    this.DataProgramacaoPagamentoFinal = PropertyEntity({ text: "Até: ", val: ko.observable(""), getType: typesKnockout.date });
    this.DataInicialLancamento = PropertyEntity({ text: "Período de Lançamento:", getType: typesKnockout.date });
    this.DataFinalLancamento = PropertyEntity({ text: "Até:", getType: typesKnockout.date });
    this.Moeda = PropertyEntity({ val: ko.observable(EnumMoedaCotacaoBancoCentral.Todos), options: EnumMoedaCotacaoBancoCentral.obterOpcoesPesquisa(), def: EnumMoedaCotacaoBancoCentral.Todas, text: "Moeda:", visible: ko.observable(false) });
    this.Autorizados = PropertyEntity({ val: ko.observable(EnumSimNaoPesquisa.Todos), options: EnumSimNaoPesquisa.obterOpcoesPesquisa(), def: EnumSimNaoPesquisa.Todos, text: "Autorizados:", visible: ko.observable(true) });

    this.DataInicialEmissaoDocumentoEntrada.dateRangeLimit = this.DataFinalEmissaoDocumentoEntrada;
    this.DataFinalEmissaoDocumentoEntrada.dateRangeInit = this.DataInicialEmissaoDocumentoEntrada;
    this.DataInicialEntradaDocumentoEntrada.dateRangeLimit = this.DataFinalEntradaDocumentoEntrada;
    this.DataFinalEntradaDocumentoEntrada.dateRangeInit = this.DataInicialEntradaDocumentoEntrada;
    this.DataAutorizacaoInicial.dateRangeLimit = this.DataAutorizacaoFinal;
    this.DataAutorizacaoFinal.dateRangeInit = this.DataAutorizacaoInicial;
    this.DataInicialLancamento.dataRangeLimit = this.DataFinalLancamento;
    this.DataFinalLancamento.dataRangeInit = this.DataInicialLancamento;

    this.DataPosicaoFinal = PropertyEntity({ text: "Data da posição financeira até: ", val: ko.observable(""), getType: typesKnockout.date, visible: ko.observable(false) });
    this.ValorInicial = PropertyEntity({ text: "Valor do título: ", val: ko.observable("0,00"), getType: typesKnockout.decimal, visible: ko.observable(true) });
    this.ValorFinal = PropertyEntity({ text: "Até: ", val: ko.observable("0,00"), getType: typesKnockout.decimal, visible: ko.observable(true) });
    this.DocumentoOriginal = PropertyEntity({ text: "Nº Doc. Original: ", visible: ko.observable(true) });
    this.NumeroPedidoCliente = PropertyEntity({ text: "Nº Pedido Cliente:", visible: ko.observable(true) });
    this.NumeroOcorrenciaCliente = PropertyEntity({ text: "Nº Ocorrência Cliente:", visible: ko.observable(true) });
    this.NumeroOcorrencia = PropertyEntity({ text: "Nº Ocorrência:", getType: typesKnockout.int, visible: ko.observable(true), configInt: { precision: 0, allowZero: false, thousands: '' } });
    this.NumeroDocumentoOriginario = PropertyEntity({ text: "Nº Doc. Originário: ", getType: typesKnockout.int, visible: ko.observable(true), configInt: { precision: 0, allowZero: false, thousands: '' } });
    this.TipoProposta = PropertyEntity({ getType: typesKnockout.selectMultiple, val: ko.observable(new Array()), def: new Array(), text: "Tipo Proposta:", options: EnumTipoPropostaMultimodal.obterOpcoesPerfil() });

    this.DocumentoEntrada = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Documento de Entrada:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.Empresa = PropertyEntity({ text: ko.observable("Transportador:"), issue: 69, type: types.multiplesEntities, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoMovimento = PropertyEntity({ text: ko.observable("Tipo Movimento:"), type: types.multiplesEntities, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });

    this.Portador = PropertyEntity({ text: "Portador:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), val: ko.observable(""), visible: ko.observable(true) });
    this.Adiantado = PropertyEntity({ val: ko.observable(-1), options: _adiantado, def: -1, text: "Adiantado:", visible: ko.observable(false) });
    this.Provisao = PropertyEntity({ val: ko.observable(EnumProvisaoPesquisaTitulo.ComProvisao), options: _provisaoPesquisaTitulo, def: EnumProvisaoPesquisaTitulo.ComProvisao, text: "Provisão:", visible: ko.observable(true) });

    this.TipoDocumento = PropertyEntity({ val: ko.observable(EnumTipoDocumentoPesquisaTitulo.Todos), options: EnumTipoDocumentoPesquisaTitulo.obterOpcoesPesquisa(), def: EnumTipoDocumentoPesquisaTitulo.Todos, text: "Tipo de Documento:", visible: ko.observable(true) });
    this.TipoBoleto = PropertyEntity({ val: ko.observable(EnumTipoBoletoPesquisaTitulo.Todos), options: EnumTipoBoletoPesquisaTitulo.obterOpcoesPesquisa(), def: EnumTipoBoletoPesquisaTitulo.Todos, text: "Tipo Boleto:", visible: ko.observable(true) });
    this.FormaTitulo = PropertyEntity({ val: ko.observable(EnumFormaTitulo.Todos), def: EnumFormaTitulo.Todos, text: "Forma do Título:", options: EnumFormaTitulo.obterOpcoes(), getType: typesKnockout.selectMultiple, required: false });

    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo Operação:", idBtnSearch: guid(), visible: ko.observable(true), val: ko.observable("") });
    this.TipoPagamentoRecebimento = PropertyEntity({ text: "Tipo Pagamento Recebimento:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), val: ko.observable("") });
    this.PagamentoEletronico = PropertyEntity({ text: "Remessa Pagamento:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), val: ko.observable("") });
    this.Remessa = PropertyEntity({ text: "Remessa:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), val: ko.observable("") });
    this.Cheque = PropertyEntity({ text: "Cheque:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), val: ko.observable("") });
    this.PagamentoMotoristaTipo = PropertyEntity({ text: "Tipo Pagamento Motorista:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), val: ko.observable(""), visible: ko.observable(true) });

    this.ComandoBanco = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Comando Banco:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });

    var situacoesContato = self.SituacoesContato || [];
    var situacaoContatoDefault = situacoesContato.length > 0 ? situacoesContato[0].value : "";


    this.TipoContato = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, text: "Tipo de Contato:", options: ko.observable([]), url: "TipoContato/BuscarTodos", visible: ko.observable(true) });
    this.SituacaoContato = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, text: "Situação:", options: ko.observable([]), url: "SituacaoContato/BuscarTodos", visible: ko.observable(true) });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true), val: ko.observable("") });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo", idBtnSearch: guid(), visible: ko.observable(true), val: ko.observable("") });
    this.TituloRenegociado = PropertyEntity({ val: ko.observable(EnumTituloRenegociado.Todos), options: EnumTituloRenegociado.obterOpcoesPesquisa(), text: "Renegociado: ", def: EnumTituloRenegociado.Todos });
    this.ModeloDocumento = PropertyEntity({ text: "Modelo de Documento:", type: types.multiplesEntities, codEntity: ko.observable(0), idBtnSearch: guid(), val: ko.observable("") });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            console.log(_pesquisaTitulo.TipoContato.val());
            console.log(_pesquisaTitulo.SituacaoContato.val());

            _gridTitulo.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaTitulo.Visible.visibleFade()) {
                _pesquisaTitulo.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaTitulo.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", icon: ko.observable("fal fa-plus"), visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

//*******EVENTOS*******

function loadRelatorioTitulo() {

    _pesquisaTitulo = new PesquisaTitulo();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridTitulo = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/Titulo/Pesquisa", _pesquisaTitulo, null, null, 10);
    _gridTitulo.SetPermitirEdicaoColunas(true);

    _relatorioTitulo = new RelatorioGlobal("Relatorios/Titulo/BuscarDadosRelatorio", _gridTitulo, function () {
        _relatorioTitulo.loadRelatorio(function () {
            KoBindings(_pesquisaTitulo, "knockoutPesquisaTitulo");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaTitulo");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaTitulo");

            new BuscarClientes(_pesquisaTitulo.Pessoa);
            new BuscarClientes(_pesquisaTitulo.Portador);
            new BuscarConhecimentoNotaReferencia(_pesquisaTitulo.ConhecimentoDeTransporte, RetornoCTe);
            new BuscarTitulo(_pesquisaTitulo.Titulo, null, null, RetornoTitulo);
            new BuscarFatura(_pesquisaTitulo.Fatura, RetornoFatura);
            new BuscarDocumentoEntrada(_pesquisaTitulo.DocumentoEntrada, RetornoDocumentoEntrada);
            new BuscarBorderos(_pesquisaTitulo.Bordero);
            new BuscarTransportadores(_pesquisaTitulo.Empresa);
            new BuscarBorderos(_pesquisaTitulo.Bordero);
            new BuscarTiposOperacao(_pesquisaTitulo.TipoOperacao);
            new BuscarTipoPagamentoRecebimento(_pesquisaTitulo.TipoPagamentoRecebimento);
            new BuscarPagamentoEletronico(_pesquisaTitulo.PagamentoEletronico);
            new BuscarTipoMovimento(_pesquisaTitulo.TipoMovimento);
            new BuscarRemessa(_pesquisaTitulo.Remessa);
            new BuscarCheque(_pesquisaTitulo.Cheque, RetornoCheque);
            new BuscarCategoriaPessoa(_pesquisaTitulo.Categoria);
            new BuscarModeloDocumentoFiscal(_pesquisaTitulo.ModeloDocumento);
            new BuscarPagamentoMotoristaTipo(_pesquisaTitulo.PagamentoMotoristaTipo);
            new BuscarGruposPessoas(_pesquisaTitulo.GrupoPessoa);
            new BuscarBoletoRetornoComando(_pesquisaTitulo.ComandoBanco, RetornoComandoBancoTitulo);
            new BuscarVeiculos(_pesquisaTitulo.Veiculo)

            if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) {
                _pesquisaTitulo.Moeda.visible(true);
            }

            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe) {
                _pesquisaTitulo.Adiantado.visible(true);
                _pesquisaTitulo.Fatura.visible(false);
                _pesquisaTitulo.GrupoPessoa.visible(false);
                _pesquisaTitulo.Bordero.visible(false);
                _pesquisaTitulo.NumeroPedidoCliente.visible(false);
                _pesquisaTitulo.NumeroOcorrenciaCliente.visible(false);
                _pesquisaTitulo.NumeroOcorrencia.visible(false);
                _pesquisaTitulo.Empresa.visible(false);
                _pesquisaTitulo.PagamentoMotoristaTipo.visible(false);
            }

            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiEmbarcador) {
                _pesquisaTitulo.Empresa.text("Empresa/Filial");
            }

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaTitulo);
}

function RetornoTitulo(data) {
    _pesquisaTitulo.Titulo.val(data.Codigo);
    _pesquisaTitulo.Titulo.codEntity(data.Codigo);
}

function RetornoFatura(data) {
    _pesquisaTitulo.Fatura.codEntity(data.Codigo);
    _pesquisaTitulo.Fatura.val(data.Numero);
}

function RetornoCTe(data) {
    _pesquisaTitulo.ConhecimentoDeTransporte.val(data.Numero + "-" + data.Serie);
    _pesquisaTitulo.ConhecimentoDeTransporte.codEntity(data.Codigo);
}

function RetornoDocumentoEntrada(data) {
    _pesquisaTitulo.DocumentoEntrada.val(data.Numero);
    _pesquisaTitulo.DocumentoEntrada.codEntity(data.Codigo);
}

function RetornoCheque(data) {
    _pesquisaTitulo.Cheque.val(data.NumeroCheque);
    _pesquisaTitulo.Cheque.codEntity(data.Codigo);
}

function RetornoComandoBancoTitulo(data) {
    _pesquisaTitulo.ComandoBanco.codEntity(data.Codigo);
    _pesquisaTitulo.ComandoBanco.val(data.Descricao);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioTitulo.gerarRelatorio("Relatorios/Titulo/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioTitulo.gerarRelatorio("Relatorios/Titulo/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
