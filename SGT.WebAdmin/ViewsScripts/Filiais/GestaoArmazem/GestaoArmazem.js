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
/// <reference path="../../Consultas/Armazem.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/Produto.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridGestaoArmazem;
var _armazenamentoProduto;
var _pesquisaGestaoArmazem;
var _gridArmazenamentoProduto;

var PesquisaGestaoArmazem = function () {
    this.Filial = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Filial: ", idBtnSearch: guid() });
    this.ProdutoEmbarcador = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Produto Embarcador: ", idBtnSearch: guid() });
    this.Armazem = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Armazém: ", idBtnSearch: guid() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridGestaoArmazem.CarregarGrid();
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

var ArmazenamentoProduto = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Historico = PropertyEntity({ type: types.local, visible: ko.observable(true) });
}

//*******EVENTOS*******

function loadGestaoArmazem() {
    _pesquisaGestaoArmazem = new PesquisaGestaoArmazem();
    KoBindings(_pesquisaGestaoArmazem, "knockoutPesquisaGestaoArmazem", false, _pesquisaGestaoArmazem.Pesquisar.id);

    _armazenamentoProduto = new ArmazenamentoProduto();
    KoBindings(_armazenamentoProduto, "knockoutArmazenamentoProduto");

    BuscarFilial(_pesquisaGestaoArmazem.Filial);
    BuscarProdutos(_pesquisaGestaoArmazem.ProdutoEmbarcador, null, null, null);
    BuscarArmazem(_pesquisaGestaoArmazem.Armazem, null, null, _pesquisaGestaoArmazem.Filial);

    loadGridGestaoArmazem();
    loadGridArmazenamentoProduto();
}

function loadGridGestaoArmazem() {
    const menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [] };
    menuOpcoes.opcoes.push({ descricao: "Histórico", id: guid(), metodo: historicoClick, tamanho: "20", icone: "" });

    const configExportacao = {
        url: "GestaoArmazem/ExportarPesquisa",
        titulo: "Saldo de Produto por Armazém e Filial"
    };

    _gridGestaoArmazem = new GridViewExportacao(_pesquisaGestaoArmazem.Pesquisar.idGrid, "GestaoArmazem/Pesquisa", _pesquisaGestaoArmazem, menuOpcoes, configExportacao);
    _gridGestaoArmazem.CarregarGrid();
}

function loadGridArmazenamentoProduto() {
    _gridArmazenamentoProduto = new GridView(_armazenamentoProduto.Historico.id, "GestaoArmazem/HistoricoArmazemProduto", _armazenamentoProduto);
}

function historicoClick(dataRow) {
    _armazenamentoProduto.Codigo.val(dataRow.Codigo);
    _gridArmazenamentoProduto.CarregarGrid();
    _armazenamentoProduto.Historico.visible(true);
    Global.abrirModal("#divModalArmazenamentoProduto");
}