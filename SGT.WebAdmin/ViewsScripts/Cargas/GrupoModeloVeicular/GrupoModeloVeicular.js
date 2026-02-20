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
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/GrupoModeloVeicular.js" />


//*******MAPEAMENTO KNOUCKOUT*******


var _gridGrupoModeloVeicular;
var _grupoModeloVeicular;
var _pesquisaGrupoModeloVeicular;


var PesquisaGrupoModeloVeicular = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Situação: " });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridGrupoModeloVeicular.CarregarGrid();
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

var grupoModeloVeicular = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", issue: 586, required: true });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Situação: ", issue: 557 });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//*******EVENTOS*******


function loadGrupoModeloVeicular() {

    _grupoModeloVeicular = new grupoModeloVeicular();
    KoBindings(_grupoModeloVeicular, "knockoutCadastroGrupoModeloVeicular");

    HeaderAuditoria("GrupoModeloVeicular", _grupoModeloVeicular);

    _pesquisaGrupoModeloVeicular = new PesquisaGrupoModeloVeicular();
    KoBindings(_pesquisaGrupoModeloVeicular, "knockoutPesquisaGrupoModeloVeicular", false, _pesquisaGrupoModeloVeicular.Pesquisar.id);

    buscarGrupoModeloVeicular();
}

function adicionarClick(e, sender) {
    Salvar(e, "GrupoModeloVeicular/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "sucesso", "cadastrado");
                _gridGrupoModeloVeicular.CarregarGrid();
                limparCamposGrupoModeloVeicular();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(e, "GrupoModeloVeicular/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "sucesso", "Atualizado com sucesso");
                _gridGrupoModeloVeicular.CarregarGrid();
                limparCamposGrupoModeloVeicular();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, sender);
}


function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o grupo " + _grupoModeloVeicular.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_grupoModeloVeicular, "GrupoModeloVeicular/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridGrupoModeloVeicular.CarregarGrid();
                limparCamposGrupoModeloVeicular();
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposGrupoModeloVeicular();
}

//*******MÉTODOS*******


function buscarGrupoModeloVeicular() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarGrupoModeloVeicular, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridGrupoModeloVeicular = new GridView(_pesquisaGrupoModeloVeicular.Pesquisar.idGrid, "GrupoModeloVeicular/Pesquisa", _pesquisaGrupoModeloVeicular, menuOpcoes, null);
    _gridGrupoModeloVeicular.CarregarGrid();
}

function editarGrupoModeloVeicular(grupoModeloVeicularGrid) {
    limparCamposGrupoModeloVeicular();
    _grupoModeloVeicular.Codigo.val(grupoModeloVeicularGrid.Codigo);
    BuscarPorCodigo(_grupoModeloVeicular, "GrupoModeloVeicular/BuscarPorCodigo", function (arg) {
        _pesquisaGrupoModeloVeicular.ExibirFiltros.visibleFade(false);
        _grupoModeloVeicular.Atualizar.visible(true);
        _grupoModeloVeicular.Cancelar.visible(true);
        _grupoModeloVeicular.Excluir.visible(true);
        _grupoModeloVeicular.Adicionar.visible(false);
    }, null);
}

function limparCamposGrupoModeloVeicular() {
    _grupoModeloVeicular.Atualizar.visible(false);
    _grupoModeloVeicular.Cancelar.visible(false);
    _grupoModeloVeicular.Excluir.visible(false);
    _grupoModeloVeicular.Adicionar.visible(true);
    LimparCampos(_grupoModeloVeicular);
}
