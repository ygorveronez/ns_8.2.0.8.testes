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

var _produtoCombustivel;

var ProdutoCombustivel = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.CodigoANP = PropertyEntity({ text: "Código ANP: ", required: false, maxlength: 9 });
    this.PercentualGLP = PropertyEntity({ text: "Percentual GLP ANP: ", getType: typesKnockout.decimal, maxlength: 8, configDecimal: { precision: 4, allowZero: true } });
    this.PercentualGNN = PropertyEntity({ text: "Percentual GNN ANP: ", getType: typesKnockout.decimal, maxlength: 8, configDecimal: { precision: 4, allowZero: true } });
    this.PercentualGNI = PropertyEntity({ text: "Percentual GNI ANP: ", getType: typesKnockout.decimal, maxlength: 8, configDecimal: { precision: 4, allowZero: true } });
    this.ValorPartidaANP = PropertyEntity({ text: "Valor Partida ANP: ", getType: typesKnockout.decimal, maxlength: 18 });
    this.IndicadorImportacaoCombustivel = PropertyEntity({ val: ko.observable(EnumIndicadorImportacaoCombustivel.Nenhum), options: _indicadorImportacaoCombustivel, def: EnumIndicadorImportacaoCombustivel.Nenhum, text: "Indicador Imp. Comp.: ", required: false });
    this.PercentualOrigemCombustivel = PropertyEntity({ text: "Percentual Origem: ", getType: typesKnockout.decimal, maxlength: 8, configDecimal: { precision: 4, allowZero: true } });
    this.PercentualMisturaBiodiesel = PropertyEntity({ text: "Percentual Mistura Biodiesel: ", getType: typesKnockout.decimal, maxlength: 8, configDecimal: { precision: 4, allowZero: true } });

    this.ProdutoCombustivel = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: "Este produto é Combustível?", enable: ko.observable(true), visible: ko.observable(true) });
    this.ControlaEstoqueCombustivel = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: "Controla estoque para abastecimentos interno?", enable: ko.observable(true), visible: ko.observable(false) });

    this.ProdutoCombustivel.val.subscribe(function (novoValor) {
        if (!novoValor) {
            _produtoCombustivel.ControlaEstoqueCombustivel.visible(false);
        } else if (novoValor) {
            _produtoCombustivel.ControlaEstoqueCombustivel.visible(true);
        }
    });
}

//*******EVENTOS*******

function loadProdutoCombustivel() {
    _produtoCombustivel = new ProdutoCombustivel();
    KoBindings(_produtoCombustivel, "knockoutCombustivelProduto");
}

//*******MÉTODOS*******

function limparCamposProdutoCombustivel() {
    LimparCampos(_produtoCombustivel);
    _produtoCombustivel.ControlaEstoqueCombustivel.visible(false);
}