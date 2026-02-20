/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _franquiaPorfaixaKm;
var _faixaKmFranquiaCadastro;
var _crudFaixaKmFranquiaCadastro;
var _gridFaixaKmFranquia;

/*
 * Declaração das Classes
 */

var CRUDFaixaKmFranquiaCadastro = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarFaixaKmFranquiaClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarFaixaKmFranquiaClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirFaixaKmFranquiaClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

var FaixaKmFranquiaCadastro = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.QuilometragemInicial = PropertyEntity({ val: ko.observable(""), text: "*KM Inicial:", def: "", getType: typesKnockout.int, required: true, configInt: { precision: 0, allowZero: true }, maxlength: 11 });
    this.QuilometragemFinal = PropertyEntity({ val: ko.observable(""), text: "*KM Final:", def: "", getType: typesKnockout.int, required: true, configInt: { precision: 0, allowZero: false }, maxlength: 11 });
    this.ValorPorQuilometro = PropertyEntity({ val: ko.observable(""), text: "*Valor por KM:", def: "", getType: typesKnockout.decimal, required: true });
}

var FranquiaPorFaixaKm = function () {
    this.ListaFaixaKmFranquia = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), idBtnSearch: guid() });

    this.Adicionar = PropertyEntity({ eventClick: adicionarFaixaKmFranquiaModalClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadFranquiaPorFaixaKm() {
    _franquiaPorfaixaKm = new FranquiaPorFaixaKm();
    KoBindings(_franquiaPorfaixaKm, "knockoutFranquiaPorFaixaKm");

    _faixaKmFranquiaCadastro = new FaixaKmFranquiaCadastro();
    KoBindings(_faixaKmFranquiaCadastro, "knockoutFaixaKmFranquiaCadastro");

    _crudFaixaKmFranquiaCadastro = new CRUDFaixaKmFranquiaCadastro();
    KoBindings(_crudFaixaKmFranquiaCadastro, "knockoutCRUDFaixaKmFranquiaCadastro");

    loadGridFaixaKmFranquia();
}

function loadGridFaixaKmFranquia() {
    var ordenacao = { column: 1, dir: orderDir.asc }
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [{ descricao: "Editar", id: guid(), metodo: editarFaixaKmFranquiaClick, tamanho: 20 }] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "QuilometragemInicial", visible: false },
        { data: "QuilometragemFinal", visible: false },
        { data: "Descricao", title: "Faixa de KM", width: "50%" },
        { data: "ValorPorQuilometro", title: "Valor por KM", width: "30%" }
    ];

    _gridFaixaKmFranquia = new BasicDataTable(_franquiaPorfaixaKm.ListaFaixaKmFranquia.idGrid, header, menuOpcoes, ordenacao);
    _gridFaixaKmFranquia.CarregarGrid([]);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarFaixaKmFranquiaClick() {
    if (!validarFaixaKmFranquia())
        return;

    _franquiaPorfaixaKm.ListaFaixaKmFranquia.val().push(obterFaixaKmFranquiaSalvar());

    recarregarGridFaixaKmFranquia();
    fecharModalCadastroFaixaKmFranquia();
}

function adicionarFaixaKmFranquiaModalClick() {
    _faixaKmFranquiaCadastro.Codigo.val(guid());

    controlarBotoesFaixaKmFranquiaCadastroHabilitados(false);
    exibirModalCadastroFaixaKmFranquia();
}

function atualizarFaixaKmFranquiaClick() {
    if (!validarFaixaKmFranquia())
        return;

    var listaFaixaKmFranquia = obterListaFaixaKmFranquia();

    for (var i = 0; i < listaFaixaKmFranquia.length; i++) {
        if (_faixaKmFranquiaCadastro.Codigo.val() == listaFaixaKmFranquia[i].Codigo) {
            listaFaixaKmFranquia.splice(i, 1, obterFaixaKmFranquiaSalvar());
            break;
        }
    }

    _franquiaPorfaixaKm.ListaFaixaKmFranquia.val(listaFaixaKmFranquia);

    recarregarGridFaixaKmFranquia();
    fecharModalCadastroFaixaKmFranquia();
}

function editarFaixaKmFranquiaClick(registroSelecionado) {
    var faixaKmFranquia = obterFaixaKmFranquiaPorCodigo(registroSelecionado.Codigo);

    if (faixaKmFranquia) {
        PreencherObjetoKnout(_faixaKmFranquiaCadastro, { Data: faixaKmFranquia });

        controlarBotoesFaixaKmFranquiaCadastroHabilitados(true);
        exibirModalCadastroFaixaKmFranquia();
    }
}

function excluirFaixaKmFranquiaClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a faixa de KM?", function () {
        var listaFaixaKmFranquia = obterListaFaixaKmFranquia();

        for (var i = 0; i < listaFaixaKmFranquia.length; i++) {
            if (_faixaKmFranquiaCadastro.Codigo.val() == listaFaixaKmFranquia[i].Codigo)
                listaFaixaKmFranquia.splice(i, 1);
        }

        _franquiaPorfaixaKm.ListaFaixaKmFranquia.val(listaFaixaKmFranquia);

        recarregarGridFaixaKmFranquia();
        fecharModalCadastroFaixaKmFranquia();
    });
}

/*
 * Declaração das Funções Públicas
 */

function controlarEdicaoFranquiaPorFaixaKm() {
    controlarCamposHabilitadosPorKnockout(_franquiaPorfaixaKm, !_CAMPOS_BLOQUEADOS);

    _franquiaPorfaixaKm.Adicionar.visible(!_CAMPOS_BLOQUEADOS);
}

function preencherFranquiaPorFaixaKm(faixasKmFranquia) {
    _franquiaPorfaixaKm.ListaFaixaKmFranquia.val(faixasKmFranquia);

    recarregarGridFaixaKmFranquia();
}

function preencherFranquiaPorFaixaKmSalvar(contratoFreteTransportador) {
    contratoFreteTransportador["ListaFaixaKmFranquia"] = obterListaFaixaKmFranquiaSalvar();
}

function limparCamposFranquiaPorFaixaKm() {
    _franquiaPorfaixaKm.ListaFaixaKmFranquia.val([]);

    recarregarGridFaixaKmFranquia();
}

/*
 * Declaração das Funções Privadas
 */

function controlarBotoesFaixaKmFranquiaCadastroHabilitados(isEdicao) {
    _crudFaixaKmFranquiaCadastro.Atualizar.visible(isEdicao);
    _crudFaixaKmFranquiaCadastro.Excluir.visible(isEdicao);
    _crudFaixaKmFranquiaCadastro.Adicionar.visible(!isEdicao);
}

function exibirModalCadastroFaixaKmFranquia() {
    Global.abrirModal('divModalCadastroFaixaKmFranquia');
    $("#divModalCadastroFaixaKmFranquia").one('hidden.bs.modal', function () {
        limparCamposFaixaKmFranquiaCadastro();
    });
}

function fecharModalCadastroFaixaKmFranquia() {
    Global.fecharModal('divModalCadastroFaixaKmFranquia');
}

function limparCamposFaixaKmFranquiaCadastro() {
    LimparCampos(_faixaKmFranquiaCadastro);
}

function obterFaixaKmFranquiaPorCodigo(codigo) {
    var listaFaixaKmFranquia = obterListaFaixaKmFranquia();

    for (var i = 0; i < listaFaixaKmFranquia.length; i++) {
        var faixaKmFranquia = listaFaixaKmFranquia[i];

        if (codigo == faixaKmFranquia.Codigo)
            return faixaKmFranquia;
    }

    return undefined;
}

function obterFaixaKmFranquiaSalvar() {
    return {
        Codigo: _faixaKmFranquiaCadastro.Codigo.val(),
        Descricao: "De " + _faixaKmFranquiaCadastro.QuilometragemInicial.val() + " até " + _faixaKmFranquiaCadastro.QuilometragemFinal.val() + " quilômetros",
        QuilometragemInicial: _faixaKmFranquiaCadastro.QuilometragemInicial.val(),
        QuilometragemFinal: _faixaKmFranquiaCadastro.QuilometragemFinal.val(),
        ValorPorQuilometro: _faixaKmFranquiaCadastro.ValorPorQuilometro.val()
    };
}

function obterListaFaixaKmFranquia() {
    return _franquiaPorfaixaKm.ListaFaixaKmFranquia.val().slice();
}

function obterListaFaixaKmFranquiaSalvar() {
    var listaFaixaKmFranquia = obterListaFaixaKmFranquia();
    var listaFaixaKmFranquiaSalvar = new Array();

    for (var i = 0; i < listaFaixaKmFranquia.length; i++) {
        var faixaKmFranquia = listaFaixaKmFranquia[i];
        
        listaFaixaKmFranquiaSalvar.push({
            Codigo: faixaKmFranquia.Codigo,
            QuilometragemInicial: Globalize.parseInt(faixaKmFranquia.QuilometragemInicial),
            QuilometragemFinal: Globalize.parseInt(faixaKmFranquia.QuilometragemFinal),
            ValorPorQuilometro: faixaKmFranquia.ValorPorQuilometro
        });
    }

    return JSON.stringify(listaFaixaKmFranquiaSalvar);
}

function recarregarGridFaixaKmFranquia() {
    var listaFaixaKmFranquia = obterListaFaixaKmFranquia();

    _gridFaixaKmFranquia.CarregarGrid(listaFaixaKmFranquia, !_CAMPOS_BLOQUEADOS);
}

function validarFaixaKmFranquia() {
    if (!ValidarCamposObrigatorios(_faixaKmFranquiaCadastro)) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
        return false;
    }

    var quilometragemInicial = Globalize.parseInt(_faixaKmFranquiaCadastro.QuilometragemInicial.val());
    var quilometragemFinal = Globalize.parseInt(_faixaKmFranquiaCadastro.QuilometragemFinal.val());

    if (quilometragemFinal <= quilometragemInicial) {
        exibirMensagem(tipoMensagem.atencao, "Dados Inválidos", "O KM final deve ser maior que o inicial!");
        return false;
    }

    var listaFaixaKmFranquia = obterListaFaixaKmFranquia();

    for (var i = 0; i < listaFaixaKmFranquia.length; i++) {
        var faixaKmFranquia = listaFaixaKmFranquia[i];

        if (faixaKmFranquia.Codigo == _faixaKmFranquiaCadastro.Codigo.val())
            continue;

        var quilometragemInicialCadastrada = Globalize.parseInt(faixaKmFranquia.QuilometragemInicial);
        var quilometragemFinalCadastrada = Globalize.parseInt(faixaKmFranquia.QuilometragemFinal);

        if (
            (quilometragemInicial >= quilometragemInicialCadastrada && quilometragemInicial <= quilometragemFinalCadastrada) ||
            (quilometragemFinal >= quilometragemInicialCadastrada && quilometragemFinal <= quilometragemFinalCadastrada) ||
            (quilometragemInicialCadastrada >= quilometragemInicial && quilometragemInicialCadastrada <= quilometragemFinal) ||
            (quilometragemFinalCadastrada >= quilometragemInicial && quilometragemFinalCadastrada <= quilometragemFinal)
        ) {
            exibirMensagem(tipoMensagem.atencao, "Registro Duplicado", "Já existe um cadastro que contém a faixa de KM informada!");
            return false;
        }
    }

    return true;
}
