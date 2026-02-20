var EnumTipoDiasAprovacaoHelper = function () {
    this.Todas = "";
    this.DiasCorridos = 0;
    this.DiasUteis = 1;
};

EnumTipoDiasAprovacaoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoDiasAprovacao.DiasCorridos, value: this.DiasCorridos },
            { text: Localization.Resources.Enumeradores.TipoDiasAprovacao.DiasUteis, value: this.DiasUteis }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.TipoDiasAprovacao.Todas, value: this.Todas }].concat(this.obterOpcoes());
    }
};

var EnumTipoDiasAprovacao = Object.freeze(new EnumTipoDiasAprovacaoHelper());