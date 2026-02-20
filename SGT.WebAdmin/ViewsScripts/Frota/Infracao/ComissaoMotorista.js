/// <reference path="Infracao.js" />
/// <reference path="../../Enumeradores/EnumSituacaoInfracao.js" />
/// <reference path="../../Enumeradores/EnumTipoHistoricoInfracao.js" />
/// <reference path="../../Consultas/Justificativa.js" />

var _comissaoMotorista;

var ComissaoMotorista = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.LancarDescontoMotorista = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Lançar desconto para o motorista ao próximo acerto? ", idFade: guid(), visibleFade: ko.observable(false), enable: ko.observable(true) });
    this.DescontoComissaoMotorista = PropertyEntity({ text: "*Valor Desconto:", getType: typesKnockout.decimal, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: false }, required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });
    this.JustificativaDesconto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Justificativa:", idBtnSearch: guid(), visible: ko.observable(true), required: ko.observable(false), enable: ko.observable(true) });
    this.LancarDescontoMotorista.val.subscribe(function (novoValor) {
        if (novoValor) {
            _comissaoMotorista.LancarDescontoMotorista.visibleFade(true);
            _comissaoMotorista.DescontoComissaoMotorista.required(true);
            _comissaoMotorista.JustificativaDesconto.required(true);
        } else {
            _comissaoMotorista.LancarDescontoMotorista.visibleFade(false);
            _comissaoMotorista.DescontoComissaoMotorista.required(false);
            _comissaoMotorista.JustificativaDesconto.required(false);
        }
    });

    this.ReduzirPercentualComissaoMotorista = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Reduzir o percentual de comissão ao próximo acerto? ", idFade: guid(), visibleFade: ko.observable(false), enable: ko.observable(true) });
    this.PercentualReducaoComissaoMotorista = PropertyEntity({ text: "*- % Comissão:", getType: typesKnockout.decimal, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: false }, required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });
    this.ReduzirPercentualComissaoMotorista.val.subscribe(function (novoValor) {
        if (novoValor) {
            _comissaoMotorista.ReduzirPercentualComissaoMotorista.visibleFade(true);
            _comissaoMotorista.PercentualReducaoComissaoMotorista.required(true);
        } else {
            _comissaoMotorista.ReduzirPercentualComissaoMotorista.visibleFade(false);
            _comissaoMotorista.PercentualReducaoComissaoMotorista.required(false);
        }
    });
}

function loadComissaoMotorista() {
    _comissaoMotorista = new ComissaoMotorista();
    KoBindings(_comissaoMotorista, "knockoutComissaoMotorista");

    if (_CONFIGURACAO_TMS.NaoLancarDescontosDasOcorrenciasNoAcertoDeViagem) {
        //_comissaoMotorista.DescontoComissaoMotorista.visible(false);
        _comissaoMotorista.PercentualReducaoComissaoMotorista.visible(false);        
    }

    new BuscarJustificativas(_comissaoMotorista.JustificativaDesconto, null, null, [EnumTipoFinalidadeJustificativa.Todas, EnumTipoFinalidadeJustificativa.AcertoViagemOutrasDespesas]);
}

function limparComissaoMotorista() {
    LimparCampos(_comissaoMotorista);
}