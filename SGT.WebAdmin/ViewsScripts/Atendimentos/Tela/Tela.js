/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/Sistema.js" />
/// <reference path="../../Consultas/Modulo.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridTela;
var _tela;
var _pesquisaTela;

var PesquisaTela = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Status = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Status: " });
    this.Sistema = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Sistema:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Modulo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Módulo:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridTela.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var Tela = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: true, maxlength: 300 });
    this.Status = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Status: " });
    this.Sistema = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Sistema:", idBtnSearch: guid(), visible: ko.observable(true), required: true });
    this.Modulo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Módulo:", idBtnSearch: guid(), visible: ko.observable(true), required: true });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//*******EVENTOS*******

function loadTela() {
    _tela = new Tela();
    KoBindings(_tela, "knockoutCadastroTela");

    _pesquisaTela = new PesquisaTela();
    KoBindings(_pesquisaTela, "knockoutPesquisaTela", false, _pesquisaTela.Pesquisar.id);

    new BuscarSistemas(_pesquisaTela.Sistema);
    new BuscarModulos(_pesquisaTela.Modulo);
    new BuscarSistemas(_tela.Sistema, RetornoSelecaoSistema);
    new BuscarModulos(_tela.Modulo, RetornoSelecaoModulo, null, _tela.Sistema);

    buscarTelas();
}

function RetornoSelecaoSistema(data) {
    _tela.Sistema.codEntity(data.Codigo);
    _tela.Sistema.val(data.Descricao);
    LimparCampoEntity(_tela.Modulo);
}

function RetornoSelecaoModulo(data) {
    if (data.CodigoSistema > 0) {
        LimparCampoEntity(_tela.Sistema);
        _tela.Sistema.codEntity(data.CodigoSistema);
        _tela.Sistema.val(data.Sistema);
    }
    _tela.Modulo.codEntity(data.Codigo);
    _tela.Modulo.val(data.Descricao);
}

function adicionarClick(e, sender) {
    Salvar(e, "Tela/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridTela.CarregarGrid();
                limparCamposTela();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(e, "Tela/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridTela.CarregarGrid();
                limparCamposTela();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, sender);
}


function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a Tela " + _tela.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_tela, "Tela/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridTela.CarregarGrid();
                limparCamposTela();
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposTela();
}

//*******MÉTODOS*******

function buscarTelas() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarTela, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridTela = new GridView(_pesquisaTela.Pesquisar.idGrid, "Tela/Pesquisa", _pesquisaTela, menuOpcoes, null);
    _gridTela.CarregarGrid();
}

function editarTela(telaGrid) {
    limparCamposTela();
    _tela.Codigo.val(telaGrid.Codigo);
    BuscarPorCodigo(_tela, "Tela/BuscarPorCodigo", function (arg) {
        _pesquisaTela.ExibirFiltros.visibleFade(false);
        _tela.Atualizar.visible(true);
        _tela.Cancelar.visible(true);
        _tela.Excluir.visible(true);
        _tela.Adicionar.visible(false);
    }, null);
}

function limparCamposTela() {
    _tela.Atualizar.visible(false);
    _tela.Cancelar.visible(false);
    _tela.Excluir.visible(false);
    _tela.Adicionar.visible(true);
    LimparCampos(_tela);
}
