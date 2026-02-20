/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Rest.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _cadastroCteParcial;
var _CRUDcadastroCteParcial;
var _gridCteParcial;
var _cteParcial;

/*
 * Declaração das Classes
 */

var CRUDCadastroCteParcial = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarCteParcialClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarCteParcialClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirCteParcialClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
}

var CadastroCteParcial = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Numero = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Numero.getRequiredFieldDescription(), maxlength: 10, required: true, getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" } });
}

var CteParcial = function () {
    this.ListaCteParcial = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid() });

    this.ListaCteParcial.val.subscribe(function () {
        recarregarGridCteParcial();
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarCteParcialModalClick, type: types.event, text: Localization.Resources.Pedidos.Pedido.AdicionarCTE });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridCteParcial() {
    var linhasPorPaginas = 5;
    var ordenacao = { column: 0, dir: orderDir.asc };
    var opcaoEditar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: editarCteParcialClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 20, opcoes: [opcaoEditar] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Numero", title: Localization.Resources.Gerais.Geral.Numero, width: "80%" }
    ];

    _gridCteParcial = new BasicDataTable(_cteParcial.ListaCteParcial.idGrid, header, menuOpcoes, ordenacao, null, linhasPorPaginas);
    _gridCteParcial.CarregarGrid([]);
}

function loadCteParcial() {
    _cteParcial = new CteParcial();
    KoBindings(_cteParcial, "knockoutCteParcial");

    _cadastroCteParcial = new CadastroCteParcial();
    KoBindings(_cadastroCteParcial, "knockoutCadastroCteParcial");

    _CRUDcadastroCteParcial = new CRUDCadastroCteParcial();
    KoBindings(_CRUDcadastroCteParcial, "knockoutCRUDCadastroCteParcial");

    loadGridCteParcial();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarCteParcialClick() {
    if (ValidarCamposObrigatorios(_cadastroCteParcial)) {
        if (isCteParcialDuplicado())
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.RegistroDuplicado, Localization.Resources.Pedidos.Pedido.CteInformadoJaCadastrado);
        else {
            _cteParcial.ListaCteParcial.val().push(obterCadastroCteParcialSalvar());

            recarregarGridCteParcial();
            fecharModalCadastroCteParcial();
        }
    }
    else
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
}

function adicionarCteParcialModalClick() {
    _cadastroCteParcial.Codigo.val(guid());

    controlarBotoesCadastroCteParcialHabilitados(false);

    exibirModalCadastroCteParcial();
}

function atualizarCteParcialClick() {
    if (ValidarCamposObrigatorios(_cadastroCteParcial)) {
        if (isCteParcialDuplicado())
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.RegistroDuplicado, Localization.Resources.Pedidos.Pedido.CteInformadoJaCadastrado);
        else {
            var listaCteParcial = obterListaCteParcial();

            for (var i = 0; i < listaCteParcial.length; i++) {
                if (_cadastroCteParcial.Codigo.val() == listaCteParcial[i].Codigo) {
                    listaCteParcial.splice(i, 1, obterCadastroCteParcialSalvar());
                    break;
                }
            }

            _cteParcial.ListaCteParcial.val(listaCteParcial);

            recarregarGridCteParcial();
            fecharModalCadastroCteParcial();
        }
    }
    else
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
}

function editarCteParcialClick(registroSelecionado) {
    PreencherObjetoKnout(_cadastroCteParcial, { Data: registroSelecionado });

    controlarBotoesCadastroCteParcialHabilitados(true);

    exibirModalCadastroCteParcial();
}

function excluirCteParcialClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Pedidos.Pedido.RealmenteDesejaExcluirCTE, function () {
        removerCteParcial(_cadastroCteParcial.Codigo.val());
        fecharModalCadastroCteParcial();
    });
}

/*
 * Declaração das Funções Públicas
 */

function limparCamposCteParcial() {
    preencherCteParcial([]);
}

function preencherCteParcial(dadosCteParcial) {
    _cteParcial.ListaCteParcial.val(dadosCteParcial);
}

function preencherCteParcialSalvar(pedido) {
    pedido["CtesParciais"] = obterListaCteParcialSalvar();
}

/*
 * Declaração das Funções
 */

function controlarBotoesCadastroCteParcialHabilitados(isEdicao) {
    _CRUDcadastroCteParcial.Adicionar.visible(!isEdicao);
    _CRUDcadastroCteParcial.Atualizar.visible(isEdicao);
    _CRUDcadastroCteParcial.Excluir.visible(isEdicao);
}

function exibirModalCadastroCteParcial() {
    Global.abrirModal('divModalCadastroCteParcial');
    $("#divModalCadastroCteParcial").one('hidden.bs.modal', function () {
        LimparCampos(_cadastroCteParcial);
    });
}

function fecharModalCadastroCteParcial() {
    Global.fecharModal('divModalCadastroCteParcial');
}

function isCteParcialDuplicado() {
    var listaCteParcial = obterListaCteParcial();

    for (var i = 0; i < listaCteParcial.length; i++) {
        var cteParcial = listaCteParcial[i];

        if ((_cadastroCteParcial.Codigo.val() != cteParcial.Codigo) && (_cadastroCteParcial.Numero.val() == cteParcial.Numero))
            return true;
    }

    return false;
}

function obterCadastroCteParcialSalvar() {
    return {
        Codigo: _cadastroCteParcial.Codigo.val(),
        Numero: _cadastroCteParcial.Numero.val()
    };
}

function obterListaCteParcial() {
    return _cteParcial.ListaCteParcial.val().slice();
}

function obterListaCteParcialSalvar() {
    var listaCteParcial = obterListaCteParcial();

    return JSON.stringify(listaCteParcial);
}

function recarregarGridCteParcial() {
    var listaCteParcial = obterListaCteParcial();

    _gridCteParcial.CarregarGrid(listaCteParcial);
}

function removerCteParcial(codigo) {
    var listaCteParcial = obterListaCteParcial();

    for (var i = 0; i < listaCteParcial.length; i++) {
        if (codigo == listaCteParcial[i].Codigo) {
            listaCteParcial.splice(i, 1);
            break;
        }
    }

    _cteParcial.ListaCteParcial.val(listaCteParcial);
}
