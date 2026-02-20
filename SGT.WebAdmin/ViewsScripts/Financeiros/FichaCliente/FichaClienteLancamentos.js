/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Enumeradores/EnumEntradaSaida.js" />
/// <reference path="../../Financeiros/FichaCliente.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _fichaClienteLancamentos;
var _gridFichaClienteLancamentos;

/*
 * Declaração das Classes
 */

var FichaClienteLancamentos = function () {
    this.Visible = PropertyEntity({ visible: false, idFade: guid(), visibleFade: ko.observable(false) });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Tipo = PropertyEntity({ text: "*Tipo: ", val: ko.observable(EnumEntradaSaida.Entrada), options: EnumEntradaSaida.obterOpcoes(), def: EnumEntradaSaida.Entrada });
    this.Valor = PropertyEntity({ text: "*Valor: ", val: ko.observable(0), getType: typesKnockout.decimal, maxlength: 18 });
    this.Data = PropertyEntity({ val: ko.observable(""), def: "", text: "*Data", getType: typesKnockout.date });

    this.Adicionar = PropertyEntity({ type: types.event, eventClick: AdicionarFichaClienteLancamentoClick, text: "Adicionar", visible: ko.observable(true), enable: ko.observable(true), idBtnSearch: guid() });
    this.Atualizar = PropertyEntity({ type: types.event, eventClick: AtualizarFichaClienteLancamentoClick, text: "Atualizar", visible: ko.observable(false), enable: ko.observable(false), idBtnSearch: guid() });
    this.Cancelar = PropertyEntity({ type: types.event, eventClick: LimparCamposFichaClienteLancamentosClick, text: "Cancelar", visible: ko.observable(true), enable: ko.observable(true), idBtnSearch: guid() });

    this.FichaClienteLancamentos = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), required: true, idGrid: guid() });
    this.FichaClienteLancamentos.val.subscribe(function () {
        RenderizarGridFichaClienteLancamentos();
    });
}


//*******EVENTOS*******

function LoadFichaClienteLancamentos() {

    _fichaClienteLancamentos = new FichaClienteLancamentos();
    KoBindings(_fichaClienteLancamentos, "knockoutFichaClienteLancamentos");

    LoadGridFichaClienteLancamentos();
}

function ExcluirLancamentoClick(lancamento) {
    var dataGrid = _fichaClienteLancamentos.FichaClienteLancamentos.val();

    for (var i = 0; i < dataGrid.length; i++) {
        if (lancamento.Codigo == dataGrid[i].Codigo) {
            dataGrid.splice(i, 1);
            break;
        }
    }

    _fichaClienteLancamentos.FichaClienteLancamentos.val(dataGrid);

    atualizarSaldo();
}

function LoadGridFichaClienteLancamentos() {

    // Menu
    var opcaoEditar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: function (item) { EditarLancamentoClick(item); }, tamanho: "5", icone: "" };
    var opcaoExcluir = { descricao: "Excluir", id: guid(), evento: "onclick", metodo: function (item) { ExcluirLancamentoClick(item); }, tamanho: "5", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar, opcaoExcluir] };

    // Cabecalho
    var header = [
        { data: "Codigo", visible: false },
        { data: "Tipo", title: "Tipo de Lançamento", width: "25%", className: "text-align-center" },
        { data: "Valor", title: "Valor", width: "25%", className: "text-align-center" },
        { data: "Data", title: "Data", width: "25%", className: "text-align-center" },
    ];

    // Grid
    _gridFichaClienteLancamentos = new BasicDataTable(_fichaClienteLancamentos.FichaClienteLancamentos.idGrid, header, menuOpcoes, null, null, 10);
    _gridFichaClienteLancamentos.CarregarGrid([]);
}

function EditarLancamentoClick(lancamento) {
    var dataGrid = _fichaClienteLancamentos.FichaClienteLancamentos.val();

    for (var i = 0; i < dataGrid.length; i++) {
        if (lancamento.Codigo == dataGrid[i].Codigo) {
            _fichaClienteLancamentos.Codigo.val(dataGrid[i].Codigo);
            _fichaClienteLancamentos.Tipo.val(dataGrid[i].Tipo);
            _fichaClienteLancamentos.Valor.val(dataGrid[i].Valor);
            _fichaClienteLancamentos.Data.val(dataGrid[i].Data);
            break;
        }
    }

    _fichaClienteLancamentos.Adicionar.visible(false);
    _fichaClienteLancamentos.Adicionar.enable(false);
    _fichaClienteLancamentos.Atualizar.visible(true);
    _fichaClienteLancamentos.Atualizar.enable(true);
}

function LimparCamposFichaClienteLancamentos() {
    LimparCampos(_fichaClienteLancamentos);
    _fichaClienteLancamentos.FichaClienteLancamentos.val([]);
    RenderizarGridFichaClienteLancamentos();
}

function LimparCamposFichaClienteLancamentosClick() {
    LimparCampos(_fichaClienteLancamentos);
}

function AdicionarFichaClienteLancamentoClick() {
    if (!ValidaFichaClienteLancamento())
        return;

    var dataGrid = _fichaClienteLancamentos.FichaClienteLancamentos.val();

    var lancamento = {
        Codigo: guid(),
        Tipo: _fichaClienteLancamentos.Tipo.val(),
        Valor: _fichaClienteLancamentos.Valor.val(),
        Data: _fichaClienteLancamentos.Data.val(),
    };

    dataGrid.push(lancamento);
    _fichaClienteLancamentos.FichaClienteLancamentos.val(dataGrid);

    LimparCamposFichaClienteLancamentosClick();

    atualizarSaldo();
}

function AtualizarFichaClienteLancamentoClick() {

    if (!ValidaFichaClienteLancamento())
        return;

    var dataGrid = _fichaClienteLancamentos.FichaClienteLancamentos.val();

    for (var i = 0; i < dataGrid.length; i++) {

        if (_fichaClienteLancamentos.Codigo.val() == dataGrid[i].Codigo) {
            dataGrid[i].Tipo = _fichaClienteLancamentos.Tipo.val();
            dataGrid[i].Valor = _fichaClienteLancamentos.Valor.val();
            dataGrid[i].Data = _fichaClienteLancamentos.Data.val();
            break;
        }
    }

    _fichaClienteLancamentos.Atualizar.visible(false);
    _fichaClienteLancamentos.Atualizar.enable(false);
    _fichaClienteLancamentos.Adicionar.visible(true);
    _fichaClienteLancamentos.Adicionar.enable(true);


    _fichaClienteLancamentos.FichaClienteLancamentos.val(dataGrid);

    LimparCamposFichaClienteLancamentosClick();

    atualizarSaldo();
}

//*******MÉTODOS*******



function ValidaFichaClienteLancamento() {
    var msg = "";

    if (_fichaClienteLancamentos.Valor.val() == "" || _fichaClienteLancamentos.Valor.val() == 0)
        msg = "É necessário informar o Valor";

    if (_fichaClienteLancamentos.Data.val() == "" || _fichaClienteLancamentos.Data.val() == 0)
        msg = "É necessário informar uma Data";

    if (msg != "")
        exibirMensagem(tipoMensagem.atencao, "Configuração Inválida", msg);

    return msg == "";
}

function RenderizarGridFichaClienteLancamentos() {

    var itens = _fichaClienteLancamentos.FichaClienteLancamentos.val();

    converterTipoParaGrid(itens);

    _gridFichaClienteLancamentos.CarregarGrid(itens);
}

function recarregarGridFichaClienteLancamentos(lancamentos) {

    _fichaClienteLancamentos.FichaClienteLancamentos.val(lancamentos);
    RenderizarGridFichaClienteLancamentos();
}

function converterTipoParaGrid(itens) {

    for (var i = 0; i < itens.length; i++) {
        if (itens[i].Tipo == 1)
            itens[i].Tipo = "Entrada"

        else if (itens[i].Tipo == 2)
            itens[i].Tipo = "Saída"
    }

}