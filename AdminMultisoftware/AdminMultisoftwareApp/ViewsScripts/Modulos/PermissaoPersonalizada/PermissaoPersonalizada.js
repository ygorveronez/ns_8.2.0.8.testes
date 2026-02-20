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
/// <reference path="../../Consultas/Formulario.js" />
/// <reference path="../../Enumeradores/EnumPermissaoPersonalizada.js" />


//*******MAPEAMENTO KNOUCKOUT*******


var _gridPermissaoPersonalizada;
var _permissaoPersonalizada;
var _pesquisaPermissaoPersonalizada;

var PesquisaPermissaoPersonalizada = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Formulario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Formulário:", idBtnSearch: guid() });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridPermissaoPersonalizada.CarregarGrid();
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

var PermissaoPersonalizada = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: true, maxlength: 150 });
    this.CodigoPermissao = PropertyEntity({ val: ko.observable(EnumPermissaoPersonalizada.Acerto_PermiteLiberarAutorizarAbastecimento), options: _SelectPermissaoPersonalizada, text: "*Tipo de Permissão: ", def: EnumPermissaoPersonalizada.Acerto_PermiteLiberarAutorizarAbastecimento });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Situação: " });
    this.Formulario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Formulário:", idBtnSearch: guid() });
    this.TranslationResourcePath = PropertyEntity({ text: "Translation Resource Path: ", required: false, maxlength: 500 });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//*******EVENTOS*******


function loadPermissaoPersonalizada() {

    _permissaoPersonalizada = new PermissaoPersonalizada();
    KoBindings(_permissaoPersonalizada, "knockoutCadastroPermissaoPersonalizada");

    _pesquisaPermissaoPersonalizada = new PesquisaPermissaoPersonalizada();
    KoBindings(_pesquisaPermissaoPersonalizada, "knockoutPesquisaPermissaoPersonalizada", false, _pesquisaPermissaoPersonalizada.Pesquisar.id);

    new BuscarFormulario(_permissaoPersonalizada.Formulario);
    new BuscarFormulario(_pesquisaPermissaoPersonalizada.Formulario);

    buscarPermissaoPersonalizadas();

    HeaderAuditoria("PermissaoPersonalizada", _permissaoPersonalizada);
}

function adicionarClick(e, sender) {
    Salvar(e, "PermissaoPersonalizada/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "sucesso", "cadastrado");
                _gridPermissaoPersonalizada.CarregarGrid();
                limparCamposPermissaoPersonalizada();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(e, "PermissaoPersonalizada/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "sucesso", "Atualizado com sucesso");
                _gridPermissaoPersonalizada.CarregarGrid();
                limparCamposPermissaoPersonalizada();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, sender);
}


function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a Permissão Personalizada " + _permissaoPersonalizada.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_permissaoPersonalizada, "PermissaoPersonalizada/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridPermissaoPersonalizada.CarregarGrid();
                limparCamposPermissaoPersonalizada();
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposPermissaoPersonalizada();
}

//*******MÉTODOS*******


function buscarPermissaoPersonalizadas() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarPermissaoPersonalizada, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridPermissaoPersonalizada = new GridView(_pesquisaPermissaoPersonalizada.Pesquisar.idGrid, "PermissaoPersonalizada/Pesquisa", _pesquisaPermissaoPersonalizada, menuOpcoes, null);
    _gridPermissaoPersonalizada.CarregarGrid();
}

function editarPermissaoPersonalizada(permissaoPersonalizadaGrid) {
    limparCamposPermissaoPersonalizada();
    _permissaoPersonalizada.Codigo.val(permissaoPersonalizadaGrid.Codigo);
    BuscarPorCodigo(_permissaoPersonalizada, "PermissaoPersonalizada/BuscarPorCodigo", function (arg) {
        _pesquisaPermissaoPersonalizada.ExibirFiltros.visibleFade(false);
        _permissaoPersonalizada.Atualizar.visible(true);
        _permissaoPersonalizada.Cancelar.visible(true);
        _permissaoPersonalizada.Excluir.visible(true);
        _permissaoPersonalizada.Adicionar.visible(false);
    }, null);
}

function limparCamposPermissaoPersonalizada() {
    _permissaoPersonalizada.Atualizar.visible(false);
    _permissaoPersonalizada.Cancelar.visible(false);
    _permissaoPersonalizada.Excluir.visible(false);
    _permissaoPersonalizada.Adicionar.visible(true);
    LimparCampos(_permissaoPersonalizada);
}
