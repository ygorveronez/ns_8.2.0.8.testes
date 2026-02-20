var EnumSituacaoPreCargaHelper = function () {
    this.Todas = "";
    this.AguardandoGeracaoCarga = 0;
    this.CargaGerada = 1;
    this.Cancelada = 2;
    this.Nova = 3;
    this.AguardandoAceite = 4;
    this.AguardandoDadosTransporte = 5;
}

EnumSituacaoPreCargaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.ResumoEntrega.AgAceiteTransportador, value: this.AguardandoAceite },
            { text: Localization.Resources.Enumeradores.ResumoEntrega.AgDadosTransportador, value: this.AguardandoDadosTransporte },
            { text: Localization.Resources.Enumeradores.ResumoEntrega.AgPreCarga, value: this.AguardandoGeracaoCarga },
            { text: Localization.Resources.Enumeradores.ResumoEntrega.Cancelado, value: this.Cancelada },
            { text: Localization.Resources.Enumeradores.ResumoEntrega.PreCargaVinculada, value: this.CargaGerada },
            { text: Localization.Resources.Enumeradores.ResumoEntrega.Nova, value: this.Nova }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.ResumoEntrega.Todas, value: this.Todas }].concat(this.obterOpcoes());
    }
}

var EnumSituacaoPreCarga = Object.freeze(new EnumSituacaoPreCargaHelper());
