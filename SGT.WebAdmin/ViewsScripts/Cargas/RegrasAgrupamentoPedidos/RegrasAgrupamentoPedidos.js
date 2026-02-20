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
/// <reference path="OutrosCodigosIntegracao.js" />
/// <reference path="GestaPatio.js" />
/// <reference path="../../Consultas/Localidade.js" />


//*******MAPEAMENTO KNOUCKOUT*******


var _gridRegrasAgrupamentoPedidos;
var _regrasAgrupamentoPedidos;
var _pesquisaRegrasAgrupamentoPedidos;
var _CRUDRegrasAgrupamentoPedidos;

var PesquisaRegrasAgrupamentoPedidos = function () {
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:",issue: 70, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Situação: ",issue: 556 });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridRegrasAgrupamentoPedidos.CarregarGrid();
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

var RegrasAgrupamentoPedidos = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", issue: 70, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Situação: ", issue: 557 });

    this.RaioKMEntreCidades = PropertyEntity({ text: "Raio de KM entre as Cidades de Destino:",issue: 1287, required: false, getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: '' }, val: ko.observable(""), def: "", maxlength: 3 });
    this.ToleranciaDiasDiferenca = PropertyEntity({ text: "Tolerância de dias de carregamento entre os pedidos:",issue: 1288, required: false, getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: '' }, val: ko.observable(""), def: "", maxlength: 2 });
    this.NumeroMaximoEntregas = PropertyEntity({ text: "Número máximo de entregas por Carga:",issue: 1289, required: false, getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: '' }, val: ko.observable(""), def: "", maxlength: 3 });
    
}

var CRUDRegrasAgrupamentoPedidos = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//*******EVENTOS*******


function loadRegrasAgrupamentoPedidos() {

    _regrasAgrupamentoPedidos = new RegrasAgrupamentoPedidos();
    KoBindings(_regrasAgrupamentoPedidos, "knockoutCadastroRegrasAgrupamentoPedidos");

    HeaderAuditoria("RegrasAgrupamentoPedidos", _regrasAgrupamentoPedidos);

    _CRUDRegrasAgrupamentoPedidos = new CRUDRegrasAgrupamentoPedidos();
    KoBindings(_CRUDRegrasAgrupamentoPedidos, "knockoutCRUDRegrasAgrupamentoPedidos");

    _pesquisaRegrasAgrupamentoPedidos = new PesquisaRegrasAgrupamentoPedidos();
    KoBindings(_pesquisaRegrasAgrupamentoPedidos, "knockoutPesquisaRegrasAgrupamentoPedidos", false, _pesquisaRegrasAgrupamentoPedidos.Pesquisar.id);
    new BuscarFilial(_regrasAgrupamentoPedidos.Filial);
    new BuscarFilial(_pesquisaRegrasAgrupamentoPedidos.Filial);

    buscarRegrasAgrupamentoPedidos();
    
}


function adicionarClick(e, sender) {
    Salvar(_regrasAgrupamentoPedidos, "RegrasAgrupamentoPedidos/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "sucesso", "cadastrado");
                _gridRegrasAgrupamentoPedidos.CarregarGrid();
                limparCamposRegrasAgrupamentoPedidos();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_regrasAgrupamentoPedidos, "RegrasAgrupamentoPedidos/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "sucesso", "Atualizado com sucesso");
                _gridRegrasAgrupamentoPedidos.CarregarGrid();
                limparCamposRegrasAgrupamentoPedidos();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, sender);
}


function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir essa regra de agrupamento para os pedidos?", function () {
        ExcluirPorCodigo(_regrasAgrupamentoPedidos, "RegrasAgrupamentoPedidos/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridRegrasAgrupamentoPedidos.CarregarGrid();
                limparCamposRegrasAgrupamentoPedidos();
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposRegrasAgrupamentoPedidos();
}

//*******MÉTODOS*******


function buscarRegrasAgrupamentoPedidos() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarRegrasAgrupamentoPedidos, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridRegrasAgrupamentoPedidos = new GridView(_pesquisaRegrasAgrupamentoPedidos.Pesquisar.idGrid, "RegrasAgrupamentoPedidos/Pesquisa", _pesquisaRegrasAgrupamentoPedidos, menuOpcoes, null);
    _gridRegrasAgrupamentoPedidos.CarregarGrid();
}

function editarRegrasAgrupamentoPedidos(regrasAgrupamentoPedidosGrid) {
    limparCamposRegrasAgrupamentoPedidos();
    _regrasAgrupamentoPedidos.Codigo.val(regrasAgrupamentoPedidosGrid.Codigo);
    BuscarPorCodigo(_regrasAgrupamentoPedidos, "RegrasAgrupamentoPedidos/BuscarPorCodigo", function (arg) {
        _pesquisaRegrasAgrupamentoPedidos.ExibirFiltros.visibleFade(false);
        _CRUDRegrasAgrupamentoPedidos.Atualizar.visible(true);
        _CRUDRegrasAgrupamentoPedidos.Cancelar.visible(true);
        _CRUDRegrasAgrupamentoPedidos.Excluir.visible(true);
        _CRUDRegrasAgrupamentoPedidos.Adicionar.visible(false);
    }, null);
}

function limparCamposRegrasAgrupamentoPedidos() {
    _CRUDRegrasAgrupamentoPedidos.Atualizar.visible(false);
    _CRUDRegrasAgrupamentoPedidos.Cancelar.visible(false);
    _CRUDRegrasAgrupamentoPedidos.Excluir.visible(false);
    _CRUDRegrasAgrupamentoPedidos.Adicionar.visible(true);
    LimparCampos(_regrasAgrupamentoPedidos);
}
