//*******MAPEAMENTO KNOUCKOUT*******

var _gridProdutoMotivoRejeicaoColeta;
var _produtoMotivoRejeicaoColeta;

var ProdutoMotivoRejeicaoColeta = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Produto = PropertyEntity({ type: types.event, text: "Adicionar Produto", issue: 0, idBtnSearch: guid() });
};

//*******EVENTOS*******

function LoadProdutoMotivoRejeicaoColeta() {

    _produtoMotivoRejeicaoColeta = new ProdutoMotivoRejeicaoColeta();
    KoBindings(_produtoMotivoRejeicaoColeta, "knockoutProdutoMotivoRejeicaoColeta");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirProdutoMotivoRejeicaoColetaClick(_produtoMotivoRejeicaoColeta.Produto, data);
            }
        }]
    };

    var header = [{ data: "Codigo", visible: false },
    { data: "Descricao", title: "Descrição", width: "80%" }];

    _gridProdutoMotivoRejeicaoColeta = new BasicDataTable(_produtoMotivoRejeicaoColeta.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarProdutos(_produtoMotivoRejeicaoColeta.Produto, null, null, null, null, null, null, _gridProdutoMotivoRejeicaoColeta);
    
    _produtoMotivoRejeicaoColeta.Produto.basicTable = _gridProdutoMotivoRejeicaoColeta;

    RecarregarGridProdutoMotivoRejeicaoColeta();
}

function RecarregarGridProdutoMotivoRejeicaoColeta() {
    _gridProdutoMotivoRejeicaoColeta.CarregarGrid(_motivoRejeicaoColeta.Produtos.val());
}


function ExcluirProdutoMotivoRejeicaoColetaClick(knoutProdutoMotivoRejeicaoColeta, data) {
    var produtoMotivoRejeicaoColetaGrid = knoutProdutoMotivoRejeicaoColeta.basicTable.BuscarRegistros();

    for (var i = 0; i < produtoMotivoRejeicaoColetaGrid.length; i++) {
        if (data.Codigo == produtoMotivoRejeicaoColetaGrid[i].Codigo) {
            produtoMotivoRejeicaoColetaGrid.splice(i, 1);
            break;
        }
    }

    knoutProdutoMotivoRejeicaoColeta.basicTable.CarregarGrid(produtoMotivoRejeicaoColetaGrid);
}

function LimparCamposProdutoMotivoRejeicaoColeta() {
    LimparCampos(_produtoMotivoRejeicaoColeta);
}