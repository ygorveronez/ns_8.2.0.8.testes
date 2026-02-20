
var EnumTipoCampoCCeHelper = function () {
    this.Texto = 0;
    this.Inteiro = 1;
    this.Decimal = 2;
    this.Selecao = 3;
    this.Data = 4;
};

EnumTipoCampoCCeHelper.prototype = {
    ObterOpcoes: function () {
        return [
            { text: "Texto", value: this.Texto },
            { text: "Inteiro", value: this.Inteiro },
            { text: "Decimal", value: this.Decimal },
            //{ text: "Seleção", value: this.Selecao }, não serão tratados campos de seleção no TMS/Embarcador pois requerem implementação campo a campo
            { text: "Data", value: this.Data }
        ];
    },
    ObterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: "" }].concat(this.ObterOpcoes());
    }
};

var EnumTipoCampoCCe = Object.freeze(new EnumTipoCampoCCeHelper());