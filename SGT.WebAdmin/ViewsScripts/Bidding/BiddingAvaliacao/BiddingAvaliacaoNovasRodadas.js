/// <reference path="Etapas.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../Enumeradores/EnumStatusBiddingTipoOferta.js" />
/// <reference path="../../Enumeradores/EnumBiddingOfertaSituacao.js" />
/// <reference path="../../Enumeradores/EnumStatusBidding.js" />
/// <reference path="../../Enumeradores/EnumMes.js" />
/// <reference path="../../Enumeradores/EnumTipoTransportadorBidding.js" />
/// <reference path="BiddingAvaliacao.js" />

var _propostaNovasRodadas;
var _propostaMultiplasNovasRodadas;

var PropostaNovasRodadas = function () {
    this.ProporBaselineOuTarget = PropertyEntity({ val: ko.observable(EnumNovasRodadasBaselineTarget.ValorAlvo), def: EnumNovasRodadasBaselineTarget.ValorAlvo, text: "Propor Nova Rodada:", options: EnumNovasRodadasBaselineTarget.obterOpcoes() });
    this.Target = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.decimal, text: ko.observable("*Valor Alvo:"), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });
    this.Baseline = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.decimal, text: ko.observable("% *Baseline:"), required: ko.observable(false), enable: ko.observable(false), visible: ko.observable(false) });
    this.MenorValor = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.decimal, text: ko.observable("% *Menor Valor:"), required: ko.observable(false), enable: ko.observable(false), visible: ko.observable(false) });
    this.Confirmar = PropertyEntity({ eventClick: ConfirmarNovasRodadas, type: types.event, text: "Confirmar", visible: ko.observable(true) });
    this.BaseCalculoMenorValor = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.decimal });
    this.BaseCalculoBaseline = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.decimal });

    this.Baseline.val.subscribe(function (novoValor) {
        if (novoValor) {
            var baseline = Globalize.parseFloat(_propostaNovasRodadas.BaseCalculoBaseline.val());
            var novoValorFloat = Globalize.parseFloat(novoValor);

            var porcentagem = novoValorFloat;
            var reducao = (porcentagem / 100) * baseline;

            var proximoValorAlvoBaseline = baseline - reducao;

            _propostaNovasRodadas.Target.val(Globalize.format(proximoValorAlvoBaseline, "n2"));
        }
    });

    this.MenorValor.val.subscribe(function (novoValor) {
        if (novoValor) {
            var menorValor = Globalize.parseFloat(_propostaNovasRodadas.BaseCalculoMenorValor.val());
            var novoValorFloat = Globalize.parseFloat(novoValor);

            var porcentagem = novoValorFloat;
            var reducao = (porcentagem / 100) * menorValor;

            var proximoValorAlvoMenorValor = menorValor - reducao;

            _propostaNovasRodadas.Target.val(Globalize.format(proximoValorAlvoMenorValor, "n2"));
        }
    });

    this.ProporBaselineOuTarget.val.subscribe(function (novoValor) {
        if (novoValor == EnumNovasRodadasBaselineTarget.Baseline) {
            _propostaNovasRodadas.Target.val("");
            _propostaNovasRodadas.Target.enable(false);

            _propostaNovasRodadas.Baseline.required(true);
            _propostaNovasRodadas.Baseline.enable(true);
            _propostaNovasRodadas.Baseline.visible(true);

            _propostaNovasRodadas.MenorValor.visible(false);
            _propostaNovasRodadas.MenorValor.val("");
        } else if (novoValor == EnumNovasRodadasBaselineTarget.MenorValor) {
            _propostaNovasRodadas.Target.val("");
            _propostaNovasRodadas.Target.enable(false);

            _propostaNovasRodadas.MenorValor.required(true);
            _propostaNovasRodadas.MenorValor.enable(true);
            _propostaNovasRodadas.MenorValor.visible(true);

            _propostaNovasRodadas.Baseline.visible(false);
            _propostaNovasRodadas.Baseline.val("");
        } else if (novoValor == EnumNovasRodadasBaselineTarget.ValorAlvo) {
            _propostaNovasRodadas.Target.enable(true);

            _propostaNovasRodadas.Baseline.visible(false);
            _propostaNovasRodadas.Baseline.val("");

            _propostaNovasRodadas.MenorValor.visible(false);
            _propostaNovasRodadas.MenorValor.val("");
        }

    });
};

var PropostaMultiplasNovasRodadas = function () {
    this.ProporBaselineOuMenorValor = PropertyEntity({ val: ko.observable(EnumNovasRodadasBaselineMenorValor.Baseline), def: EnumNovasRodadasBaselineMenorValor.Baseline, text: "Propor Nova Rodada:", options: EnumNovasRodadasBaselineMenorValor.obterOpcoes() });
    this.Baseline = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.decimal, text: ko.observable("% *Baseline:"), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });
    this.MenorValor = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.decimal, text: ko.observable("% *Menor Valor:"), required: ko.observable(false), enable: ko.observable(false), visible: ko.observable(false) });
    this.Confirmar = PropertyEntity({ eventClick: ConfirmarMultiplasNovasRodadas, type: types.event, text: "Confirmar", visible: ko.observable(true) });
    this.MultiplasNovasRodadas = PropertyEntity({ type: types.listEntity, list: new Array(), codEntity: ko.observable(0), idGrid: guid() });

    this.ProporBaselineOuMenorValor.val.subscribe(function (novoValor) {
        if (novoValor == EnumNovasRodadasBaselineMenorValor.Baseline) {
            _propostaMultiplasNovasRodadas.Baseline.required(true);
            _propostaMultiplasNovasRodadas.Baseline.enable(true);
            _propostaMultiplasNovasRodadas.Baseline.visible(true);

            _propostaMultiplasNovasRodadas.MenorValor.visible(false);
            _propostaMultiplasNovasRodadas.MenorValor.val("");

        } else if (novoValor == EnumNovasRodadasBaselineMenorValor.MenorValor) {
            _propostaMultiplasNovasRodadas.MenorValor.required(true);
            _propostaMultiplasNovasRodadas.MenorValor.enable(true);
            _propostaMultiplasNovasRodadas.MenorValor.visible(true);

            _propostaMultiplasNovasRodadas.Baseline.visible(false);
            _propostaMultiplasNovasRodadas.Baseline.val("");
        } 
    });
};

function LoadBiddingAvaliacaoNovasRodadas() {
    _propostaNovasRodadas = new PropostaNovasRodadas();
    KoBindings(_propostaNovasRodadas, "knockoutPropostaNovasRodadas");

    _propostaMultiplasNovasRodadas = new PropostaMultiplasNovasRodadas();
    KoBindings(_propostaMultiplasNovasRodadas, "knockoutPropostaMultiplasNovasRodadas");
}