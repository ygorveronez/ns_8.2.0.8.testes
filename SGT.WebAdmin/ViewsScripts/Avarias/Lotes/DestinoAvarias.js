/// <reference path="Lotes.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _produtoAvaria;
var _gridProdutoAvaria;
var _CRUDDestinoAvaria;

var ProdutoAvaria = function () {
    this.Grid = PropertyEntity({ type: types.local });

    this.ProdutosAvariados = PropertyEntity({ type: types.listEntity, list: new Array(), codEntity: ko.observable(0), idGrid: guid() });
};

var CRUDDestinoAvaria = function () {
    this.Finalizar = PropertyEntity({ eventClick: FinalizarDestinoAvariaClick, type: types.event, text: "Finalizar Destino Avaria", visible: ko.observable(true) });
};

//*******EVENTOS*******

function LoadDestinoAvarias() {
    _produtoAvaria = new ProdutoAvaria();
    KoBindings(_produtoAvaria, "knockoutProdutoAvaria");

    _CRUDDestinoAvaria = new CRUDDestinoAvaria();
    KoBindings(_CRUDDestinoAvaria, "knockoutCRUDDestinoAvaria");

    LoadGridProdutoAvaria();

    LoadDestinoAvariasProduto();
}

function FinalizarDestinoAvariaClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja finalizar os destinos das avarias?", function () {
        executarReST("LotesDestinoAvaria/FinalizarDestinoAvaria", { Codigo: _lote.Codigo.val() }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Finalizado com sucesso!");
                    LimparCamposLote();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    });
}

////*******MÉTODOS*******

function CarregarDestinoAvarias() {
    _CRUDDestinoAvaria.Finalizar.visible(false);
    if (_lote.Situacao.val() === EnumSituacaoLote.Finalizada)
        _CRUDDestinoAvaria.Finalizar.visible(true);

    executarReST("LotesDestinoAvaria/BuscarProdutosAvariados", { Codigo: _lote.Codigo.val() }, function (r) {
        if (r.Success) {
            if (r.Data) {
                PreencherObjetoKnout(_produtoAvaria, r);
                RecarregarGridProdutoAvaria();
                CarregarDadosDestinoAvariasProduto();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function LoadGridProdutoAvaria() {
    var header = [
        { data: "Codigo", visible: false },
        { data: "NumeroAvaria", title: "Avaria", width: "10%" },
        { data: "ProdutoEmbarcador", title: "Produto", width: "40%" },
        { data: "NotaFiscal", title: "Nota Fiscal", width: "15%" },
        { data: "UnidadesAvariadas", title: "Un. Avariadas", width: "15%" }
    ];

    _gridProdutoAvaria = new BasicDataTable(_produtoAvaria.Grid.id, header);

    RecarregarGridProdutoAvaria();
}

function RecarregarGridProdutoAvaria() {
    var data = new Array();

    $.each(_produtoAvaria.ProdutosAvariados.list, function (i, item) {
        var itemGrid = new Object();

        itemGrid.Codigo = item.Codigo.val;
        itemGrid.NumeroAvaria = item.NumeroAvaria.val;
        itemGrid.ProdutoEmbarcador = item.ProdutoEmbarcador.val;
        itemGrid.NotaFiscal = item.NotaFiscal.val;
        itemGrid.UnidadesAvariadas = item.UnidadesAvariadas.val;

        data.push(itemGrid);
    });

    _gridProdutoAvaria.CarregarGrid(data);
}