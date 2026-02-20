/// <reference path="../../../js/Global/Buscas.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../Consultas/ImpostoValorAgregado.js" />

// #region Objetos Globais do Arquivo

var _CRUDdireitoFiscal;
var _gridDireitoFiscal;
var _direitoFiscal;
var _pesquisaDireitoFiscal;

// #endregion Objetos Globais do Arquivo

// #region Classes

var PesquisaDireitoFiscal = function () {
    this.IVA = PropertyEntity({ text: "Imposto sobre Valor Agregado:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.Descricao = PropertyEntity({ text: "Descrição:", getType: typesKnockout.string, val: ko.observable(""), def: "", maxlength: 100 });
    this.Situacao = PropertyEntity({ text: "Situação:", options: _statusPesquisa, val: ko.observable(0), def: 0 });

    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", iDireitoFiscalade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridDireitoFiscal, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
}

var DireitoFiscal = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.IVA = PropertyEntity({ text: "Imposto sobre Valor Agregado:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.Descricao = PropertyEntity({ text: "Descrição:", getType: typesKnockout.string, val: ko.observable(""), def: "", maxlength: 100 });
    this.ICMS_ISS = PropertyEntity({ text: "ICMS/ISS:", getType: typesKnockout.string, val: ko.observable(""), def: "", maxlength: 50 });
    this.IPI = PropertyEntity({ text: "IPI:", getType: typesKnockout.string, val: ko.observable(""), def: "", maxlength: 50 });
    this.PIS = PropertyEntity({ text: "PIS:", getType: typesKnockout.string, val: ko.observable(""), def: "", maxlength: 50 });
    this.COFINS = PropertyEntity({ text: "COFINS:", getType: typesKnockout.string, val: ko.observable(""), def: "", maxlength: 50 });
    this.Situacao = PropertyEntity({ text: "Situação:", options: _status, val: ko.observable(true), def: true });
}

var CRUD_DireitoFiscal = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

// #endregion Classes

// #region Funções de Inicialização

function loadDireitoFiscal() {
    _pesquisaDireitoFiscal = new PesquisaDireitoFiscal();
    KoBindings(_pesquisaDireitoFiscal, "knockoutPesquisaDireitoFiscal", false, _pesquisaDireitoFiscal.Pesquisar.id);

    _direitoFiscal = new DireitoFiscal();
    KoBindings(_direitoFiscal, "knockoutDireitoFiscal");

    HeaderAuditoria("DireitoFiscal", _direitoFiscal);

    _CRUDdireitoFiscal = new CRUD_DireitoFiscal();
    KoBindings(_CRUDdireitoFiscal, "knockoutCRUD_DireitoFiscal");

    new BuscarImpostoValorAgregado(_pesquisaDireitoFiscal.IVA);
    new BuscarImpostoValorAgregado(_direitoFiscal.IVA);

    loadGridDireitoFiscal();
}

function loadGridDireitoFiscal() {
    var opcaoEditar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: function (item) { editarClick(item); }, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };

    _gridDireitoFiscal = new GridView(_pesquisaDireitoFiscal.Pesquisar.idGrid, "DireitoFiscal/Pesquisa", _pesquisaDireitoFiscal, menuOpcoes);
    _gridDireitoFiscal.CarregarGrid();
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function adicionarClick(e, sender) {
    Salvar(_direitoFiscal, "DireitoFiscal/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                recarregarGridDireitoFiscal();
                limparCamposDireitoFiscal();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_direitoFiscal, "DireitoFiscal/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                recarregarGridDireitoFiscal();
                limparCamposDireitoFiscal();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function cancelarClick() {
    limparCamposDireitoFiscal();
}

function editarClick(registroSelecionado) {
    limparCamposDireitoFiscal();
   
    _direitoFiscal.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_direitoFiscal, "DireitoFiscal/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaDireitoFiscal.ExibirFiltros.visibleFade(false);

                var isEdicao = true;

                controlarBotoesHabilitados(isEdicao);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function excluirClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse cadastro?", function () {
        ExcluirPorCodigo(_direitoFiscal, "DireitoFiscal/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    recarregarGridDireitoFiscal();
                    limparCamposDireitoFiscal();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }, null);
    });
}

function exibirFiltrosClick(e) {
    e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
}

// #endregion Funções Associadas a Eventos

// #region Funções

function controlarBotoesHabilitados(isEdicao) {
    _CRUDdireitoFiscal.Atualizar.visible(isEdicao);
    _CRUDdireitoFiscal.Excluir.visible(isEdicao);
    _CRUDdireitoFiscal.Cancelar.visible(isEdicao);
    _CRUDdireitoFiscal.Adicionar.visible(!isEdicao);
}

function limparCamposDireitoFiscal() {
    var isEdicao = false;

    controlarBotoesHabilitados(isEdicao);
    LimparCampos(_direitoFiscal);
    exibirFiltros();
}

function recarregarGridDireitoFiscal() {
    _gridDireitoFiscal.CarregarGrid();
}

function exibirFiltros() {
    if (!_pesquisaDireitoFiscal.ExibirFiltros.visibleFade())
        _pesquisaDireitoFiscal.ExibirFiltros.visibleFade(true);
}

// #endregion Funções

