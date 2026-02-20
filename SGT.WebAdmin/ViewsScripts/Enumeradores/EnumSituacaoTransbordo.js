var EnumSituacaoTransbordoHelper = function () {
    this.Todas = 0;
    this.AgInformacoes = 1;
    this.EmTransporte = 2;
    this.Finalizado = 3;
    this.Cancelado = 4;
    this.AgIntegracao = 5;
    this.FalhaIntegracao = 6;
};

EnumSituacaoTransbordoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.SituacaoTransbordo.AgInformacoesCarga, value: this.AgInformacoes },
            { text: Localization.Resources.Enumeradores.SituacaoTransbordo.EmTransporte, value: this.EmTransporte },
            { text: Localization.Resources.Enumeradores.SituacaoTransbordo.Finalizado, value: this.Finalizado },
            { text: Localization.Resources.Enumeradores.SituacaoTransbordo.Cancelado, value: this.Cancelado }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.SituacaoTransbordo.Todas, value: this.Todas }].concat(this.obterOpcoes());
    }
};

var EnumSituacaoTransbordo = Object.freeze(new EnumSituacaoTransbordoHelper());