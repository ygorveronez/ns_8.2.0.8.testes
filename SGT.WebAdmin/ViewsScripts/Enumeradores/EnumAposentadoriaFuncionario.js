var EnumAposentadoriaFuncionarioHelper = function () {
    this.NaoInformado = 0;
    this.Aposentado = 1;
    this.NaoAposentado = 2;
    this.Todos = 99;
};

EnumAposentadoriaFuncionarioHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.AposentadoriaFuncionario.NaoInformado, value: this.NaoInformado },
            { text: Localization.Resources.Enumeradores.AposentadoriaFuncionario.Aposentado, value: this.Aposentado },
            { text: Localization.Resources.Enumeradores.AposentadoriaFuncionario.NaoAposentado, value: this.NaoAposentado }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{
            text: Localization.Resources.Enumeradores.AposentadoriaFuncionario.Todos, value: this.Todos }].concat(this.obterOpcoes());
    },
};

var EnumAposentadoriaFuncionario = Object.freeze(new EnumAposentadoriaFuncionarioHelper());