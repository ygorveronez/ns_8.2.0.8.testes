var EnumRegimeTributarioHelper = function () {
    this.NaoSelecionado = "";
    this.SimplesNacional = 1;
    this.LucroReal = 2;
    this.LucroPresumido = 3;
};

EnumRegimeTributarioHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.RegimeTributario.SimplesNacional, value: this.SimplesNacional },
            { text: Localization.Resources.Enumeradores.RegimeTributario.LucroReal, value: this.LucroReal },
            { text: Localization.Resources.Enumeradores.RegimeTributario.LucroPresumido, value: this.LucroPresumido }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Gerais.Geral.Todos, value: this.NaoSelecionado }].concat(this.obterOpcoes());
    },
    obterOpcoesNaoSelecionado: function () {
        return [{ text: Localization.Resources.Gerais.Geral.NaoSelecionado, value: this.NaoSelecionado }].concat(this.obterOpcoes());
    }
};

var EnumRegimeTributario = Object.freeze(new EnumRegimeTributarioHelper());