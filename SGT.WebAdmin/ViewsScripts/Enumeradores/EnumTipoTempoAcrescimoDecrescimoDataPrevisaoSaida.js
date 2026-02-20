var EnumTipoTempoAcrescimoDecrescimoDataPrevisaoSaidaHelper = function () {
    this.Acrescimo = 0;
    this.Descrescimo = 1;
};

EnumTipoTempoAcrescimoDecrescimoDataPrevisaoSaidaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Acréscimo", value: this.Acrescimo },
            { text: "Decréscimo", value: this.Descrescimo }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Gerais.Geral.Todos, value: "" }].concat(this.obterOpcoes());
    }
};

var EnumTipoTempoAcrescimoDecrescimoDataPrevisaoSaida = Object.freeze(new EnumTipoTempoAcrescimoDecrescimoDataPrevisaoSaidaHelper());