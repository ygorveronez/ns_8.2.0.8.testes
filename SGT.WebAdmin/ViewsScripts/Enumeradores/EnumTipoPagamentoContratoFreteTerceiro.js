var EnumTipoPagamentoContratoFreteTerceiroHelper = function () {
    this.SobreFreteCarga = 1;
    this.TabelaFrete = 2;
};

EnumTipoPagamentoContratoFreteTerceiroHelper.prototype = {
    obterOpcoes: function (defaultValue, defaultText) {
        var opcoes = [];

        if (defaultValue != null && defaultText != null)
            opcoes.push({ value: defaultValue, text: defaultText });

        return opcoes.concat([
            { value: this.SobreFreteCarga, text: Localization.Resources.Enumeradores.TipoPagamentoContratoFreteTerceiro.SobreValorDoFreteDaCarga },
            { value: this.TabelaFrete, text: Localization.Resources.Enumeradores.TipoPagamentoContratoFreteTerceiro.CalculadoPelaTabelaDeFrete }
        ]);
    },
    obterOpcoesPesquisa: function () {
        return [{ value: "", text: Localization.Resources.Enumeradores.EnumTipoFavorecidoCIOT.Todos }].concat(this.ObterOpcoes());
    }
};

var EnumTipoPagamentoContratoFreteTerceiro = Object.freeze(new EnumTipoPagamentoContratoFreteTerceiroHelper());