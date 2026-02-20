var EnumMesHelper = function () {
    this.Todos = "";
    this.Janeiro = 1;
    this.Fevereiro = 2;
    this.Marco = 3;
    this.Abril = 4;
    this.Maio = 5;
    this.Junho = 6;
    this.Julho = 7;
    this.Agosto = 8;
    this.Setembro = 9;
    this.Outubro = 10;
    this.Novembro = 11;
    this.Dezembro = 12;
};

EnumMesHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.Mes.Janeiro, value: this.Janeiro },
            { text: Localization.Resources.Enumeradores.Mes.Fevereiro, value: this.Fevereiro },
            { text: Localization.Resources.Enumeradores.Mes.Marco, value: this.Marco },
            { text: Localization.Resources.Enumeradores.Mes.Abril, value: this.Abril },
            { text: Localization.Resources.Enumeradores.Mes.Maio, value: this.Maio },
            { text: Localization.Resources.Enumeradores.Mes.Junho, value: this.Junho },
            { text: Localization.Resources.Enumeradores.Mes.Julho, value: this.Julho },
            { text: Localization.Resources.Enumeradores.Mes.Agosto, value: this.Agosto },
            { text: Localization.Resources.Enumeradores.Mes.Setembro, value: this.Setembro },
            { text: Localization.Resources.Enumeradores.Mes.Outubro, value: this.Outubro },
            { text: Localization.Resources.Enumeradores.Mes.Novembro, value: this.Novembro },
            { text: Localization.Resources.Enumeradores.Mes.Dezembro, value: this.Dezembro }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.Mes.Todos, value: this.Todos }].concat(this.obterOpcoes());
    },
    obterMesAtual: function () {
        let date = new Date();
        return date.getMonth() + 1;
    }
};

var EnumMes = Object.freeze(new EnumMesHelper());