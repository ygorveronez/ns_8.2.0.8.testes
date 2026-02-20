var EnumTipoGatilhoOcorrenciaInicialFluxoPatioHelper = function () {
    this.Todos = "";
    this.InicioCarregamento = 1;
    this.ChegadaPatio = 2;
    this.PrevisaoCarregamento = 3;
}

EnumTipoGatilhoOcorrenciaInicialFluxoPatioHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoGatilhoOcorrenciaInicialFluxoPatio.ChegadaPatio, value: this.ChegadaPatio },
            { text: Localization.Resources.Enumeradores.TipoGatilhoOcorrenciaInicialFluxoPatio.InicioCarregamento, value: this.InicioCarregamento },
            { text: Localization.Resources.Enumeradores.TipoGatilhoOcorrenciaInicialFluxoPatio.PrevisaoCarregamento, value: this.PrevisaoCarregamento },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.TipoGatilhoOcorrenciaInicialFluxoPatio.Todos, value: this.Todos }].concat(this.obterOpcoes());
    }
}

var EnumTipoGatilhoOcorrenciaInicialFluxoPatio = Object.freeze(new EnumTipoGatilhoOcorrenciaInicialFluxoPatioHelper());