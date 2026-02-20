/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="JanelaCarregamentoTransportador.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridPeriodoCarregamento;
var _selecaoPeriodoCarregamento;

/*
 * Declaração das Classes
 */

var SelecaoPeriodoCarregamento = function () {
    this.DadosTransporteCarga = undefined;
    this.ListaPeriodoCarregamento = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), idBtnSearch: guid() });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadSelecaoPeriodoCarregamento() {
    _selecaoPeriodoCarregamento = new SelecaoPeriodoCarregamento();
    KoBindings(_selecaoPeriodoCarregamento, "knockoutSelecaoPeriodoCarregamento");

    loadGridPeriodoCarregamento();
}

function loadGridPeriodoCarregamento() {
    var ordenacao = { column: 1, dir: orderDir.asc };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Selecionar", id: guid(), metodo: selecionarPeriodoCarregamentoClick }] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Período", width: "70%" }
    ];

    _gridPeriodoCarregamento = new BasicDataTable(_selecaoPeriodoCarregamento.ListaPeriodoCarregamento.idGrid, header, menuOpcoes, ordenacao);
    _gridPeriodoCarregamento.CarregarGrid([]);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function selecionarPeriodoCarregamentoClick(registroSelecionado) {
    _selecaoPeriodoCarregamento.DadosTransporteCarga.PeriodoCarregamento = registroSelecionado.Codigo;

    salvarDadosTransporte(_selecaoPeriodoCarregamento.DadosTransporteCarga);
    fecharModalSelecaoPeriodoCarregamento();
}

/*
 * Declaração das Funções Públicas
 */

function exibirModalSelecaoPeriodoCarregamento(dadosTransporteCargaSalvar, listaPeriodoCarregamento) {
    _selecaoPeriodoCarregamento.DadosTransporteCarga = dadosTransporteCargaSalvar;
    _selecaoPeriodoCarregamento.ListaPeriodoCarregamento.val(listaPeriodoCarregamento);

    recarregarGridListaCarregamento();

    Global.abrirModal('divModalSelecaoPeriodoCarregamento');
    $("#divModalSelecaoPeriodoCarregamento").one('hidden.bs.modal', function () {
        limparCamposSelecaoPeriodoCarregamento();
    });
}

/*
 * Declaração das Funções Privadas
 */

function fecharModalSelecaoPeriodoCarregamento() {
    Global.fecharModal('divModalSelecaoPeriodoCarregamento');
}

function limparCamposSelecaoPeriodoCarregamento() {
    _selecaoPeriodoCarregamento.DadosTransporteCarga = undefined;
    _selecaoPeriodoCarregamento.ListaPeriodoCarregamento.val([]);

    recarregarGridListaCarregamento();
}

function obterListaPeriodoCarregamento() {
    return _selecaoPeriodoCarregamento.ListaPeriodoCarregamento.val().slice();
}

function recarregarGridListaCarregamento() {
    var listaPeriodoCarregamento = obterListaPeriodoCarregamento();

    _gridPeriodoCarregamento.CarregarGrid(listaPeriodoCarregamento);
}
