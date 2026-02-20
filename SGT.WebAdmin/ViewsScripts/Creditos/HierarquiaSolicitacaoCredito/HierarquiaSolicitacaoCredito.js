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
/// <reference path="../../Consultas/Usuario.js" />


//*******MAPEAMENTO KNOUCKOUT*******


var _gridHierarquiaSolicitacaoCredito;
var _hierarquiaSolicitacaoCredito;
var _pesquisaHierarquiaSolicitacaoCredito;

var PesquisaHierarquiaSolicitacaoCredito = function () {
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridFilial.CarregarGrid();
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

var HierarquiaSolicitacaoCredito = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Solicitante = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "Operador:", idBtnSearch: guid() });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//*******EVENTOS*******


function loadHierarquiaSolicitacaoCredito() {

    _hierarquiaSolicitacaoCredito = new HierarquiaSolicitacaoCredito();
    KoBindings(_hierarquiaSolicitacaoCredito, "knockoutCadastroHierarquiaSolicitacaoCredito");

    _pesquisaHierarquiaSolicitacaoCredito = new PesquisaHierarquiaSolicitacaoCredito();
    KoBindings(_pesquisaHierarquiaSolicitacaoCredito, "knockoutPesquisaHierarquiaSolicitacaoCredito");

    HeaderAuditoria("HierarquiaSolicitacaoCredito", _hierarquiaSolicitacaoCredito);

    new BuscarOperador(_hierarquiaSolicitacaoCredito.Solicitante);
    buscarHierarquiaSolicitacaoCreditos();

}

function adicionarClick(e, sender) {
    Salvar(e, "HierarquiaSolicitacaoCredito/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "sucesso", "cadastrado");
                _gridHierarquiaSolicitacaoCredito.CarregarGrid();
                limparCamposHierarquiaSolicitacaoCredito();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(e, "HierarquiaSolicitacaoCredito/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "sucesso", "Atualizado com sucesso");
                _gridHierarquiaSolicitacaoCredito.CarregarGrid();
                limparCamposHierarquiaSolicitacaoCredito();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, sender);
}


function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir?", function () {
        ExcluirPorCodigo(_hierarquiaSolicitacaoCredito, "HierarquiaSolicitacaoCredito/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridHierarquiaSolicitacaoCredito.CarregarGrid();
                limparCamposHierarquiaSolicitacaoCredito();
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposHierarquiaSolicitacaoCredito();
}

//*******MÉTODOS*******


function buscarHierarquiaSolicitacaoCreditos() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarHierarquiaSolicitacaoCredito, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridHierarquiaSolicitacaoCredito = new GridView(_pesquisaHierarquiaSolicitacaoCredito.Pesquisar.idGrid, "HierarquiaSolicitacaoCredito/Pesquisa", _pesquisaHierarquiaSolicitacaoCredito, menuOpcoes, null);
    _gridHierarquiaSolicitacaoCredito.CarregarGrid();
}

function editarHierarquiaSolicitacaoCredito(hierarquiaSolicitacaoCreditoGrid) {
    limparCamposHierarquiaSolicitacaoCredito();
    _hierarquiaSolicitacaoCredito.Codigo.val(hierarquiaSolicitacaoCreditoGrid.Codigo);
    BuscarPorCodigo(_hierarquiaSolicitacaoCredito, "HierarquiaSolicitacaoCredito/BuscarPorCodigo", function (arg) {
        _pesquisaHierarquiaSolicitacaoCredito.ExibirFiltros.visibleFade(false);
        _hierarquiaSolicitacaoCredito.Atualizar.visible(true);
        _hierarquiaSolicitacaoCredito.Cancelar.visible(true);
        _hierarquiaSolicitacaoCredito.Excluir.visible(true);
        _hierarquiaSolicitacaoCredito.Adicionar.visible(false);
    }, null);
}

function limparCamposHierarquiaSolicitacaoCredito() {
    _hierarquiaSolicitacaoCredito.Atualizar.visible(false);
    _hierarquiaSolicitacaoCredito.Cancelar.visible(false);
    _hierarquiaSolicitacaoCredito.Excluir.visible(false);
    _hierarquiaSolicitacaoCredito.Adicionar.visible(true);
    LimparCampos(_hierarquiaSolicitacaoCredito);
}
