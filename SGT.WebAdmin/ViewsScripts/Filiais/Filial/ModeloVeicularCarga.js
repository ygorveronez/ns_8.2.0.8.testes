/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />

// #region Objetos Globais do Arquivo

var _CRUDFilialModeloVeicularCargaCadastro;
var _filialModeloVeicularCarga;
var _filialModeloVeicularCargaCadastro;
var _gridFilialModeloVeicularCarga;

// #endregion Objetos Globais do Arquivo

// #region Classes

var CRUDFilialModeloVeicularCargaCadastro = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarFilialModeloVeicularCargaClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarFilialModeloVeicularCargaClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirFilialModeloVeicularCargaClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
}

var FilialModeloVeicularCargaCadastro = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.ModeloVeicularCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: Localization.Resources.Filiais.Filial.ModeloVeicularCarga.getRequiredFieldDescription(), idBtnSearch: guid() });
    this.IntegrarOrdemEmbarque = PropertyEntity({ text: Localization.Resources.Filiais.Filial.IntegrarOrdemEmbarque, getType: typesKnockout.bool, val: ko.observable(false), def: false });
}

var FilialModeloVeicularCarga = function () {
    this.ListaFilialModeloVeicularCarga = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid() });

    this.Adicionar = PropertyEntity({ eventClick: adicionarFilialModeloVeicularCargaModalClick, type: types.event, text: Localization.Resources.Filiais.Filial.AdicionarModeloVeicularCarga });
}

// #endregion Classes

// #region Funções de Inicialização

function loadGridFilialModeloVeicularCarga() {
    var linhasPorPaginas = 5;
    var ordenacao = { column: 1, dir: orderDir.asc };
    var opcaoEditar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: editarFilialModeloVeicularCargaClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 10, opcoes: [opcaoEditar] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "ModeloVeicularCarga", title: Localization.Resources.Filiais.Filial.ModeloVeicularCarga, width: "80%", className: "text-align-left", orderable: false }
    ];

    _gridFilialModeloVeicularCarga = new BasicDataTable(_filialModeloVeicularCarga.ListaFilialModeloVeicularCarga.idGrid, header, menuOpcoes, ordenacao, null, linhasPorPaginas);
    _gridFilialModeloVeicularCarga.CarregarGrid([]);
}

function loadFilialModeloVeicularCarga() {
    _filialModeloVeicularCarga = new FilialModeloVeicularCarga();
    KoBindings(_filialModeloVeicularCarga, "knockoutFilialModeloVeicularCarga");

    _filialModeloVeicularCargaCadastro = new FilialModeloVeicularCargaCadastro();
    KoBindings(_filialModeloVeicularCargaCadastro, "knockoutFilialModeloVeicularCargaCadastro");

    _CRUDFilialModeloVeicularCargaCadastro = new CRUDFilialModeloVeicularCargaCadastro();
    KoBindings(_CRUDFilialModeloVeicularCargaCadastro, "knockoutCRUDFilialModeloVeicularCargaCadastro");

    new BuscarModelosVeicularesCarga(_filialModeloVeicularCargaCadastro.ModeloVeicularCarga);

    loadGridFilialModeloVeicularCarga();
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function adicionarFilialModeloVeicularCargaClick() {
    if (!validarFilialModeloVeicularCarga())
        return;

    _filialModeloVeicularCarga.ListaFilialModeloVeicularCarga.val().push(obterFilialModeloVeicularCargaSalvar());

    recarregarGridFilialModeloVeicularCarga();
    fecharModalFilialModeloVeicularCargaCadastro();
}

function adicionarFilialModeloVeicularCargaModalClick() {
    _filialModeloVeicularCargaCadastro.Codigo.val(guid());

    controlarBotoesFilialModeloVeicularCargaCadastroHabilitados(false);

    exibirModalFilialModeloVeicularCargaCadastro();
}

function atualizarFilialModeloVeicularCargaClick() {
    if (!validarFilialModeloVeicularCarga())
        return;

    var listaFilialModeloVeicularCarga = obterListaFilialModeloVeicularCarga();

    for (var i = 0; i < listaFilialModeloVeicularCarga.length; i++) {
        if (_filialModeloVeicularCargaCadastro.Codigo.val() == listaFilialModeloVeicularCarga[i].Codigo) {
            listaFilialModeloVeicularCarga.splice(i, 1, obterFilialModeloVeicularCargaSalvar());
            break;
        }
    }

    _filialModeloVeicularCarga.ListaFilialModeloVeicularCarga.val(listaFilialModeloVeicularCarga);

    recarregarGridFilialModeloVeicularCarga();
    fecharModalFilialModeloVeicularCargaCadastro();
}

function editarFilialModeloVeicularCargaClick(registroSelecionado) {
    var filialModeloVeicularCarga = obterFilialModeloVeicularCargaPorCodigo(registroSelecionado.Codigo);

    if (!filialModeloVeicularCarga)
        return;

    PreencherObjetoKnout(_filialModeloVeicularCargaCadastro, { Data: filialModeloVeicularCarga });

    controlarBotoesFilialModeloVeicularCargaCadastroHabilitados(true);
    exibirModalFilialModeloVeicularCargaCadastro();
}

function excluirFilialModeloVeicularCargaClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Gerais.Geral.DesejaExcluirRegistro, function () {
        var listaFilialModeloVeicularCarga = obterListaFilialModeloVeicularCarga();

        for (var i = 0; i < listaFilialModeloVeicularCarga.length; i++) {
            if (_filialModeloVeicularCargaCadastro.Codigo.val() == listaFilialModeloVeicularCarga[i].Codigo)
                listaFilialModeloVeicularCarga.splice(i, 1);
        }

        _filialModeloVeicularCarga.ListaFilialModeloVeicularCarga.val(listaFilialModeloVeicularCarga);

        recarregarGridFilialModeloVeicularCarga();
        fecharModalFilialModeloVeicularCargaCadastro();
    });
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function limparCamposFilialModeloVeicularCarga() {
    _filialModeloVeicularCarga.ListaFilialModeloVeicularCarga.val([]);

    recarregarGridFilialModeloVeicularCarga();
}

function preencherFilialModeloVeicularCarga(dadosModeloVeicularCarga) {
    _filialModeloVeicularCarga.ListaFilialModeloVeicularCarga.val(dadosModeloVeicularCarga);

    recarregarGridFilialModeloVeicularCarga();
}

function preencherFilialModeloVeicularCargaSalvar(filial) {
    filial["ListaModeloVeicularCarga"] = obterListaFilialModeloVeicularCargaSalvar();
}

// #endregion Funções Públicas

// #region Funções Privadas

function controlarBotoesFilialModeloVeicularCargaCadastroHabilitados(isEdicao) {
    _CRUDFilialModeloVeicularCargaCadastro.Adicionar.visible(!isEdicao);
    _CRUDFilialModeloVeicularCargaCadastro.Atualizar.visible(isEdicao);
    _CRUDFilialModeloVeicularCargaCadastro.Excluir.visible(isEdicao);
}

function exibirModalFilialModeloVeicularCargaCadastro() {
    Global.abrirModal('divModalFilialModeloVeicularCargaCadastro');
    $("#divModalFilialModeloVeicularCargaCadastro").one('hidden.bs.modal', function () {
        LimparCampos(_filialModeloVeicularCargaCadastro);
    });
}

function fecharModalFilialModeloVeicularCargaCadastro() {
    Global.fecharModal('divModalFilialModeloVeicularCargaCadastro');
}

function obterFilialModeloVeicularCargaPorCodigo(codigo) {
    var listaFilialModeloVeicularCarga = obterListaFilialModeloVeicularCarga();

    for (var i = 0; i < listaFilialModeloVeicularCarga.length; i++) {
        var filialModeloVeicularCarga = listaFilialModeloVeicularCarga[i];

        if (codigo == filialModeloVeicularCarga.Codigo)
            return filialModeloVeicularCarga;
    }

    return undefined;
}

function obterFilialModeloVeicularCargaSalvar() {
    return {
        Codigo: _filialModeloVeicularCargaCadastro.Codigo.val(),
        IntegrarOrdemEmbarque: _filialModeloVeicularCargaCadastro.IntegrarOrdemEmbarque.val(),
        ModeloVeicularCarga: {
            Codigo: _filialModeloVeicularCargaCadastro.ModeloVeicularCarga.codEntity(),
            Descricao: _filialModeloVeicularCargaCadastro.ModeloVeicularCarga.val()
        }
    };
}

function obterListaFilialModeloVeicularCarga() {
    return _filialModeloVeicularCarga.ListaFilialModeloVeicularCarga.val().slice();
}

function obterListaFilialModeloVeicularCargaSalvar() {
    var listaFilialModeloVeicularCarga = obterListaFilialModeloVeicularCarga();
    var listaFilialModeloVeicularCargaSalvar = new Array();

    for (var i = 0; i < listaFilialModeloVeicularCarga.length; i++) {
        var filialModeloVeicularCarga = listaFilialModeloVeicularCarga[i];

        listaFilialModeloVeicularCargaSalvar.push({
            Codigo: filialModeloVeicularCarga.Codigo,
            IntegrarOrdemEmbarque: filialModeloVeicularCarga.IntegrarOrdemEmbarque,
            ModeloVeicularCarga: filialModeloVeicularCarga.ModeloVeicularCarga.Codigo
        });
    }

    return JSON.stringify(listaFilialModeloVeicularCargaSalvar);
}

function recarregarGridFilialModeloVeicularCarga() {
    var listaFilialModeloVeicularCarga = obterListaFilialModeloVeicularCarga();
    var listaFilialModeloVeicularCargaCarregar = new Array();

    for (var i = 0; i < listaFilialModeloVeicularCarga.length; i++) {
        var filialModeloVeicularCarga = listaFilialModeloVeicularCarga[i];

        listaFilialModeloVeicularCargaCarregar.push({
            Codigo: filialModeloVeicularCarga.Codigo,
            ModeloVeicularCarga: filialModeloVeicularCarga.ModeloVeicularCarga.Descricao
        });
    }

    _gridFilialModeloVeicularCarga.CarregarGrid(listaFilialModeloVeicularCargaCarregar);
}

function validarFilialModeloVeicularCarga() {
    if (!ValidarCamposObrigatorios(_filialModeloVeicularCargaCadastro)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return false;
    }

    var listaFilialModeloVeicularCarga = obterListaFilialModeloVeicularCarga();

    for (var i = 0; i < listaFilialModeloVeicularCarga.length; i++) {
        var filialModeloVeicularCarga = listaFilialModeloVeicularCarga[i];

        if (
            (filialModeloVeicularCarga.Codigo != _filialModeloVeicularCargaCadastro.Codigo.val()) &&
            (filialModeloVeicularCarga.ModeloVeicularCarga.Codigo == _filialModeloVeicularCargaCadastro.ModeloVeicularCarga.codEntity())
        ) {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.RegistroDuplicado, Localization.Resources.Gerais.Geral.RegistroDuplicadoMensagem);
            return false;
        }
    }

    return true;
}

// #endregion Funções Privadas
