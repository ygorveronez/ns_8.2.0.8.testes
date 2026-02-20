var EnumTipoMotoristaAjudanteHelper = function () {
    this.Motorista = 0;
    this.Ajudante = 1;
    this.Todos = 2;
};

EnumTipoMotoristaAjudanteHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoMotorista.Ajudante, value: this.Ajudante },
            { text: Localization.Resources.Enumeradores.TipoMotorista.Motorista, value: this.Motorista },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.TipoMotorista.Todos, value: this.Todos }].concat(this.obterOpcoes());
    },
};

var EnumTipoMotoristaAjudante = Object.freeze(new EnumTipoMotoristaAjudanteHelper());