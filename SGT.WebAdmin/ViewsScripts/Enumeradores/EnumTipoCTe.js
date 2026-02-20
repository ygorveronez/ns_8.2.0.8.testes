var EnumTipoCTeHelper = function () {
    this.Todos = -1;
    this.Normal = 0;
    this.Complementar = 1;
    this.Anulacao = 2;
    this.Substituicao = 3;
    this.Simplificado = 4;
};

EnumTipoCTeHelper.prototype = {
    ObterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoCTe.Normal, value: this.Normal },
            { text: Localization.Resources.Enumeradores.TipoCTe.Complementar, value: this.Complementar },
            { text: Localization.Resources.Enumeradores.TipoCTe.Anulacao, value: this.Anulacao },
            { text: Localization.Resources.Enumeradores.TipoCTe.Substituicao, value: this.Substituicao },
            { text: "Simplificado", value: this.Simplificado }
        ];
    },
    ObterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.TipoCTe.Todos, value: this.Todos }].concat(this.ObterOpcoes());
    },
    ObterOpcoesFaturamento: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoCTe.Normal, value: this.Normal },
            { text: Localization.Resources.Enumeradores.TipoCTe.Complementar, value: this.Complementar },            
            { text: Localization.Resources.Enumeradores.TipoCTe.Substituicao, value: this.Substituicao },
            { text: Localization.Resources.Enumeradores.TipoCTe.Todos, value: this.Todos },
            { text: "Simplificado", value: this.Simplificado }
        ];
    },
};

var EnumTipoCTe = Object.freeze(new EnumTipoCTeHelper());