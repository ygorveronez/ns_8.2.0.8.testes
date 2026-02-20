var _impostoTituloFinanceiro;

var ImpostoTituloFinanceiro = function () {
    this.TributoVariacaoImposto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Variação:"), idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });
    this.TributoTipoDocumento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Tp Docto:"), idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });
    this.TributoCodigoReceita = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Cód. da receita:"), idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });
    this.TributoTipoImposto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Imposto:"), idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });
    this.Contribuinte = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Contribuinte:"), idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });
    this.TributoReferencia = PropertyEntity({ text: "Referência: ", required: false, maxlength: 100, enable: ko.observable(true) });
    this.PeriodoApuracao = PropertyEntity({ text: "Período de Apuração: ", required: false, getType: typesKnockout.date, enable: ko.observable(true) });
}

function LoadImpostoTituloFinanceiro() {
    _impostoTituloFinanceiro = new ImpostoTituloFinanceiro();
    KoBindings(_impostoTituloFinanceiro, "tabImposto");

    _tituloFinanceiro.TributoVariacaoImposto = _impostoTituloFinanceiro.TributoVariacaoImposto;
    _tituloFinanceiro.TributoTipoDocumento = _impostoTituloFinanceiro.TributoTipoDocumento;
    _tituloFinanceiro.TributoCodigoReceita = _impostoTituloFinanceiro.TributoCodigoReceita;
    _tituloFinanceiro.TributoTipoImposto = _impostoTituloFinanceiro.TributoTipoImposto;
    _tituloFinanceiro.Contribuinte = _impostoTituloFinanceiro.Contribuinte;
    _tituloFinanceiro.TributoReferencia = _impostoTituloFinanceiro.TributoReferencia;
    _tituloFinanceiro.PeriodoApuracao = _impostoTituloFinanceiro.PeriodoApuracao;
}