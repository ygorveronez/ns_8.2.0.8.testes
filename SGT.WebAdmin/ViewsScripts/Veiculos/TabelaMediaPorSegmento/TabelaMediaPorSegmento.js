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
/// <reference path="../../Consultas/SegmentoVeiculo.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _gridTabelaMediaPorSegmento;
var _tabelaMediaPorSegmento;
var _pesquisaTabelaMediaPorSegmento;

var PesquisaTabelaMediaPorSegmento = function () {
    this.Segmento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Segmento:", idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridTabelaMediaPorSegmento.CarregarGrid();
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

var TabelaMediaPorSegmento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.MediaInicial = PropertyEntity({ text: "*Media Inicial:", getType: typesKnockout.decimal, required: true });
    this.MediaFinal = PropertyEntity({ text: "*Media Final:", getType: typesKnockout.decimal, required: true });
    this.Percentual = PropertyEntity({ text: "*Percentual:", getType: typesKnockout.decimal, required: true });
    this.Segmento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Segmento:", idBtnSearch: guid(), required: ko.observable(true), visible: ko.observable(true) });
}

var CRUDTabelaMediaPorSegmento = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******


function loadTabelaMediaPorSegmento() {
    _tabelaMediaPorSegmento = new TabelaMediaPorSegmento();
    KoBindings(_tabelaMediaPorSegmento, "knockoutCadastroTabelaMediaPorSegmento");

    HeaderAuditoria("TabelaMediaPorSegmento", _tabelaMediaPorSegmento);

    _crudTabelaMediaPorSegmento = new CRUDTabelaMediaPorSegmento();
    KoBindings(_crudTabelaMediaPorSegmento, "knockoutCRUDTabelaMediaPorSegmento");

    _pesquisaTabelaMediaPorSegmento = new PesquisaTabelaMediaPorSegmento();
    KoBindings(_pesquisaTabelaMediaPorSegmento, "knockoutPesquisaTabelaMediaPorSegmento", false, _pesquisaTabelaMediaPorSegmento.Pesquisar.id);

    new BuscarSegmentoVeiculo(_tabelaMediaPorSegmento.Segmento);
    new BuscarSegmentoVeiculo(_pesquisaTabelaMediaPorSegmento.Segmento);

    buscarTabelaMediaPorSegmento();
}

function adicionarClick(e, sender) {
    Salvar(_tabelaMediaPorSegmento, "TabelaMediaPorSegmento/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridTabelaMediaPorSegmento.CarregarGrid();
                limparCamposTabelaMediaPorSegmento();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_tabelaMediaPorSegmento, "TabelaMediaPorSegmento/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridTabelaMediaPorSegmento.CarregarGrid();
                limparCamposTabelaMediaPorSegmento();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a informação da folha?", function () {
        ExcluirPorCodigo(_tabelaMediaPorSegmento, "TabelaMediaPorSegmento/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridTabelaMediaPorSegmento.CarregarGrid();
                limparCamposTabelaMediaPorSegmento();
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposTabelaMediaPorSegmento();
}

//*******MÉTODOS*******


function buscarTabelaMediaPorSegmento() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarTabelaMediaPorSegmento, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridTabelaMediaPorSegmento = new GridView(_pesquisaTabelaMediaPorSegmento.Pesquisar.idGrid, "TabelaMediaPorSegmento/Pesquisa", _pesquisaTabelaMediaPorSegmento, menuOpcoes, null);
    _gridTabelaMediaPorSegmento.CarregarGrid();
}

function editarTabelaMediaPorSegmento(tabelaMediaPorSegmentoGrid) {
    limparCamposTabelaMediaPorSegmento();
    _tabelaMediaPorSegmento.Codigo.val(tabelaMediaPorSegmentoGrid.Codigo);
    BuscarPorCodigo(_tabelaMediaPorSegmento, "TabelaMediaPorSegmento/BuscarPorCodigo", function (arg) {
        _pesquisaTabelaMediaPorSegmento.ExibirFiltros.visibleFade(false);
        _crudTabelaMediaPorSegmento.Atualizar.visible(true);
        _crudTabelaMediaPorSegmento.Cancelar.visible(true);
        _crudTabelaMediaPorSegmento.Excluir.visible(true);
        _crudTabelaMediaPorSegmento.Adicionar.visible(false);
    }, null);
}

function limparCamposTabelaMediaPorSegmento() {
    _crudTabelaMediaPorSegmento.Atualizar.visible(false);
    _crudTabelaMediaPorSegmento.Cancelar.visible(false);
    _crudTabelaMediaPorSegmento.Excluir.visible(false); 
    _crudTabelaMediaPorSegmento.Adicionar.visible(true);
    LimparCampos(_tabelaMediaPorSegmento);
}