var EnumTipoAgrupamentoFaturaHelper = function () {
    this.Todos = 0;
    this.NavioViagemDirecaoPOLPOD = 1;
    this.NavioViagemDirecaoPOL = 2;
    this.Booking = 3;
    this.Container = 4;
    this.NumeroControleCliente = 5;
    this.NumeroReferenciaEDI = 6;
    this.CTe = 7;
    this.Manual = 8;
};

EnumTipoAgrupamentoFaturaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoAgrupamentoFatura.NenhumaOpcaoPoderaEmitirApenasFaturamentoManual, value: this.Todos },
            { text: Localization.Resources.Enumeradores.TipoAgrupamentoFatura.NavioViagemDirecaoPOLPODManifesto, value: this.NavioViagemDirecaoPOLPOD },
            { text: Localization.Resources.Enumeradores.TipoAgrupamentoFatura.NavioViagemDirecaoPOL, value: this.NavioViagemDirecaoPOL },
            { text: Localization.Resources.Enumeradores.TipoAgrupamentoFatura.Booking, value: this.Booking },
            { text: Localization.Resources.Enumeradores.TipoAgrupamentoFatura.Container, value: this.Container },
            { text: Localization.Resources.Enumeradores.TipoAgrupamentoFatura.NumeroDeControleDoCliente, value: this.NumeroControleCliente },
            { text: Localization.Resources.Enumeradores.TipoAgrupamentoFatura.NumeroDeReferenciaDoEDI, value: this.NumeroReferenciaEDI },
            { text: Localization.Resources.Enumeradores.TipoAgrupamentoFatura.PorCTe, value: this.CTe },
            { text: Localization.Resources.Enumeradores.TipoAgrupamentoFatura.Manual, value: this.Manual }
        ];
    }
};

var EnumTipoAgrupamentoFatura = Object.freeze(new EnumTipoAgrupamentoFaturaHelper());