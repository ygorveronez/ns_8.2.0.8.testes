var EnumTipoPropriedadeHelper = function () {
    this.NaoInformado = "";
    this.FrotaPropria = 1;
    this.Terceiros = 2;
    this.Agregado = 3;
};

EnumTipoPropriedadeHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Frota Própria", value: this.FrotaPropria },
            { text: "Terceiros", value: this.Terceiros },
            { text: "Agregados", value: this.Agregado },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Não informado", value: this.NaoInformado }].concat(this.obterOpcoes());
    }
};

var EnumTipoPropriedade = Object.freeze(new EnumTipoPropriedadeHelper());