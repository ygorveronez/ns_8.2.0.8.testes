/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridDistribuidor;
var _distribuidorPorRegiao;
var _pesquisaDistribuidor;
var _tipoCarga;

var PesquisaDistribuidor = function () {
    this.Regiao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Região:", required: true, idBtnSearch: guid() });
    this.Distribuidor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Distribuidor:", required: true, idBtnSearch: guid() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridDistribuidor.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var DistribuidorPorRegiao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Regiao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Região:", required: true, idBtnSearch: guid() });
    this.Distribuidor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "*Distribuidor", idBtnSearch: guid() });
    this.Status = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "Status: " });
    this.TiposDeCargas = PropertyEntity({ type: types.dynamic, list: new Array(), idGrid: guid() });
};

var CRUDDistribuidorPorRegia = function () {

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
};

//*******EVENTOS*******


function loadDistribuidorPorRegiao() {
    _distribuidorPorRegiao = new DistribuidorPorRegiao();
    KoBindings(_distribuidorPorRegiao, "knockoutDistribuidorPorRegiao");

    _CRUDDistribuidorPorRegiao = new CRUDDistribuidorPorRegia();
    KoBindings(_CRUDDistribuidorPorRegiao, "knockoutCRUDCadastroDistribuidorPorRegiao");


    _pesquisaDistribuidor = new PesquisaDistribuidor();
    KoBindings(_pesquisaDistribuidor, "knockoutPesquisaDistribuidor", false, _pesquisaDistribuidor.Pesquisar.id);

    buscarDistribuidores();
    loadTipoCarga();

    new BuscarRegioes(_distribuidorPorRegiao.Regiao);
    new BuscarRegioes(_pesquisaDistribuidor.Regiao);
    new BuscarClientes(_distribuidorPorRegiao.Distribuidor);
    new BuscarClientes(_pesquisaDistribuidor.Distribuidor);
}


function atualizarClick(e, sender) {
    preencherListasTipoDeCarga();
   
    Salvar(_distribuidorPorRegiao, "DistribuidorPorRegiao/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridTipoCarga.CarregarGrid([]);
                limparCamposDistribuidorPorRegiao();
                _gridDistribuidor.CarregarGrid();
                _CRUDDistribuidorPorRegiao.Adicionar.visible(true);
                _CRUDDistribuidorPorRegiao.Atualizar.visible(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function adicionarClick(e, sender) {

    var distribuidorRegiao = obterDistribuidorPorRegiaoSalvar();
    executarReST("DistribuidorPorRegiao/Adicionar", distribuidorRegiao, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridDistribuidor.CarregarGrid();
                limparCamposDistribuidorPorRegiao();
                recarregarGridTipoDeCarga();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Atenção!", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function cancelarClick(e) {
    limparCamposDistribuidorPorRegiao();
    _CRUDDistribuidorPorRegiao.Atualizar.visible(false);
    _CRUDDistribuidorPorRegiao.Adicionar.visible(true);
}

//*******MÉTODOS*******

function buscarDistribuidores() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarClick, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridDistribuidor = new GridView(_pesquisaDistribuidor.Pesquisar.idGrid, "DistribuidorPorRegiao/Pesquisa", _pesquisaDistribuidor, menuOpcoes);
    _gridDistribuidor.CarregarGrid();

}

function editarClick(registroSelecionado) {
    limparCamposDistribuidorPorRegiao();

    _distribuidorPorRegiao.Codigo.val(registroSelecionado.Codigo);

    _CRUDDistribuidorPorRegiao.Atualizar.visible(true);
    _CRUDDistribuidorPorRegiao.Adicionar.visible(false);
    _CRUDDistribuidorPorRegiao.Cancelar.visible(true);
    BuscarPorCodigo(_distribuidorPorRegiao, "DistribuidorPorRegiao/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaDistribuidor.ExibirFiltros.visibleFade(false);
                PreencherObjetoKnout(_distribuidorPorRegiao, retorno);
                recarregarGridTipoDeCarga();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function limparCamposDistribuidorPorRegiao() {

    LimparCampos(_distribuidorPorRegiao);
}

function obterDistribuidorPorRegiaoSalvar() {
    preencherListasTipoDeCarga();

    var distribuidorRegiao = RetornarObjetoPesquisa(_distribuidorPorRegiao);

    return distribuidorRegiao;
}

function preencherListasTipoDeCarga() {
    _distribuidorPorRegiao.TiposDeCargas.val(JSON.stringify(_gridTipoCarga.BuscarRegistros()));
}
