var EnumTipoConsultaHelper = function () {
    this.Todos = -1;
    this.AguardandoIntegracao = 0;
    this.Integrados = 1;
    this.FalhaNaIntegracao = 2;
    this.AguardandoRetorno = 3;
};

EnumTipoConsultaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoConsulta.Todos, value: this.Todos },
            { text: Localization.Resources.Enumeradores.TipoConsulta.AguardandoIntegracao, value: this.AguardandoIntegracao },
            { text: Localization.Resources.Enumeradores.TipoConsulta.Integrados, value: this.Integrados },
            { text: Localization.Resources.Enumeradores.TipoConsulta.FalhaNaIntegracao, value: this.FalhaNaIntegracao },
            { text: Localization.Resources.Enumeradores.TipoConsulta.AguardandoRetorno, value: this.AguardandoRetorno }
        ];
    },
}

var EnumTipoConsulta = Object.freeze(new EnumTipoConsultaHelper());