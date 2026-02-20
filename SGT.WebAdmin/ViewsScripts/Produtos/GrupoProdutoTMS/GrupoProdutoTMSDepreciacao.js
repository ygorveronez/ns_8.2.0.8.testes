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
/// <reference path="../../Consultas/TipoMovimento.js" />
/// <reference path="GrupoProdutoTMS.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _grupoProdutoTMSDepreciacao;

var GrupoProdutoTMSDepreciacao = function () {
    this.GerarDepreciacao = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: "Gerar depreciação aos bens associados ao grupo?", enable: ko.observable(true), visible: ko.observable(true) });

    this.PercentualDepreciacao = PropertyEntity({ text: "*% Depreciação por Ano:", getType: typesKnockout.decimal, maxlength: 6, val: ko.observable(""), def: "", configDecimal: { precision: 2, allowZero: false, allowNegative: false }, required: ko.observable(false), visible: ko.observable(false) });

    this.TipoMovimentoBaixa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Tipo de Movimento de Baixa:", idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(false) });
    this.TipoMovimentoDepreciacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Tipo de Movimento de Depreciação:", idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(false) });
    this.TipoMovimentoDepreciacaoAcumulada = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Tipo de Movimento de Depreciação Acumulada:", idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(false) });

    this.GerarDepreciacao.val.subscribe(function (novoValor) {
        if (!novoValor) {
            limparCamposGrupoProdutoTMSDepreciacao();
        } else if (novoValor) {
            alterarVisibleRequiredCamposGrupoProdutoTMSDepreciacao(true);
        }
    });
}

//*******EVENTOS*******

function loadGrupoProdutoTMSDepreciacao() {
    _grupoProdutoTMSDepreciacao = new GrupoProdutoTMSDepreciacao();
    KoBindings(_grupoProdutoTMSDepreciacao, "knockoutDepreciacaoGrupoProdutoTMS");

    new BuscarTipoMovimento(_grupoProdutoTMSDepreciacao.TipoMovimentoBaixa);
    new BuscarTipoMovimento(_grupoProdutoTMSDepreciacao.TipoMovimentoDepreciacao);
    new BuscarTipoMovimento(_grupoProdutoTMSDepreciacao.TipoMovimentoDepreciacaoAcumulada);
}

//*******MÉTODOS*******

function limparCamposGrupoProdutoTMSDepreciacao() {
    LimparCampos(_grupoProdutoTMSDepreciacao);
    alterarVisibleRequiredCamposGrupoProdutoTMSDepreciacao(false);
}

function alterarVisibleRequiredCamposGrupoProdutoTMSDepreciacao(val) {
    _grupoProdutoTMSDepreciacao.PercentualDepreciacao.visible(val);
    _grupoProdutoTMSDepreciacao.PercentualDepreciacao.required(val);
    _grupoProdutoTMSDepreciacao.TipoMovimentoBaixa.visible(val);
    _grupoProdutoTMSDepreciacao.TipoMovimentoBaixa.required(val);
    _grupoProdutoTMSDepreciacao.TipoMovimentoDepreciacao.visible(val);
    _grupoProdutoTMSDepreciacao.TipoMovimentoDepreciacao.required(val);
    _grupoProdutoTMSDepreciacao.TipoMovimentoDepreciacaoAcumulada.visible(val);
    _grupoProdutoTMSDepreciacao.TipoMovimentoDepreciacaoAcumulada.required(val);
}

function validaCamposObrigatoriosGrupoProdutoTMSDepreciacao() {
    return ValidarCamposObrigatorios(_grupoProdutoTMSDepreciacao);
}