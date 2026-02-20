/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Rest.js" />

var _cadastroBaseline;
var _CRUDBaseline;
var _gridBaseline;
var _baseline;

/*
 * Declaração das Classes
 */

var CadastroBaseline = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.TipoBaseline = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo Baseline:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Valor = PropertyEntity({ getType: typesKnockout.decimal, text: "Valor:", val: ko.observable(""), def: "", required: true, enable: ko.observable(true), maxlength: 15 });
}

var Baseline = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.ListaBaseline = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid() });

    this.ListaBaseline.val.subscribe(function () {
        recarregarGridBaseline();
    });

    this.AdicionarBaseline = PropertyEntity({ eventClick: adicionarBaselineModalClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
}

var CRUDBaseline = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarBaselineClick, type: types.event, text: "Adicionar", visible: ko.observable(true), enable: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarBaselineClick, type: types.event, text: "Atualizar", visible: ko.observable(false), enable: ko.observable(true) });
    this.Excluir = PropertyEntity({ eventClick: excluirBaselineClick, type: types.event, text: "Excluir", visible: ko.observable(false), enable: ko.observable(true) });
};

/*
 * Declaração das Funções de Inicialização
 */
function loadGridBaseline() {
    var linhasPorPaginas = 5;
    var ordenacao = { column: 0, dir: orderDir.asc };
    var opcaoEditar = { descricao: "Editar", id: guid(), metodo: editarBaselineClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 10, opcoes: [opcaoEditar] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoTipoBaseline", visible: false },
        { data: "TipoBaseline", title: "Tipo Baseline:", width: "35%" },
        { data: "Valor", title: "Valor", width: "35%" }
    ];

    _gridBaseline = new BasicDataTable(_baseline.ListaBaseline.idGrid, header, menuOpcoes, ordenacao, null, linhasPorPaginas);
    _gridBaseline.CarregarGrid([]);
}

function loadBaseline() {
    _baseline = new Baseline();
    KoBindings(_baseline, "knockoutBaseline");

    _cadastroBaseline = new CadastroBaseline();
    KoBindings(_cadastroBaseline, "knockoutCadastroBaseline");

    _CRUDBaseline = new CRUDBaseline();
    KoBindings(_CRUDBaseline, "knockoutCRUDBaseline");

    loadGridBaseline();

    new BuscarTipoBaseline(_cadastroBaseline.TipoBaseline);
}

/*
 * Declaração das Funções Associadas a Eventos
 */
function adicionarBaselineClick() {
    if (validarTipoBaselineDuplicado()) {
        _baseline.ListaBaseline.val().push(obterCadastroBaselineSalvar());
        recarregarGridBaseline();
        fecharModalCadastroBaseline();
    }
}

function adicionarBaselineModalClick() {
    _cadastroBaseline.Codigo.val(guid());

    controlarBotoesCadastroBaselineHabilitados(false);

    exibirModalCadastroBaseline();
}

function atualizarBaselineClick() {
    if (validarTipoBaselineDuplicado()) {
        let listaBaseline = obterListaBaseline();
        for (let i = 0; i < listaBaseline.length; i++) {
            if (_cadastroBaseline.Codigo.val() == listaBaseline[i].Codigo) {
                listaBaseline.splice(i, 1, obterCadastroBaselineSalvar());
                break;
            }
        }
        _baseline.ListaBaseline.val(listaBaseline);
        fecharModalCadastroBaseline();
    }
}


function validarTipoBaselineDuplicado() {
    var listaBaseline = obterListaBaseline();

    for (var i = 0; i < listaBaseline.length; i++) {
        var baseline = listaBaseline[i];

        if (baseline.TipoBaseline == _cadastroBaseline.TipoBaseline.val() && baseline.Codigo != _cadastroBaseline.Codigo.val()) {
            exibirMensagem(tipoMensagem.atencao, "Registro Duplicado.", "Tipo de Baseline já existe para essa rota.");
            return false;
        }
    }

    return true;
}

function editarBaselineClick(registroSelecionado) {
    PreencherObjetoKnout(_cadastroBaseline, { Data: registroSelecionado });
    _cadastroBaseline.TipoBaseline.val(registroSelecionado.TipoBaseline);
    _cadastroBaseline.TipoBaseline.codEntity(registroSelecionado.CodigoTipoBaseline);
    _cadastroBaseline.Valor.val(registroSelecionado.Valor);

    controlarBotoesCadastroBaselineHabilitados(true);

    exibirModalCadastroBaseline();
}

function excluirBaselineClick() {
    exibirConfirmacao("Confirmação", "Deseja realmente excluir o registro?", function () {
        removerBaseline(_cadastroBaseline.Codigo.val());
        recarregarGridBaseline();
        fecharModalCadastroBaseline();
    });
}

/*
 * Declaração das Funções Públicas
 */
function limparCamposValoresBaseline() {
    preencherBaseline([]);
}

function preencherBaseline(baseline) {
    _baseline.ListaBaseline.val(baseline);
    recarregarGridBaseline();

    _baseline.AdicionarBaseline.visible(true);
}

function preencherBaselineSalvar(contrato) {
    contrato["Baseline"] = obterListaBaselineSalvar();
}

/*
 * Declaração das Funções
 */
function controlarBotoesCadastroBaselineHabilitados(isEdicao) {
    _CRUDBaseline.Adicionar.visible(!isEdicao);
    _CRUDBaseline.Atualizar.visible(isEdicao);
    _CRUDBaseline.Excluir.visible(isEdicao);
}

function exibirModalCadastroBaseline() {
    Global.abrirModal('divModalCadastroBaseline');
    $("#divModalCadastroBaseline").one('hidden.bs.modal', function () {
        LimparCampos(_cadastroBaseline);
    });
}

function fecharModalCadastroBaseline() {
    Global.fecharModal('divModalCadastroBaseline');
}

function obterCadastroBaselineSalvar() {
    return {
        Codigo: _cadastroBaseline.Codigo.val(),
        TipoBaseline: _cadastroBaseline.TipoBaseline.val(),
        CodigoTipoBaseline: _cadastroBaseline.TipoBaseline.codEntity(),
        Valor: _cadastroBaseline.Valor.val(),
    };
}

function obterListaBaseline() {
    return _baseline.ListaBaseline.val().slice();
}

function obterListaBaselineSalvar() {
    var listaBaseline = obterListaBaseline();

    return JSON.stringify(listaBaseline);
}

function recarregarGridBaseline() {
    var listaBaseline = obterListaBaseline();
    _gridBaseline.CarregarGrid(listaBaseline);
}

function removerBaseline(codigo) {
    var listaBaseline = obterListaBaseline();

    for (var i = 0; i < listaBaseline.length; i++) {
        if (codigo == listaBaseline[i].Codigo) {
            listaBaseline.splice(i, 1);
            break;
        }
    }

    _baseline.ListaBaseline.val(listaBaseline);
}