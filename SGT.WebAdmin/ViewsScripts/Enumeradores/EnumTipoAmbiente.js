var EnumTipoAmbienteHelper = function () {
    this.Producao = 1;
    this.Homologacao = 2;
};

EnumTipoAmbienteHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoAmbiente.Homologacao, value: this.Homologacao },
            { text: Localization.Resources.Enumeradores.TipoAmbiente.Producao, value: this.Producao }
        ];
    }
};

var EnumTipoAmbiente = Object.freeze(new EnumTipoAmbienteHelper());