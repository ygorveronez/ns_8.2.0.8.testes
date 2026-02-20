var EnumTipoParametroCriticidadeHelper = function () {
    this.Gerencial = 1;
    this.CausaProblema = 2;
};

EnumTipoParametroCriticidadeHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoParametroCriticidade.Gerencial, value: this.Gerencial },
            { text: Localization.Resources.Enumeradores.TipoParametroCriticidade.CausaProblema, value: this.CausaProblema }
        ];
    },

    obterDescricao: function (valor) {
        switch (parseInt(valor, 10)) {
            case this.Gerencial: return Localization.Resources.Enumeradores.TipoParametroCriticidade.Gerencial;
            case this.CausaProblema: return Localization.Resources.Enumeradores.TipoParametroCriticidade.CausaProblema;
            default: return "";
        }
    }
}

var EnumTipoParametroCriticidade = Object.freeze(new EnumTipoParametroCriticidadeHelper());