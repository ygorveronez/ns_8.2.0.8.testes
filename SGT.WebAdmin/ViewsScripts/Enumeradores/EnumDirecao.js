var EnumDirecaoHelper = function () {
    this.Todos = "";
    this.Norte = "N";
    this.Leste = "L";
    this.Sul = "S";
    this.Oeste = "O";
};

EnumDirecaoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.Direcao.Norte, value: this.Norte },
            { text: Localization.Resources.Enumeradores.Direcao.Leste, value: this.Leste },
            { text: Localization.Resources.Enumeradores.Direcao.Sul, value: this.Sul },
            { text: Localization.Resources.Enumeradores.Direcao.Oeste, value: this.Oeste }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.Direcao.Todos, value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumDirecao = Object.freeze(new EnumDirecaoHelper());