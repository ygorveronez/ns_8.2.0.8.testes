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
/// <reference path="../../Consultas/CentroResultado.js" />
/// <reference path="../../Consultas/Almoxarifado.js" />
/// <reference path="Produto.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _produtoBem;

var ProdutoBem = function () {
    this.ProdutoBem = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: "Este produto é um Bem?", enable: ko.observable(true), visible: ko.observable(true) });

    this.Almoxarifado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Almoxarifado:", idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(false) });
    this.CentroResultado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Centro de Resultado:", idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(false) });

    this.ProdutoBem.val.subscribe(function (novoValor) {
        if (!novoValor) {
            limparCamposProdutoBem();
        } else if (novoValor) {
            alterarVisibleRequiredCamposProdutoBem(true);
        }
    });
}

//*******EVENTOS*******

function loadProdutoBem() {
    _produtoBem = new ProdutoBem();
    KoBindings(_produtoBem, "knockoutBemProduto");

    new BuscarCentroResultado(_produtoBem.CentroResultado);
    new BuscarAlmoxarifado(_produtoBem.Almoxarifado);
}

//*******MÉTODOS*******

function limparCamposProdutoBem() {
    LimparCampos(_produtoBem);
    alterarVisibleRequiredCamposProdutoBem(false);
}

function alterarVisibleRequiredCamposProdutoBem(val) {
    _produtoBem.Almoxarifado.visible(val);
    _produtoBem.Almoxarifado.required(val);
    _produtoBem.CentroResultado.visible(val);
    _produtoBem.CentroResultado.required(val);
}

function validaCamposObrigatoriosBem() {
    return ValidarCamposObrigatorios(_produtoBem);
}
