var MovimentacaoPneuDadosAdicionais = function () {
    var self = this;

    this.Calibragem = PropertyEntity({ text: "*Calibragem:", getType: typesKnockout.int, maxlength: 9, configInt: { precision: 0, allowZero: true, thousands: "" }, required: true });
    this.MilimetragemDois = PropertyEntity({ text: "Milimetragem 2:", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false }, maxlength: 10 });
    this.MilimetragemMedia = PropertyEntity({ text: "Média Milimetragem:", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false }, maxlength: 10, enable: false });
    this.MilimetragemQuatro = PropertyEntity({ text: "Milimetragem 4:", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false }, maxlength: 10 });
    this.MilimetragemTres = PropertyEntity({ text: "Milimetragem 3:", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false }, maxlength: 10 });
    this.MilimetragemUm = PropertyEntity({ text: "Milimetragem 1:", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false }, maxlength: 10 });
    this.SulcoAnterior = PropertyEntity({ text: "*Sulco Anterior:", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false }, maxlength: 10, required: true, enable: false });
    this.SulcoAtual = PropertyEntity({ text: "*Sulco Atual:", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: true }, maxlength: 10, required: true });
    this.UtilizarDadosAdicionais = this.CodigoVeiculo = PropertyEntity({ val: ko.observable(true), def: true, getType: typesKnockout.bool });

    this.MilimetragemDois.val.subscribe(function () { self._atualizarMilimetragemMedia(); });
    this.MilimetragemQuatro.val.subscribe(function () { self._atualizarMilimetragemMedia(); });
    this.MilimetragemTres.val.subscribe(function () { self._atualizarMilimetragemMedia(); });
    this.MilimetragemUm.val.subscribe(function () { self._atualizarMilimetragemMedia(); });
    this.UtilizarDadosAdicionais.val.subscribe(function () { self._controlarCamposObrigatorios(); });
}

MovimentacaoPneuDadosAdicionais.prototype = {
    constructor: MovimentacaoPneuDadosAdicionais,
    _atualizarMilimetragemMedia: function () {
        this.MilimetragemMedia.val(this._obterMilimetragemMedia());
    },
    _controlarCamposObrigatorios: function () {
        var utilizarDadosAdicionais = this.UtilizarDadosAdicionais.val();

        this.Calibragem.required = utilizarDadosAdicionais;
        this.SulcoAnterior.required = utilizarDadosAdicionais;
        this.SulcoAtual.required = utilizarDadosAdicionais;
    },
    _obterMilimetragemMedia: function () {
        var milimetragemUm = parseFloat(this.MilimetragemUm.val().replace(/\./g, "").replace(",", "."));

        if (isNaN(milimetragemUm))
            return "";

        var milimetragemDois = parseFloat(this.MilimetragemDois.val().replace(/\./g, "").replace(",", "."));

        if (isNaN(milimetragemDois))
            return "";

        var milimetragemTres = parseFloat(this.MilimetragemTres.val().replace(/\./g, "").replace(",", "."));

        if (isNaN(milimetragemTres))
            return "";

        var milimetragemQuatro = parseFloat(this.MilimetragemQuatro.val().replace(/\./g, "").replace(",", "."));

        if (isNaN(milimetragemQuatro))
            return "";

        return Globalize.format((milimetragemUm + milimetragemDois + milimetragemTres + milimetragemQuatro) / 4, "n2");
    }
}
