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


var _produtosAvariados;
var _gridProdutosAvariados;
var $modalProdutosAvariados;

var ProdutosAvariados = function () {
    this.Solicitacao = PropertyEntity({ type: types.map, getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Lote = PropertyEntity({ type: types.map, getType: typesKnockout.int, val: ko.observable(0), def: 0 });

    this.Produtos = PropertyEntity({ type: types.local, text: "Produtos da Avaria", val: ko.observable(""), idGrid: guid() });
}


//*******EVENTOS*******
function loadProdutosAvariados() {
    _produtosAvariados = new ProdutosAvariados();
    KoBindings(_produtosAvariados, "knockoutProdutosAvariados");

    $modalProdutosAvariados = $("#divModalProdutosAvariados");
}

function limparProdutosAvariadosClick(e, sender) {
    LimpaRemocaoProdutoLote();
}


//*******MÉTODOS*******
function BuscarProdutosAvariados(cb) {
    // Cria a nova tabela
    _gridProdutosAvariados = new GridView(_produtosAvariados.Produtos.idGrid, "AceiteLoteAvaria/PesquisaProdutos", _produtosAvariados);
    _gridProdutosAvariados.CarregarGrid(cb);
}

function ManutencaoAvaria(codSolicitacao, codLote) {
    _produtosAvariados.Solicitacao.val(codSolicitacao);
    _produtosAvariados.Lote.val(codLote);

    BuscarProdutosAvariados(function () {
        $modalProdutosAvariados.modal('show');
    });
}

function LimpaRemocaoProdutoLote() {
    _produtosAvariados.Codigo.val(0);
    _produtosAvariados.Motivo.val(_produtosAvariados.Motivo.def);
    _produtosAvariados.Observacao.val(_produtosAvariados.Observacao.def);
}