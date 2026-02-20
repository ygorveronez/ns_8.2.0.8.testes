var EnumTransitoAduaneiroHelper = function () {
    this.Sim = 0;
    this.Nao = 1;
    this.Nenhum = 2;
};

EnumTransitoAduaneiroHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Gerais.Geral.Sim, value: this.Sim },
            { text: Localization.Resources.Gerais.Geral.Nao, value: this.Nao },
            { text: Localization.Resources.Gerais.Geral.Nenhum, value: this.Nenhum }
        ];
    },
};

var EnumTransitoAduaneiro = Object.freeze(new EnumTransitoAduaneiroHelper());