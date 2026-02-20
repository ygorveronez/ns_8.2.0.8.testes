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
/// <reference path="../../Consultas/Fronteira.js" />


//*******MAPEAMENTO KNOUCKOUT*******


var _gridfronteira;
var _fronteira;
var _pesquisafronteira;


var Pesquisafronteira = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });

    this.Localidade = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Cidade:", idBtnSearch: guid() });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Situação: " });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridfronteira.CarregarGrid();
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

var fronteira = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", issue: 586, required: true });
    this.CodigoFronteiraEmbarcador = PropertyEntity({ text: "Código da fronteira no Embarcador: ", issue: 15, maxlength: 50 });
    this.CodigoAduanaDestino = PropertyEntity({ text: "Código da Aduana: ", maxlength: 50, visible: ko.observable(false) });
    this.Localidade = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Cidade da Fronteira:", issue: 16, idBtnSearch: guid() });

    this.FronteiraOutroLado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Outro Lado da Fronteira:", issue: 309, idBtnSearch: guid() });

    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Situação: ", issue: 557 });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//*******EVENTOS*******


function loadFronteira() {

    _fronteira = new fronteira();
    KoBindings(_fronteira, "knockoutCadastroFronteira");
    new BuscarFronteiras(_fronteira.FronteiraOutroLado);

    HeaderAuditoria("Fronteira", _fronteira);

    _pesquisafronteira = new Pesquisafronteira();
    KoBindings(_pesquisafronteira, "knockoutPesquisaFronteira", false, _pesquisafronteira.Pesquisar.id);

    new BuscarLocalidades(_pesquisafronteira.Localidade);
    new BuscarLocalidades(_fronteira.Localidade);
    buscarfronteiras();

}

function adicionarClick(e, sender) {
    Salvar(e, "fronteira/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "sucesso", "cadastrado");
                _gridfronteira.CarregarGrid();
                limparCamposfronteira();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(e, "fronteira/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "sucesso", "Atualizado com sucesso");
                _gridfronteira.CarregarGrid();
                limparCamposfronteira();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, sender);
}


function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a fronteira " + _fronteira.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_fronteira, "fronteira/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridfronteira.CarregarGrid();
                limparCamposfronteira();
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposfronteira();
}

//*******MÉTODOS*******


function buscarfronteiras() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarfronteira, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridfronteira = new GridView(_pesquisafronteira.Pesquisar.idGrid, "fronteira/Pesquisa", _pesquisafronteira, menuOpcoes, null);
    _gridfronteira.CarregarGrid();
}

function editarfronteira(fronteiraGrid) {
    limparCamposfronteira();
    _fronteira.Codigo.val(fronteiraGrid.Codigo);
    BuscarPorCodigo(_fronteira, "fronteira/BuscarPorCodigo", function (arg) {
        _pesquisafronteira.ExibirFiltros.visibleFade(false);
        _fronteira.Atualizar.visible(true);
        _fronteira.Cancelar.visible(true);
        _fronteira.Excluir.visible(true);
        _fronteira.Adicionar.visible(false);
    }, null);
}

function limparCamposfronteira() {
    _fronteira.Atualizar.visible(false);
    _fronteira.Cancelar.visible(false);
    _fronteira.Excluir.visible(false);
    _fronteira.Adicionar.visible(true);
    LimparCampos(_fronteira);
}
