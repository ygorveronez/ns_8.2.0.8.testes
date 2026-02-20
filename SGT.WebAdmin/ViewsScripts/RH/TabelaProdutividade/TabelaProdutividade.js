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

//*******MAPEAMENTO KNOUCKOUT*******

var _gridTabelaProdutividade;
var _tabelaProdutividade;
var _GRUDTabelaProdutividade;
var _pesquisaTabelaProdutividade;

var PesquisaTabelaProdutividade = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Status: " });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridTabelaProdutividade.CarregarGrid();
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

var TabelaProdutividade = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: true });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Situação: " });

    this.DadosValores = PropertyEntity({ ftype: types.listEntity, getType: typesKnockout.listEntity, def: [], val: ko.observableArray([]), codEntity: ko.observable(0) });
    this.Valores = PropertyEntity({ type: types.listEntity, getType: typesKnockout.listEntity, def: [], val: ko.observableArray([]), codEntity: ko.observable(0) });
}

var CRUDTabelaProdutividade = function () {

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//*******EVENTOS*******


function loadTabelaProdutividade() {

    _tabelaProdutividade = new TabelaProdutividade();
    KoBindings(_tabelaProdutividade, "knockoutCadastroTabelaProdutividade");

    _GRUDTabelaProdutividade = new CRUDTabelaProdutividade();
    KoBindings(_GRUDTabelaProdutividade, "knockoutCRUDTabelaProdutividade");

    _pesquisaTabelaProdutividade = new PesquisaTabelaProdutividade();
    KoBindings(_pesquisaTabelaProdutividade, "knockoutPesquisaTabelaProdutividade", false, _pesquisaTabelaProdutividade.Pesquisar.id);

    HeaderAuditoria("TabelaProdutividade", _tabelaProdutividade);

    buscarTabelaProdutividades();
    LoadValor();
    limparCamposTabelaProdutividade();
}

function adicionarClick(e, sender) {
    Salvar(_tabelaProdutividade, "TabelaProdutividade/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridTabelaProdutividade.CarregarGrid();
                limparCamposTabelaProdutividade();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_tabelaProdutividade, "TabelaProdutividade/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridTabelaProdutividade.CarregarGrid();
                limparCamposTabelaProdutividade();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, sender);
}


function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a tabela de produtividade " + _tabelaProdutividade.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_tabelaProdutividade, "TabelaProdutividade/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridTabelaProdutividade.CarregarGrid();
                limparCamposTabelaProdutividade();
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposTabelaProdutividade();
}

//*******MÉTODOS*******


function buscarTabelaProdutividades() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarTabelaProdutividade, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridTabelaProdutividade = new GridView(_pesquisaTabelaProdutividade.Pesquisar.idGrid, "TabelaProdutividade/Pesquisa", _pesquisaTabelaProdutividade, menuOpcoes, null);
    _gridTabelaProdutividade.CarregarGrid();
}

function editarTabelaProdutividade(tabelaProdutividadeGrid) {
    limparCamposTabelaProdutividade();
    _tabelaProdutividade.Codigo.val(tabelaProdutividadeGrid.Codigo);
    BuscarPorCodigo(_tabelaProdutividade, "TabelaProdutividade/BuscarPorCodigo", function (arg) {
        _pesquisaTabelaProdutividade.ExibirFiltros.visibleFade(false);

        RenderizaGridValor(_valor);

        _GRUDTabelaProdutividade.Atualizar.visible(true);
        _GRUDTabelaProdutividade.Cancelar.visible(true);
        _GRUDTabelaProdutividade.Excluir.visible(true);
        _GRUDTabelaProdutividade.Adicionar.visible(false);
    }, null);
}

function limparCamposTabelaProdutividade() {
    _GRUDTabelaProdutividade.Atualizar.visible(false);
    _GRUDTabelaProdutividade.Cancelar.visible(false);
    _GRUDTabelaProdutividade.Excluir.visible(false);
    _GRUDTabelaProdutividade.Adicionar.visible(true);
    LimparCampos(_tabelaProdutividade);
    LimparCamposValor();

    $("#tabDados").click();

    _tabelaProdutividade.Valores.list = new Array();
}