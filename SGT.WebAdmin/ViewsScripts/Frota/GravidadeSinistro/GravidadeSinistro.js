/// <reference path="../../../js/Global/Buscas.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../Consultas/ModeloDocumentoFiscal.js" />
/// <reference path="../../Enumeradores/EnumUsoMaterial.js" />

// #region Objetos Globais do Arquivo

var _crudGravidadeSinistro;
var _gridGravidadeSinistro;
var _GravidadeSinistro;
var _pesquisaGravidadeSinistro;

// #endregion Objetos Globais do Arquivo

// #region Classes

var PesquisaGravidadeSinistro = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", val: ko.observable(""), def: "", maxlenght: 100 });
    this.Status = PropertyEntity({ text: "Status:", options: _statusPesquisa, val: ko.observable(0), def: 0 });

    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridGravidadeSinistro, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
}

var GravidadeSinistro = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", val: ko.observable(""), def: "", maxlenght: 100, required: true });
    this.Status = PropertyEntity({ text: "*Status:", val: ko.observable(1), options: _status, def: 1, enable: ko.observable(1), required: true });
}

var CrudGravidadeSinistro = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

// #endregion Classes

// #region Funções de Inicialização

function loadGravidadeSinistro() {
    _pesquisaGravidadeSinistro = new PesquisaGravidadeSinistro();
    KoBindings(_pesquisaGravidadeSinistro, "knockoutPesquisaGravidadeSinistro", false, _pesquisaGravidadeSinistro.Pesquisar.id);

    _GravidadeSinistro = new GravidadeSinistro();
    KoBindings(_GravidadeSinistro, "knockoutGravidadeSinistro");

    HeaderAuditoria("GravidadeSinistro", _GravidadeSinistro);

    _crudGravidadeSinistro = new CrudGravidadeSinistro();
    KoBindings(_crudGravidadeSinistro, "knockoutCRUD_GravidadeSinistro");

    loadGridGravidadeSinistro();
}

function loadGridGravidadeSinistro() {
    var opcaoEditar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: function (item) { editarClick(item); }, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };

    _gridGravidadeSinistro = new GridView(_pesquisaGravidadeSinistro.Pesquisar.idGrid, "GravidadeSinistro/Pesquisa", _pesquisaGravidadeSinistro, menuOpcoes);
    _gridGravidadeSinistro.CarregarGrid();
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function adicionarClick(e, sender) {
    Salvar(_GravidadeSinistro, "GravidadeSinistro/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                recarregarGridGravidadeSinistro();
                limparCamposGravidadeSinistro();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_GravidadeSinistro, "GravidadeSinistro/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                recarregarGridGravidadeSinistro();
                limparCamposGravidadeSinistro();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function cancelarClick() {
    limparCamposGravidadeSinistro();
}

function editarClick(registroSelecionado) {
    limparCamposGravidadeSinistro();

    _GravidadeSinistro.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_GravidadeSinistro, "GravidadeSinistro/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaGravidadeSinistro.ExibirFiltros.visibleFade(false);

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
        ExcluirPorCodigo(_GravidadeSinistro, "GravidadeSinistro/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    recarregarGridGravidadeSinistro();
                    limparCamposGravidadeSinistro();
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
    _crudGravidadeSinistro.Atualizar.visible(isEdicao);
    _crudGravidadeSinistro.Excluir.visible(isEdicao);
    _crudGravidadeSinistro.Cancelar.visible(isEdicao);
    _crudGravidadeSinistro.Adicionar.visible(!isEdicao);
}

function limparCamposGravidadeSinistro() {
    var isEdicao = false;

    controlarBotoesHabilitados(isEdicao);
    LimparCampos(_GravidadeSinistro);
    exibirFiltros();
}

function recarregarGridGravidadeSinistro() {
    _gridGravidadeSinistro.CarregarGrid();
}

function exibirFiltros() {
    if (!_pesquisaGravidadeSinistro.ExibirFiltros.visibleFade())
        _pesquisaGravidadeSinistro.ExibirFiltros.visibleFade(true);
}

// #endregion Funções Privadas

