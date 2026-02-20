/*ContainerRedex.js*/
/// <reference path="../../../js/Global/Buscas.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Charts.js" />
/// <reference path="../../../js/Global/Globais.js" />

var _gridContainerRedex;
var _pesquisaContainerRedex;
var _gridHistoricos;
var _informarEmbarqueContainer;

/*
 * Declaração das Classes
 */

var PesquisaContainerRedex = function () {
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            if (ValidarCamposObrigatorios(_pesquisaContainerRedex)) {
                _pesquisaContainerRedex.ExibirFiltros.visibleFade(false);
                _gridContainerRedex.CarregarGrid();
            }
            else
                exibirMensagemCamposObrigatorio();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.DataInicial = PropertyEntity({ text: "Data Inicial Coleta: ", getType: typesKnockout.date, val: ko.observable(null), cssClass: ko.observable("") });
    this.DataFinal = PropertyEntity({ text: "Data Final Coleta: ", getType: typesKnockout.date, val: ko.observable(null), cssClass: ko.observable("") });
    this.DataInicial.dateRangeLimit = this.DataFinalColeta;
    this.DataFinal.dateRangeInit = this.DataInicialColeta;
    this.DiasPosse = PropertyEntity({ text: "Dias em Posse: ", col: 12 });
    this.AreaRedex = PropertyEntity({ text: "Área de Redex:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.SomenteExcedentes = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Excederem o tempo de freetime", visible: ko.observable(true) });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });
};

var InformarEmbarqueContainer = function () {
    this.Codigo = PropertyEntity({ text: "Código: " });
    this.DataEmbarque = PropertyEntity({ text: "Data Embarque: ", getType: typesKnockout.date, val: ko.observable(null), cssClass: ko.observable(""), required: true });
    this.LocalEmbarque = PropertyEntity({ type: types.entity, val: ko.observable(""), codEntity: ko.observable(0), text: "*Local Embarque:", required: true, visible: ko.observable(true), idBtnSearch: guid() });

    this.Confirmar = PropertyEntity({ text: "Confirmar", type: types.event, val: ko.observable(false), eventClick: ConfirmarInformarEmbarqueClick, visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ text: "Cancelar", type: types.event, val: ko.observable(false), eventClick: CancelarInformarEmbarqueClick, visible: ko.observable(true) });
};

function loadPesquisaContainerRedex() {
    _pesquisaContainerRedex = new PesquisaContainerRedex();
    KoBindings(_pesquisaContainerRedex, "knockoutPesquisaContainer", false, _pesquisaContainerRedex.Pesquisar.id);
}

function loadInformarEmbarque() {
    _informarEmbarqueContainer = new InformarEmbarqueContainer();
    KoBindings(_informarEmbarqueContainer, "knockoutInformarEmbarque");

    new BuscarClientes(_informarEmbarqueContainer.LocalEmbarque);
}

function loadGridContainer() {
    var draggableRows = false;
    var draggableRows = false;
    var limiteRegistros = 100;
    var totalRegistrosPorPagina = 100;

    var opcaoHistoricos = { descricao: "Históricos", id: guid(), evento: "onclick", metodo: visualizarHistoricosClick, tamanho: "8", icone: "" };
    var opcaoInformarEmbarque = { descricao: "Informar Embarque", id: guid(), evento: "onclick", metodo: visualizarInformarEmbarqueClick, tamanho: "8", icone: "" };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", opcoes: [opcaoHistoricos, opcaoInformarEmbarque], tamanho: 5, };
    var configuracoesExportacao = { url: "ContainerRedex/ExportarPesquisa", titulo: "Controle Container" };

    _gridContainerRedex = new GridView("grid-containers", "ContainerRedex/Pesquisa", _pesquisaContainerRedex, menuOpcoes, null, totalRegistrosPorPagina, null, true, draggableRows, undefined, limiteRegistros, undefined, configuracoesExportacao);
    _gridContainerRedex.CarregarGrid();
}

function loadContainerRedex() {
    loadPesquisaContainerRedex();
    loadGridContainer();
    loadInformarEmbarque();

    new BuscarClientes(_pesquisaContainerRedex.AreaRedex);
}

function visualizarHistoricosClick(row) {
    $(".title-numero-container").html(row.NumeroContainer);
    ExibirModalHistorico();
    var configuracoesExportacao = { url: "ControleContainer/ExportarHistoricoColetaContainer?codigo=" + row.Codigo, titulo: "HistóricoContainer" };

    _gridHistoricos = new GridView("grid-historicos", "ControleContainer/ObterHistoricoColetaContainer?codigo=" + row.Codigo, null, null, null, 10, null, true, null, null, 10000, true, configuracoesExportacao, null, true, null, false);
    _gridHistoricos.CarregarGrid();
}

function ExibirModalHistorico() {
    Global.abrirModal('divModalHistoricos');
}

function visualizarInformarEmbarqueClick(row) {
    Global.abrirModal('divModalEmbarque');
    _informarEmbarqueContainer.Codigo.val(row.Codigo);
}

function CancelarInformarEmbarqueClick() {
    Global.fecharModal('divModalEmbarque');
}

function ConfirmarInformarEmbarqueClick() {
    if (ValidarCamposObrigatorios(_informarEmbarqueContainer)) {
        exibirConfirmacao("Confirmação", "Você realmente deseja informar o embarque do Container?", function () {
            Salvar(_informarEmbarqueContainer, "ContainerRedex/InformarEmbarqueContainer", function (retorno) {
                if (retorno.Success) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", retorno.Msg);
                    CancelarInformarEmbarqueClick();
                    _gridContainerRedex.CarregarGrid();
                } else exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
            });
        });
    }
}