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
/// <reference path="../../Consultas/Filial.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridDespesaFrotaPropria;
var _despesaFrotaPropria;
var _pesquisaDespesaFrotaPropria;

var PesquisaDespesaFrotaPropria = function () {
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid() });
    this.DataInicial = PropertyEntity({ text: "Data Inicial:", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final:", getType: typesKnockout.date });

    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;
    
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridDespesaFrotaPropria.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() === true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var DespesaFrotaPropria = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Filial:", idBtnSearch: guid(), required: ko.observable(true) });
    this.Data = PropertyEntity({ text: "*Data: ", getType: typesKnockout.date, required: ko.observable(true) });
    this.Valor = PropertyEntity({ text: "*Valor: ", getType: typesKnockout.decimal, required: ko.observable(true) });
    this.Observacao = PropertyEntity({ text: "Observação:", maxlength: 1000, val: ko.observable(""), enable: ko.observable(true) });        
};

var CRUDDespesaFrotaPropria = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******


function loadDespesaFrotaPropria() {
    _despesaFrotaPropria = new DespesaFrotaPropria();
    KoBindings(_despesaFrotaPropria, "knockoutCadastroDespesaFrotaPropria");

    HeaderAuditoria("DespesaFrotaPropria", _despesaFrotaPropria);

    _crudDespesaFrotaPropria = new CRUDDespesaFrotaPropria();
    KoBindings(_crudDespesaFrotaPropria, "knockoutCRUDDespesaFrotaPropria");

    _pesquisaDespesaFrotaPropria = new PesquisaDespesaFrotaPropria();
    KoBindings(_pesquisaDespesaFrotaPropria, "knockoutPesquisaDespesaFrotaPropria", false, _pesquisaDespesaFrotaPropria.Pesquisar.id);

    new BuscarFilial(_pesquisaDespesaFrotaPropria.Filial);
    new BuscarFilial(_despesaFrotaPropria.Filial);

    buscarDespesaFrotaPropria();
}

function adicionarClick(e, sender) {
    Salvar(_despesaFrotaPropria, "DespesaFrotaPropria/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridDespesaFrotaPropria.CarregarGrid();
                limparCamposDespesaFrotaPropria();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_despesaFrotaPropria, "DespesaFrotaPropria/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridDespesaFrotaPropria.CarregarGrid();
                limparCamposDespesaFrotaPropria();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a Despesa da Frota?", function () {
        ExcluirPorCodigo(_despesaFrotaPropria, "DespesaFrotaPropria/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridDespesaFrotaPropria.CarregarGrid();
                limparCamposDespesaFrotaPropria();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposDespesaFrotaPropria();
}

//*******MÉTODOS*******


function buscarDespesaFrotaPropria() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarDespesaFrotaPropria, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    var configExportacao = {
        url: "DespesaFrotaPropria/ExportarPesquisa",
        titulo: "Despesa Frota Própria"
    };

    _gridDespesaFrotaPropria = new GridViewExportacao(_pesquisaDespesaFrotaPropria.Pesquisar.idGrid, "DespesaFrotaPropria/Pesquisa", _pesquisaDespesaFrotaPropria, menuOpcoes, configExportacao);
    _gridDespesaFrotaPropria.CarregarGrid();
}

function editarDespesaFrotaPropria(despesaFrotaPropriaGrid) {
    limparCamposDespesaFrotaPropria();
    _despesaFrotaPropria.Codigo.val(despesaFrotaPropriaGrid.Codigo);
    BuscarPorCodigo(_despesaFrotaPropria, "DespesaFrotaPropria/BuscarPorCodigo", function (arg) {
        _pesquisaDespesaFrotaPropria.ExibirFiltros.visibleFade(false);
        _crudDespesaFrotaPropria.Atualizar.visible(true);
        _crudDespesaFrotaPropria.Cancelar.visible(true);
        _crudDespesaFrotaPropria.Excluir.visible(true);
        _crudDespesaFrotaPropria.Adicionar.visible(false);
    }, null);
}

function limparCamposDespesaFrotaPropria() {
    _crudDespesaFrotaPropria.Atualizar.visible(false);
    _crudDespesaFrotaPropria.Cancelar.visible(false);
    _crudDespesaFrotaPropria.Excluir.visible(false);
    _crudDespesaFrotaPropria.Adicionar.visible(true);
    LimparCampos(_despesaFrotaPropria);
}