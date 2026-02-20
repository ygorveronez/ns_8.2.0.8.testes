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
/// <reference path="../../Consultas/BoletoConfiguracao.js" />
/// <reference path="../../Enumeradores/EnumDownloadRealizado.js" />

var _listaSituacaoConciliacao = [
    { text: "Todos", value: 0 },
    { text: "Aberta", value: 1 },
    { text: "Disponível para anuência", value: 3 },
    { text: "Assinada", value: 4 },
];

var _listaTipoPagamento = [
    { text: 'Todos', value: -1 },
    { text: 'Pago', value: 0, },
    { text: 'Em Aberto', value: 1, },
];

var _listaTipoDocumento = [
    { text: "Todos", value: 99, },
    { text: "CTe", value: 0, },
    { text: "NFSe", value: 1, },
    { text: "Outros", value: 2, },
    { text: "NFS", value: 3, },
    { text: "Subcontratacao", value: 4, },
];


//*******MAPEAMENTO KNOUCKOUT*******

var _gridConciliacao;
var _gridCtes;
var _gridCtesAssinatura;
var _gridAcrescimoDecrescimo;
var _detalhesEtapa;
var _assinarEtapa;
var _pesquisaConciliacao;
var _etapaConciliacaoTransporte;


var PesquisaConciliacao = function () {
    this.Transportador = PropertyEntity({ text: "Transportadores:", type: types.multiplesEntities, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) });
    this.Situacao = PropertyEntity({ val: ko.observable(0), options: _listaSituacaoConciliacao, def: 0, text: "Situação: " });
    this.NumeroCarta = PropertyEntity({ text: "Número da carta: ", col: 12 });

    this.DataInicial = PropertyEntity({ text: "Período Inicial: ", getType: typesKnockout.month, val: ko.observable(null), cssClass: ko.observable("") });
    this.DataFinal = PropertyEntity({ text: "Período Final: ", getType: typesKnockout.month, val: ko.observable(null), cssClass: ko.observable("") });
    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.AnuenciaDisponivelInicio = PropertyEntity({ text: "Anuência disponível Início: ", getType: typesKnockout.month, val: ko.observable(null), cssClass: ko.observable("") });
    this.AnuenciaDisponivelFinal = PropertyEntity({ text: "Anuência disponível Final: ", getType: typesKnockout.month, val: ko.observable(null), cssClass: ko.observable("") });
    this.AnuenciaDisponivelInicio.dateRangeLimit = this.AnuenciaDisponivelFinal;
    this.AnuenciaDisponivelFinal.dateRangeInit = this.AnuenciaDisponivelInicio;

    this.DataInicialAssinatura = PropertyEntity({ text: "Data Assinatura Inicial: ", getType: typesKnockout.month, val: ko.observable(null), cssClass: ko.observable("") });
    this.DataFinalAssinatura = PropertyEntity({ text: "Data Assinatura Final: ", getType: typesKnockout.month, val: ko.observable(null), cssClass: ko.observable("") });
    this.DataInicialAssinatura.dateRangeLimit = this.DataFinalAssinatura;
    this.DataFinalAssinatura.dateRangeInit = this.DataInicialAssinatura;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridConciliacao.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });
};

var EtapaConciliacaoTransporte = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("50%") });
    this.Visible = PropertyEntity({ type: types.local, val: ko.observable(true) });

    this.Etapa1 = PropertyEntity({
        text: "Detalhes", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(1),
        tooltipTitle: ko.observable("Detalhes"),
        tooltip: ko.observable("Visualização dos documentos da Conciliação")
    });
    this.Etapa2 = PropertyEntity({
        text: "Assinatura", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(2),
        tooltipTitle: ko.observable("Assinatura"),
        tooltip: ko.observable("Assinatura da Carta de Anuência")
    });
}

var DetalhesEtapa = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.DataInicial = PropertyEntity({ text: "Data Emissão Inicial: ", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataFinal = PropertyEntity({ text: "Data Emissão Final: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.TipoPagamento = PropertyEntity({ val: ko.observable(0), options: _listaTipoPagamento, def: -1, text: "Situação: " });
    this.TipoDocumento = PropertyEntity({ val: ko.observable(99), options: _listaTipoDocumento, def: 99, text: "Tipo documento: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridCtes.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
};

var AssinarEtapa = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.GridCtes = PropertyEntity({ type: types.local, idGrid: guid(), visible: ko.observable(true) });
    this.BaixarCarta = PropertyEntity({ eventClick: baixarCartaClick, type: types.event, text: "Baixar carta de anuência", visible: ko.observable(false) });
    this.Assinar = PropertyEntity({ eventClick: assinarClick, type: types.event, text: "Assinar anuência", visible: ko.observable(false) });

    // Apenas para filtragem. Não muda nunca
    this.TipoPagamento = PropertyEntity({ val: ko.observable(0), def: -1 });
    this.TipoDocumento = PropertyEntity({ val: ko.observable(99), def: 99 });
};

var DetalhesCte = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.AcrescimosDecrescimos = PropertyEntity({ type: types.local, idGrid: guid() });
};


//*******EVENTOS*******


function loadConciliacao() {
    _detalhesEtapa = new DetalhesEtapa();
    KoBindings(_detalhesEtapa, "knockoutDetalhesEtapa");

    _assinarEtapa = new AssinarEtapa();
    KoBindings(_assinarEtapa, "knockoutAssinaturaEtapa");

    HeaderAuditoria("Conciliacao", _detalhesEtapa);

    _pesquisaConciliacao = new PesquisaConciliacao();
    KoBindings(_pesquisaConciliacao, "knockoutPesquisaConciliacao", false, _pesquisaConciliacao.Pesquisar.id);

    _etapaConciliacaoTransporte = new EtapaConciliacaoTransporte();
    KoBindings(_etapaConciliacaoTransporte, "knockoutEtapaConciliacaoTransporte");

    new BuscarTransportadores(_pesquisaConciliacao.Transportador);
    new BuscarTransportadores(_detalhesEtapa.Transportador);
    buscarConciliacoes();

    _detalhesCte = new DetalhesCte();
    KoBindings(_detalhesCte, "knockoutDetalhesCte");
    
    $("[rel=popover-hover]").popover({ trigger: "hover" });
}

//*******MÉTODOS*******

function buscarConciliacoes() {
    var detalhes = { descricao: "Detalhes", evento: "onclick", metodo: verDetalhesConciliacao };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(detalhes);

    var configExportacao = {
        url: "ConciliacaoTransportador/ExportarPesquisa",
        titulo: "Anuência Transportador"
    };

    _gridConciliacao = new GridViewExportacao(_pesquisaConciliacao.Pesquisar.idGrid, "ConciliacaoTransportador/Pesquisa", _pesquisaConciliacao, menuOpcoes, configExportacao);
    _gridConciliacao.CarregarGrid();
}

function buscarCtes() {
    var detalhes = { descricao: "Detalhes", evento: "onclick", metodo: verDetalhesCte };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(detalhes);

    var configExportacao = {
        url: "ConciliacaoTransportador/ExportarPesquisaCtes",
        titulo: "Anuência Transportador"
    };

    _gridCtes = new GridViewExportacao(_detalhesEtapa.Pesquisar.idGrid, "ConciliacaoTransportador/PesquisaCtes", _detalhesEtapa, menuOpcoes, configExportacao);
    _gridCtes.CarregarGrid();

    _gridCtesAssinatura = new GridView(_assinarEtapa.GridCtes.idGrid, "ConciliacaoTransportador/PesquisaCtes", _assinarEtapa);
    _gridCtesAssinatura.CarregarGrid();
}


function verDetalhesConciliacao(ConciliacaoGrid) {
    limparCamposConciliacao();
    _detalhesEtapa.Codigo.val(ConciliacaoGrid.Codigo);
    _assinarEtapa.Codigo.val(ConciliacaoGrid.Codigo);

    BuscarPorCodigo(_detalhesEtapa, "ConciliacaoTransportador/BuscarPorCodigo", function (arg) {
        _pesquisaConciliacao.ExibirFiltros.visibleFade(false);
        _assinarEtapa.BaixarCarta.visible(arg.Data.Assinada);
        _assinarEtapa.Assinar.visible(arg.Data.UsuarioAtualPodeAssinar && !arg.Data.Assinada);

        liberarEtapas(arg.Data);
    }, null);

    buscarCtes();
    popularTermo(ConciliacaoGrid.Codigo);
    CarregarResumoPeriodo(ConciliacaoGrid.Codigo, 'resumoPeriodoDetalhes');
    CarregarResumoPeriodo(ConciliacaoGrid.Codigo, 'resumoPeriodoAssinatura', true);
    $("#detalhes").show();
}

function limparCamposConciliacao() {
    LimparCampos(_detalhesEtapa);
    LimparCampos(_assinarEtapa);
}

function verDetalhesCte(cte) {
    _detalhesCte.Codigo.val(cte.Codigo);
    _gridAcrescimoDecrescimo = new GridView(_detalhesCte.AcrescimosDecrescimos.idGrid, "ConciliacaoTransportador/PesquisaAcrescimoDecrescimo", _detalhesCte);
    _gridAcrescimoDecrescimo.CarregarGrid(() => {
        Global.abrirModal('divModalDetalhesCte');
    });
}

function assinarClick() {
    executarReST("ConciliacaoTransportador/AssinarAnuencia", { Codigo: _detalhesEtapa.Codigo.val() }, function (arg) {

        if (arg.Success) {
            if (arg.Data) {
                verDetalhesConciliacao({ Codigo: _detalhesEtapa.Codigo.val() });
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Anuência assinada com sucesso.");
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function baixarCartaClick() {
    executarDownload("ConciliacaoTransportador/DownloadAnuencia", { Codigo: _detalhesEtapa.Codigo.val() });
}

function liberarEtapas(conciliacao) {
    if (conciliacao.Fechada) {
        $("#" + _etapaConciliacaoTransporte.Etapa1.idTab + " .step").attr("class", "step green");

        if (conciliacao.Assinada) {
            habilitarEtapa2(true, "green");
        } else if (conciliacao.DisponivelParaAssinatura) {
            habilitarEtapa2(true, "yellow");
        } else {
            habilitarEtapa2(false);
        }
    } else {
        $("#" + _etapaConciliacaoTransporte.Etapa1.idTab + " .step").attr("class", "step yellow");
        habilitarEtapa2(false);
    }
}

function habilitarEtapa2(habilitar, color) {
    if (habilitar) {
        $("#" + _etapaConciliacaoTransporte.Etapa2.idTab).attr("data-bs-toggle", "tab");
        $("#" + _etapaConciliacaoTransporte.Etapa2.idTab + " .step").attr("class", "step " + color);
    } else {
        $("#" + _etapaConciliacaoTransporte.Etapa2.idTab).removeAttr("data-bs-toggle");
        $("#" + _etapaConciliacaoTransporte.Etapa2.idTab + " .step").attr("class", "step");
    }
}

