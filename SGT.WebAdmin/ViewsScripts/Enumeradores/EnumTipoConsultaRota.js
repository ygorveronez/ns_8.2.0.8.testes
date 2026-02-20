var EnumTipoConsultaRotaHelper = function () {
    this.Todos = "";
    this.MaisRapida = 1;
    this.MaisCurta = 2;
};

EnumTipoConsultaRotaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Rota mais rápida", value: this.MaisRapida },
            { text: "Rota mais curta", value: this.MaisCurta }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Gerais.Geral.Todos, value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoConsultaRota = Object.freeze(new EnumTipoConsultaRotaHelper());