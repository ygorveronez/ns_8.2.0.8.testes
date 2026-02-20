var EnumIntervaloAlertaEmailHelper = function() {
    this.Todos = "";
    this.Dia = 1;
    this.Mes = 2;
    this.Semana = 3;
    this.Ano = 4;
};


EnumIntervaloAlertaEmailHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Dia", value: this.Dia },
            { text: "Mês", value: this.Mes },
            { text: "Semana", value: this.Semana },
            { text: "Ano", value: this.Ano }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Gerais.Geral.Todos, value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumIntervaloAlertaEmail = Object.freeze(new EnumIntervaloAlertaEmailHelper());