/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/GrupoPessoa.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Container.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Porto.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/PedidoViagemNavio.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/TipoTerminalImportacao.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/TipoOperacao.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoPropostaMultimodal.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacoesCarga.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoModal.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoServicoMultimodal.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoCTe.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSimNaoPesquisa.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacaoCargaMercante.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioNFeCTeContainer, _gridNFeCTeContainer, _pesquisaNFeCTeContainer, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _situacaoCTeRelatorioNFeCTeContainer = [
    { text: "Autorizado", value: "A" },
    { text: "Pendente", value: "P" },
    { text: "Enviado", value: "E" },
    { text: "Rejeitado", value: "R" },
    { text: "Cancelado", value: "C" },
    { text: "Anulado", value: "Z" },
    { text: "Inutilizado", value: "I" },
    { text: "Denegado", value: "D" },
    { text: "Em Digitação", value: "S" },
    { text: "Em Cancelamento", value: "K" },
    { text: "Em Inutilização", value: "L" }
];

var PesquisaNFeCTeContainer = function () {
    this.DataEmissaoInicial = PropertyEntity({ text: "Data Emissão Inicial:", getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual() });
    this.DataEmissaoFinal = PropertyEntity({ text: "Data Emissão Final:", getType: typesKnockout.date });

    this.DataEmissaoInicial.dateRangeLimit = this.DataEmissaoFinal;
    this.DataEmissaoFinal.dateRangeInit = this.DataEmissaoInicial;

    this.NumeroBooking = PropertyEntity({ text: "Número Booking:" });
    this.NumeroOS = PropertyEntity({ text: "Número OS:" });
    this.NumeroControle = PropertyEntity({ text: "Número Controle:" });
    this.NumeroCTe = PropertyEntity({ text: "Número CT-e:", getType: typesKnockout.int });
    this.NumeroNota = PropertyEntity({ text: "Número Nota:", getType: typesKnockout.int });
    this.NumeroSerie = PropertyEntity({ text: "Número Série:", getType: typesKnockout.int });

    this.SituacaoCarga = PropertyEntity({ val: ko.observable(EnumSituacoesCarga.Todas), def: EnumSituacoesCarga.Todas, text: "Situação Carga:", options: EnumSituacoesCarga.obterOpcoesTMS(), getType: typesKnockout.selectMultiple, visible: ko.observable(true) });
    this.SituacaoCargaMercante = PropertyEntity({ text: "Situação Carga: ", getType: typesKnockout.selectMultiple, val: ko.observable([]), options: EnumSituacaoCargaMercante.obterOpcoes(), def: [], visible: ko.observable(false) });

    this.TipoProposta = PropertyEntity({ val: ko.observable(EnumTipoPropostaMultimodal.Todos), def: EnumTipoPropostaMultimodal.Todos, text: "Tipo Proposta:", options: EnumTipoPropostaMultimodal.obterOpcoesSemNumero(), getType: typesKnockout.selectMultiple });
    this.TipoModal = PropertyEntity({ val: ko.observable(EnumTipoModal.Todos), def: EnumTipoModal.Todos, text: "Tipo Documento:", options: EnumTipoModal.obterOpcoes(), getType: typesKnockout.selectMultiple });
    this.SituacaoCTe = PropertyEntity({ val: ko.observable(""), def: "", text: "Situação CTe:", options: _situacaoCTeRelatorioNFeCTeContainer, getType: typesKnockout.selectMultiple });
    this.TipoServico = PropertyEntity({ val: ko.observable(EnumTipoServicoMultimodal.Todos), def: EnumTipoServicoMultimodal.Todos, text: "Tipo Serviço:", options: EnumTipoServicoMultimodal.obterOpcoesSemNumero(), getType: typesKnockout.selectMultiple });
    this.TipoCTe = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, options: EnumTipoCTe.ObterOpcoes(), text: "Tipo do CT-e:" });
    this.FoiAnulado = PropertyEntity({ val: ko.observable(EnumSimNaoPesquisa.Todos), def: EnumSimNaoPesquisa.Todos, text: "Foi Anulado?", options: EnumSimNaoPesquisa.obterOpcoesPesquisa() });
    this.FoiSubstituido = PropertyEntity({ val: ko.observable(EnumSimNaoPesquisa.Todos), def: EnumSimNaoPesquisa.Todos, text: "Foi Substituído?", options: EnumSimNaoPesquisa.obterOpcoesPesquisa() });

    this.PortoOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Porto Origem:", idBtnSearch: guid() });
    this.PortoDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Porto Destino:", idBtnSearch: guid() });
    this.Viagem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Viagem:", idBtnSearch: guid() });
    this.Container = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Container:", idBtnSearch: guid() });
    this.TerminalOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Terminal Origem:", idBtnSearch: guid() });
    this.TerminalDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Terminal Destino:", idBtnSearch: guid() });
    this.GrupoPessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo Pessoa:", idBtnSearch: guid() });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo Operação:", idBtnSearch: guid() });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridNFeCTeContainer.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

//*******EVENTOS*******


function loadRelatorioNFeCTeContainer() {

    _pesquisaNFeCTeContainer = new PesquisaNFeCTeContainer();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridNFeCTeContainer = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/NFeCTeContainer/Pesquisa", _pesquisaNFeCTeContainer, null, null, 10);
    _gridNFeCTeContainer.SetPermitirEdicaoColunas(true);

    _relatorioNFeCTeContainer = new RelatorioGlobal("Relatorios/NFeCTeContainer/BuscarDadosRelatorio", _gridNFeCTeContainer, function () {
        _relatorioNFeCTeContainer.loadRelatorio(function () {
            KoBindings(_pesquisaNFeCTeContainer, "knockoutPesquisaNFeCTeContainer");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaNFeCTeContainer");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaNFeCTeContainer");

            new BuscarTipoTerminalImportacao(_pesquisaNFeCTeContainer.TerminalOrigem);
            new BuscarTipoTerminalImportacao(_pesquisaNFeCTeContainer.TerminalDestino);
            new BuscarPedidoViagemNavio(_pesquisaNFeCTeContainer.Viagem);
            new BuscarContainers(_pesquisaNFeCTeContainer.Container);
            new BuscarTiposOperacao(_pesquisaNFeCTeContainer.TipoOperacao);
            new BuscarPorto(_pesquisaNFeCTeContainer.PortoOrigem);
            new BuscarPorto(_pesquisaNFeCTeContainer.PortoDestino);
            new BuscarGruposPessoas(_pesquisaNFeCTeContainer.GrupoPessoa);

            if (_CONFIGURACAO_TMS.AtivarNovosFiltrosConsultaCarga) {
                _pesquisaNFeCTeContainer.SituacaoCarga.visible(false);
                _pesquisaNFeCTeContainer.SituacaoCarga.val(new Array());
                _pesquisaNFeCTeContainer.SituacaoCargaMercante.visible(true);
            }

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaNFeCTeContainer);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioNFeCTeContainer.gerarRelatorio("Relatorios/NFeCTeContainer/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioNFeCTeContainer.gerarRelatorio("Relatorios/NFeCTeContainer/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}