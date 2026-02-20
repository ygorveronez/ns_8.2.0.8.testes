var EnumTipoEmissaoCTeDocumentosHelper = function () {
    this.NaoInformado = 0;
    this.EmitePorPedidoAgrupado = 1;
    this.EmitePorNotaFiscalAgrupada = 2;
    this.EmitePorNotaFiscalIndividual = 3;
    this.EmitePorPedidoIndividual = 4;
    this.EmitePorNotaFiscalAgrupadaEntrePedidos = 5;
};

EnumTipoEmissaoCTeDocumentosHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoEmissaoCTeDocumentos.NaoInformado, value: this.NaoInformado },
            { text: Localization.Resources.Enumeradores.TipoEmissaoCTeDocumentos.PorDestinatarioDosPedidos, value: this.EmitePorPedidoAgrupado },
            { text: Localization.Resources.Enumeradores.TipoEmissaoCTeDocumentos.PorDestinatarioDasNotasFiscais, value: this.EmitePorNotaFiscalAgrupada },
            { text: Localization.Resources.Enumeradores.TipoEmissaoCTeDocumentos.PorNotaFiscalUmCtePorNota, value: this.EmitePorNotaFiscalIndividual },
            { text: Localization.Resources.Enumeradores.TipoEmissaoCTeDocumentos.PorPedidoUmCtePorPedido, value: this.EmitePorPedidoIndividual },
            { text: Localization.Resources.Enumeradores.TipoEmissaoCTeDocumentos.PorDestinatarioDasNotasFiscaisEntreOsPedidos, value: this.EmitePorNotaFiscalAgrupadaEntrePedidos },
        ];
    },
    obterOpcoesTipoRateioDocumentos: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoEmissaoCTeDocumentos.NaoInformado, value: this.NaoInformado },
            { text: Localization.Resources.Enumeradores.TipoEmissaoCTeDocumentos.PorDestinatarioDasNotasFiscais, value: this.EmitePorNotaFiscalAgrupada },
            { text: Localization.Resources.Enumeradores.TipoEmissaoCTeDocumentos.PorNotaFiscal, value: this.EmitePorNotaFiscalIndividual },
            { text: Localization.Resources.Enumeradores.TipoEmissaoCTeDocumentos.PorPedido, value: this.EmitePorPedidoAgrupado },
        ];
    }
};

var EnumTipoEmissaoCTeDocumentos = Object.freeze(new EnumTipoEmissaoCTeDocumentosHelper());