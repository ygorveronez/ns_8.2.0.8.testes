var EnumCargaTrechoSumarizadaHelper = function () {
    this.Agrupadora = 1;
    this.SubCarga = 2;
};

EnumCargaTrechoSumarizadaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.CargaTrechoSumarizada.Agrupadora, value: this.Agrupadora },
            { text: Localization.Resources.Enumeradores.CargaTrechoSumarizada.SubCarga, value: this.SubCarga }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Gerais.Geral.Todos, value: "" }].concat(this.obterOpcoes());
    }
}

var EnumCargaTrechoSumarizada = Object.freeze(new EnumCargaTrechoSumarizadaHelper());