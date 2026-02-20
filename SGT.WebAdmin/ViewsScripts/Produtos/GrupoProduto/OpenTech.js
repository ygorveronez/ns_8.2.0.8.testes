/// <reference path="../../Enumeradores/EnumTipoIntegracao.js" />
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

var _openTech;

var OpenTech = function () {
    this.ProdutoValorMaiorOpenTech = PropertyEntity({ val: ko.observable(""), options: ko.observable([]), def: "", text: Localization.Resources.Produtos.GrupoProduto.ProdutoParaValorMaior.getFieldDescription() });
    this.ProdutoValorMenorOpenTech = PropertyEntity({ val: ko.observable(""), options: ko.observable([]), def: "", text: Localization.Resources.Produtos.GrupoProduto.ProdutoParaValorMenor.getFieldDescription() });

    this.ProdutoValorMaiorOpenTech.val.subscribe(function (novoValor) {
        _grupoProduto.ProdutoValorMaiorOpenTech.val(novoValor);
    });

    this.ProdutoValorMenorOpenTech.val.subscribe(function (novoValor) {
        _grupoProduto.ProdutoValorMenorOpenTech.val(novoValor);
    });
}

//*******EVENTOS*******

function loadOpenTech() {
    _openTech = new OpenTech();
    KoBindings(_openTech, "knockoutOpenTech");
}

//*******MÉTODOS*******

function limparCamposOpenTech() {
    if (_openTech != null) {
        LimparCampos(_openTech);
    }
}
