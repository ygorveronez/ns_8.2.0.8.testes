var EnumRegimenTributacaoHelper = function () {
    this.NaoSelecionado = 0;
    this.LucroReal = 1;
    this.LucroPresumido = 2;
};

EnumRegimenTributacaoHelper.prototype = {
    ObterOpcoes: function () {
        return [
            { text: Localization.Resources.Gerais.Geral.NaoSelecionado, value: this.NaoSelecionado },
            { text: "Lucro Real", value: this.LucroReal },
            { text: "Lucro Presumido", value: this.LucroPresumido }
        ];
    }
};

var EnumRegimenTributacao = Object.freeze(new EnumRegimenTributacaoHelper());