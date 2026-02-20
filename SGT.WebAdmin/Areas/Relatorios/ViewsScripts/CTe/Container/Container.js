/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/GrupoPessoa.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Container.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Porto.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/PedidoViagemNavio.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/TipoTerminalImportacao.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/TipoOperacao.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Navio.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoPropostaMultimodal.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacoesCarga.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoModal.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoServicoMultimodal.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoCTe.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSimNaoPesquisa.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacaoCargaMercante.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioContainer, _gridContainer, _pesquisaContainer, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _situacaoCTeRelatorioContainer = [
    { text: "Autorizado", value: "A" },
    { text: "Pendente", value: "P" },
    { text: "Enviado", value: "E" },
    { text: "Rejeitado", value: "R" },
    { text: "Cancelado", value: "C" },
    { text: "Anulado Gerencialmente", value: "Z" },
    { text: "Inutilizado", value: "I" },
    { text: "Denegado", value: "D" },
    { text: "Em Digitação", value: "S" },
    { text: "Em Cancelamento", value: "K" },
    { text: "Em Inutilização", value: "L" }
];

var PesquisaContainer = function () {
    this.DataEmissaoInicial = PropertyEntity({ text: "*Data Emissão Inicial:", issue: 2, getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual(), required: true });
    this.DataEmissaoFinal = PropertyEntity({ text: "*Data Emissão Final:", issue: 2, getType: typesKnockout.date, required: true });

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
    this.SituacaoCTe = PropertyEntity({ val: ko.observable(""), def: "", text: "Situação CTe:", options: _situacaoCTeRelatorioContainer, getType: typesKnockout.selectMultiple });
    this.TipoServico = PropertyEntity({ val: ko.observable(EnumTipoServicoMultimodal.Todos), def: EnumTipoServicoMultimodal.Todos, text: "Tipo Serviço:", options: EnumTipoServicoMultimodal.obterOpcoesSemNumero(), getType: typesKnockout.selectMultiple });

    this.PortoOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Porto Origem:", idBtnSearch: guid() });
    this.PortoDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Porto Destino:", idBtnSearch: guid() });
    this.Viagem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Viagem:", idBtnSearch: guid(), required: false });
    this.Container = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Container:", idBtnSearch: guid() });
    this.TerminalOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Terminal Origem:", idBtnSearch: guid() });
    this.TerminalDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Terminal Destino:", idBtnSearch: guid() });
    this.GrupoPessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo Pessoa:", idBtnSearch: guid() });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo Operação:", idBtnSearch: guid() });

    this.VeioPorImportacao = PropertyEntity({ text: "Veio por importação:", options: EnumSimNaoPesquisa.obterOpcoesPesquisa(), val: ko.observable(EnumSimNaoPesquisa.Todos), def: EnumSimNaoPesquisa.Todos, visible: ko.observable(true) });
    this.SomenteCTeSubstituido = PropertyEntity({ text: "Somente CT-e Substituído", val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.TipoCTe = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, options: EnumTipoCTe.ObterOpcoes(), text: "Tipo do CT-e:", visible: ko.observable(true) });

    this.ViagemTransbordo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Viagem Transbordo:", idBtnSearch: guid() });
    this.PortoTransbordo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Porto Transbordo:", idBtnSearch: guid() });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });

    this.Balsa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Balsa:", idBtnSearch: guid() });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            debugger
            _gridContainer.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

//*******EVENTOS*******


function loadRelatorioContainer() {

    _pesquisaContainer = new PesquisaContainer();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridContainer = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/Container/Pesquisa", _pesquisaContainer, null, null, 10);
    _gridContainer.SetPermitirEdicaoColunas(true);

    _relatorioContainer = new RelatorioGlobal("Relatorios/Container/BuscarDadosRelatorio", _gridContainer, function () {
        _relatorioContainer.loadRelatorio(function () {
            KoBindings(_pesquisaContainer, "knockoutPesquisaContainer");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaContainer");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaContainer");

            new BuscarTipoTerminalImportacao(_pesquisaContainer.TerminalOrigem);
            new BuscarTipoTerminalImportacao(_pesquisaContainer.TerminalDestino);
            new BuscarPedidoViagemNavio(_pesquisaContainer.Viagem);
            new BuscarContainers(_pesquisaContainer.Container);
            new BuscarTiposOperacao(_pesquisaContainer.TipoOperacao);
            new BuscarPorto(_pesquisaContainer.PortoOrigem);
            new BuscarPorto(_pesquisaContainer.PortoDestino);
            new BuscarGruposPessoas(_pesquisaContainer.GrupoPessoa);
            new BuscarPedidoViagemNavio(_pesquisaContainer.ViagemTransbordo);
            new BuscarPorto(_pesquisaContainer.PortoTransbordo);
            new BuscarNavios(_pesquisaContainer.Balsa, null, null, EnumTipoEmbarcacao.Balsa);

            if (_CONFIGURACAO_TMS.AtivarNovosFiltrosConsultaCarga) {
                _pesquisaContainer.SituacaoCarga.visible(false);
                _pesquisaContainer.SituacaoCargaMercante.visible(true);
            }

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaContainer);
}

function GerarRelatorioPDFClick(e, sender) {
    if (ValidarFiltrosObrigatorios())
        _relatorioContainer.gerarRelatorio("Relatorios/Container/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    if (ValidarFiltrosObrigatorios())
        _relatorioContainer.gerarRelatorio("Relatorios/Container/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}

function ValidarFiltrosObrigatorios() {
    var tudoCerto = true;
    var valido = ValidarCamposObrigatorios(_pesquisaContainer);

    if (!valido) {
        exibirMensagem(tipoMensagem.atencao, "Filtros Obrigatórios", "Informe os filtros obrigatórios!");
        tudoCerto = false;
    }

    return tudoCerto;
}