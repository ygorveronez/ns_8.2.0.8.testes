var EnumPosicaoReboqueHelper = function () {
    this.NaoInformado = "";
    this.ReboqueUm = 1;
    this.ReboqueDois = 2;
    this.ReboqueTres = 3;
};

EnumPosicaoReboqueHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Gerais.Geral.NaoSelecionado, value: this.NaoInformado },
            { text: "1", value: this.ReboqueUm },
            { text: "2", value: this.ReboqueDois },
            { text: "3", value: this.ReboqueTres }
        ];
    }
};

var PosicaoReboque = Object.freeze(new EnumPosicaoReboqueHelper());