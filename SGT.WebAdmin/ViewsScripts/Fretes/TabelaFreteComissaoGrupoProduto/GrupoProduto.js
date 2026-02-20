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
/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="TabelaFrete.js" />
/// <reference path="../../Consultas/GrupoProduto.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridGrupoProduto;
var _grupoProdutoVizualizacao;
var _modalVisualizacaoProdutos;

var VisualizacaoProduto = function () {
    this.GrupoProduto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.Produto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
}

//*******EVENTOS*******

function loadGrupoProduto() {

    var menuOpcoes = {
        tipo: TypeOptionMenu.list, tamanho: 10, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                excluirGrupoProdutoClick(_tabelaFreteComissaoGrupoProduto.GrupoProduto, data);
            }
        },
        {
            descricao: "Produtos", id: guid(), metodo: function (data) {
                visualizarProdutosClick(_tabelaFreteComissaoGrupoProduto.GrupoProduto, data);
            }
        }]
    };

    var header = [{ data: "Codigo", visible: false },
                  { data: "Descricao", title: "Descrição", width: "80%" }];

    _gridGrupoProduto = new BasicDataTable(_tabelaFreteComissaoGrupoProduto.GridGrupoProduto.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarGruposProdutos(_tabelaFreteComissaoGrupoProduto.GrupoProduto, null, _gridGrupoProduto);
    _tabelaFreteComissaoGrupoProduto.GrupoProduto.basicTable = _gridGrupoProduto;

    recarregarGridGrupoProduto();

    _grupoProdutoVizualizacao = new VisualizacaoProduto();

    _modalVisualizacaoProdutos = new BuscarProdutos(_grupoProdutoVizualizacao.Produto, null, null, null, _grupoProdutoVizualizacao.GrupoProduto, false, false);
}

function recarregarGridGrupoProduto() {
    _gridGrupoProduto.CarregarGrid([]);
}

function excluirGrupoProdutoClick(knoutGrupoProduto, data) {
    var grupoProdutoGrid = knoutGrupoProduto.basicTable.BuscarRegistros();

    for (var i = 0; i < grupoProdutoGrid.length; i++) {
        if (data.Codigo == grupoProdutoGrid[i].Codigo) {
            grupoProdutoGrid.splice(i, 1);
            break;
        }
    }

    knoutGrupoProduto.basicTable.CarregarGrid(grupoProdutoGrid);
}

function visualizarProdutosClick(knoutGrupoProduto, data) {
    _grupoProdutoVizualizacao.GrupoProduto.codEntity(data.Codigo);
    _grupoProdutoVizualizacao.GrupoProduto.val(data.Descricao);

    _modalVisualizacaoProdutos.ModalBusca.AbrirModal();
    _modalVisualizacaoProdutos.Grid.CarregarGrid();
}