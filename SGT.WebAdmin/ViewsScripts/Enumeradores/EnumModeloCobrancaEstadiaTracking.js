var EnumModeloCobrancaEstadiaTrackingHelper = function () {
    this.PorEtapa = 1;
    this.PorEtapaAcumulada = 2;
    this.PorViagem = 3;
};

EnumModeloCobrancaEstadiaTrackingHelper.prototype = {
    ObterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.ModeloCobrancaEstadiaTracking.PorEtapa, value: this.PorEtapa },
            { text: Localization.Resources.Enumeradores.ModeloCobrancaEstadiaTracking.PorEtapaAcumulada, value: this.PorEtapaAcumulada },
            { text: Localization.Resources.Enumeradores.ModeloCobrancaEstadiaTracking.PorViagem, value: this.PorViagem }
        ];
    },
    ObterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.ModeloCobrancaEstadiaTracking.Todos, value: 0 }].concat(this.ObterOpcoes());
    }
};

var EnumModeloCobrancaEstadiaTracking = Object.freeze(new EnumModeloCobrancaEstadiaTrackingHelper());