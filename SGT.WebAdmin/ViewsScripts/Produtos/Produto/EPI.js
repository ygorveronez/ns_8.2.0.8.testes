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
/// <reference path="Produto.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _produtoEPI;

var ProdutoEPI = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.ProdutoEPI = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: "Este produto é EPI?", enable: ko.observable(true), visible: ko.observable(true) });
    this.NumeroCA = PropertyEntity({ text: "*Número CA: ", maxlength: 500, val: ko.observable(""), required: ko.observable(false), visible: ko.observable(false) });

    this.ProdutoEPI.val.subscribe(function (novoValor) {
        if (!novoValor) {
            _produtoEPI.NumeroCA.visible(false);
            _produtoEPI.NumeroCA.required(false);

        } else if (novoValor) {
            _produtoEPI.NumeroCA.visible(true);
            _produtoEPI.NumeroCA.required(true);
        }
    });
}

//*******EVENTOS*******

function loadProdutoEPI() {
    _produtoEPI = new ProdutoEPI();
    KoBindings(_produtoEPI, "knockoutEPIProduto");
}

//*******MÉTODOS*******

function limparCamposProdutoEPI() {
    LimparCampos(_produtoEPI);
    _produtoEPI.NumeroCA.visible(false);
    _produtoEPI.NumeroCA.required(false);
}

function validaCamposObrigatoriosEPI() {
    return ValidarCamposObrigatorios(_produtoEPI);
}
