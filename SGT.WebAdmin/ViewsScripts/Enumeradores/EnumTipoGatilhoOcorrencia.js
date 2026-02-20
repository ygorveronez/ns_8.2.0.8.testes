var EnumTipoGatilhoOcorrenciaHelper = function () {
    this.Todos = "";
    this.FluxoPatio = 1;
    this.Tracking = 2;
    this.AlteracaoData = 3;
    this.AtingirData = 4
}

EnumTipoGatilhoOcorrenciaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoGatilhoOcorrencia.FluxoPatio, value: this.FluxoPatio },
            { text: Localization.Resources.Enumeradores.TipoGatilhoOcorrencia.Tracking, value: this.Tracking },
            { text: "Alteração de Data ", value: this.AlteracaoData },
            { text: "Atingir Data ", value: this.AtingirData },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.TipoGatilhoOcorrencia.Todos, value: this.Todos }].concat(this.obterOpcoes());
    }
}

var EnumTipoGatilhoOcorrencia = Object.freeze(new EnumTipoGatilhoOcorrenciaHelper());