/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Porto.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */
var _gridCargas;
var _pesquisaCarga;
var _responsavelEntrega;

var _situacaoResponsavel = [
    { text: "Todas", value: 0 },
    { text: "Com responsável", value: 1 },
    { text: "Sem responsável", value: 2 }
];

/*
 * Declaração das Classes
 */

var PesquisaCarga = function () {
    this.CodigoCargaEmbarcador = PropertyEntity({ text: "Nº da Carga:", val: ko.observable(""), def: "" });
    this.DataInicio = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataLimite = PropertyEntity({ text: "Data Limite: ", getType: typesKnockout.date });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid(), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS) });
    this.SituacaoResponsavel = PropertyEntity({ text: "Atribuição responsável: ", val: ko.observable(0), options: _situacaoResponsavel, def: 0 });
    this.ResponsavelEntrega = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Responsável Entrega:", idBtnSearch: guid() });

    this.DataInicio.dateRangeLimit = this.DataLimite;
    this.DataLimite.dateRangeInit = this.DataInicio;
  
    this.ExibirFiltros = PropertyEntity({ type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ type: types.event, eventClick: atualizarGridCargasResponsavel, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
    this.AtribuirResponsavel = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: atribuirReponsavelClick, text: "Atribuir Responsável", visible: ko.observable(false) });
    this.RemoverResponsavel = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: removerReponsavelEntregaClick, text: "Remover Responsável", visible: ko.observable(false) });
    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), text: "Marcar Todas", visible: ko.observable(true) });
};

var ResponsavelEntrega = function () {
    this.Confirmar = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: confirmarResponsavelEntregaClick, text: "Confirmar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: cancelarResponsavelEntregaClick, text: "Cancelar", visible: ko.observable(true) });
    this.ResponsavelEntrega = PropertyEntity({ type: types.entity, required: true, codEntity: ko.observable(0), text: "*Responsável Entrega:", idBtnSearch: guid() });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadResponsavelEntrega() {
    _pesquisaCarga = new PesquisaCarga();
    KoBindings(_pesquisaCarga, "knockoutPesquisaCarga");

    _responsavelEntrega = new ResponsavelEntrega();
    KoBindings(_responsavelEntrega, "knockoutResponsavelEntrega");

    new BuscarFilial(_pesquisaCarga.Filial);
    new BuscarFuncionario(_pesquisaCarga.ResponsavelEntrega);
    new BuscarFuncionario(_responsavelEntrega.ResponsavelEntrega);

    loadGridEntregaResponsavel();
    atualizarGridCargasResponsavel();
}

function loadGridEntregaResponsavel() {
    var multiplaEscolha = {
        basicGrid: null,
        eventos: {},
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _pesquisaCarga.SelecionarTodos,
        callbackNaoSelecionado: exibirMultiplasOpcoes,
        callbackSelecionado: exibirMultiplasOpcoes,
        callbackSelecionarTodos: exibirMultiplasOpcoes,
        somenteLeitura: false
    };

    var configuracaoExportacao = {
        url: "ResponsavelEntrega/ExportarPesquisa",
        titulo: "Responsável entrega"
    };

    _gridCargas = new GridView(_pesquisaCarga.Pesquisar.idGrid, "ResponsavelEntrega/Pesquisa", _pesquisaCarga, null, null, 25, null, null, null, multiplaEscolha, null, null, configuracaoExportacao);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function atribuirReponsavelClick() {
    Global.abrirModal('divModalResponsavelEntrega');
}

function confirmarResponsavelEntregaClick() {
    var data = {
        ResponsavelEntrega: _responsavelEntrega.ResponsavelEntrega.codEntity(),
        ItensSelecionados: JSON.stringify(_gridCargas.ObterMultiplosSelecionados())
    };

    executarReST("ResponsavelEntrega/AtribuirResponsavel", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                atualizarGridCargasResponsavel();
                Global.fecharModal('divModalResponsavelEntrega');

            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function removerReponsavelEntregaClick() {
    var data = {
        ResponsavelEntrega: _responsavelEntrega.ResponsavelEntrega.codEntity(),
        ItensSelecionados: JSON.stringify(_gridCargas.ObterMultiplosSelecionados())
    };
    executarReST("ResponsavelEntrega/RemoverResponsavel", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                atualizarGridCargasResponsavel();

            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function cancelarResponsavelEntregaClick() {
    Global.fecharModal('divModalResponsavelEntrega');
}

/*
 * Declaração das Funções
 */

function atualizarGridCargasResponsavel() {
    _pesquisaCarga.SelecionarTodos.val(false);
    _pesquisaCarga.AtribuirResponsavel.visible(false);
    _pesquisaCarga.RemoverResponsavel.visible(false);
    
    _gridCargas.CarregarGrid();
}

function exibirMultiplasOpcoes() {
    _pesquisaCarga.AtribuirResponsavel.visible(false);
    _pesquisaCarga.RemoverResponsavel.visible(false);
    

    var existemRegistrosSelecionados = _gridCargas.ObterMultiplosSelecionados().length > 0;
    var selecionadoTodos = _pesquisaCarga.SelecionarTodos.val();

    if (existemRegistrosSelecionados || selecionadoTodos) {
        _pesquisaCarga.AtribuirResponsavel.visible(true);
        _pesquisaCarga.RemoverResponsavel.visible(true);
    }
}
