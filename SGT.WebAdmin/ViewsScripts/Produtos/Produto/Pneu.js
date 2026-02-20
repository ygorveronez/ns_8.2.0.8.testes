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
/// <reference path="../../Consultas/BandaRodagemPneu.js" />
/// <reference path="../../Consultas/ModeloPneu.js" />
/// <reference path="Produto.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _produtoPneu;

var ProdutoPneu = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.GerarPneuAutomatico = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: "Este produto é um Pneu?", enable: ko.observable(true), visible: ko.observable(true) });

    this.ModeloPneu = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Modelo:", idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(false) });
    this.BandaRodagemPneu = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Banda de Rodagem:", idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(false) });

    this.GerarPneuAutomatico.val.subscribe(function (novoValor) {
        if (!novoValor) {
            _produtoPneu.ModeloPneu.visible(false);
            _produtoPneu.ModeloPneu.required(false);
            _produtoPneu.BandaRodagemPneu.visible(false);
            _produtoPneu.BandaRodagemPneu.required(false);

        } else if (novoValor) {
            _produtoPneu.ModeloPneu.visible(true);
            _produtoPneu.ModeloPneu.required(true);
            _produtoPneu.BandaRodagemPneu.visible(true);
            _produtoPneu.BandaRodagemPneu.required(true);
        }
    });
}

//*******EVENTOS*******

function loadProdutoPneu() {
    _produtoPneu = new ProdutoPneu();
    KoBindings(_produtoPneu, "knockoutPneuProduto");

    new BuscarBandaRodagemPneu(_produtoPneu.BandaRodagemPneu);
    new BuscarModeloPneu(_produtoPneu.ModeloPneu);
}

//*******MÉTODOS*******

function limparCamposProdutoPneu() {
    LimparCampos(_produtoPneu);
    _produtoPneu.ModeloPneu.visible(false);
    _produtoPneu.ModeloPneu.required(false);
    _produtoPneu.BandaRodagemPneu.visible(false);
    _produtoPneu.BandaRodagemPneu.required(false);
}

function validaCamposObrigatoriosPneu() {
    return ValidarCamposObrigatorios(_produtoPneu);
}
