/// <reference path="../../../js/Global/Buscas.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../Consultas/ModeloDocumentoFiscal.js" />
/// <reference path="../../Enumeradores/EnumUsoMaterial.js" />

// #region Objetos Globais do Arquivo

var _crudTipoSinistro;
var _gridTipoSinistro;
var _tipoSinistro;
var _pesquisaTipoSinistro;

// #endregion Objetos Globais do Arquivo

// #region Classes

var PesquisaTipoSinistro = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", val: ko.observable(""), def: "", maxlenght: 100});
    this.Status = PropertyEntity({ text: "Status:", options: _statusPesquisa, val: ko.observable(0), def: 0 });

    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridTipoSinistro, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
}

var TipoSinistro = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", val: ko.observable(""), def: "", maxlenght: 100, required: true });
    this.Status = PropertyEntity({ text: "*Status:", val: ko.observable(1), options: _status, def: 1, enable: ko.observable(1), required: true });
}

var CrudTipoSinistro = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

// #endregion Classes

// #region Funções de Inicialização

function loadTipoSinistro() {
    _pesquisaTipoSinistro = new PesquisaTipoSinistro();
    KoBindings(_pesquisaTipoSinistro, "knockoutPesquisaTipoSinistro", false, _pesquisaTipoSinistro.Pesquisar.id);

    _tipoSinistro = new TipoSinistro();
    KoBindings(_tipoSinistro, "knockoutTipoSinistro");

    HeaderAuditoria("TipoSinistro", _tipoSinistro);

    _crudTipoSinistro = new CrudTipoSinistro();
    KoBindings(_crudTipoSinistro, "knockoutCRUD_TipoSinistro");

    loadGridTipoSinistro();
}

function loadGridTipoSinistro() {
    var opcaoEditar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: function (item) { editarClick(item); }, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };

    _gridTipoSinistro = new GridView(_pesquisaTipoSinistro.Pesquisar.idGrid, "TipoSinistro/Pesquisa", _pesquisaTipoSinistro, menuOpcoes);
    _gridTipoSinistro.CarregarGrid();
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function adicionarClick(e, sender) {
    Salvar(_tipoSinistro, "TipoSinistro/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                recarregarGridTipoSinistro();
                limparCamposTipoSinistro();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_tipoSinistro, "TipoSinistro/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                recarregarGridTipoSinistro();
                limparCamposTipoSinistro();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function cancelarClick() {
    limparCamposTipoSinistro();
}

function editarClick(registroSelecionado) {
    limparCamposTipoSinistro();

    _tipoSinistro.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_tipoSinistro, "TipoSinistro/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaTipoSinistro.ExibirFiltros.visibleFade(false);

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
        ExcluirPorCodigo(_tipoSinistro, "TipoSinistro/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    recarregarGridTipoSinistro();
                    limparCamposTipoSinistro();
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

// #region Funções Privadas

function controlarBotoesHabilitados(isEdicao) {
    _crudTipoSinistro.Atualizar.visible(isEdicao);
    _crudTipoSinistro.Excluir.visible(isEdicao);
    _crudTipoSinistro.Cancelar.visible(isEdicao);
    _crudTipoSinistro.Adicionar.visible(!isEdicao);
}

function limparCamposTipoSinistro() {
    var isEdicao = false;

    controlarBotoesHabilitados(isEdicao);
    LimparCampos(_tipoSinistro);
    exibirFiltros();
}

function recarregarGridTipoSinistro() {
    _gridTipoSinistro.CarregarGrid();
}

function exibirFiltros() {
    if (!_pesquisaTipoSinistro.ExibirFiltros.visibleFade())
        _pesquisaTipoSinistro.ExibirFiltros.visibleFade(true);
}

// #endregion Funções Privadas

