var EnumSituacaoCargaValidacaoStatusViagemMonitoramentoHelper = function () {
    this.Nenhum = null;
    this.NaLogistica = 0;
    this.Nova = 1;
    this.CalculoFrete = 2;
    this.AgTransportador = 4;
    this.AgNFe = 5;
    this.PendeciaDocumentos = 6;
    this.AgImpressaoDocumentos = 7;
    this.ProntoTransporte = 8;
    this.EmTransporte = 9;
    this.LiberadoPagamento = 10;
    this.Encerrada = 11;
    this.Cancelada = 13;
    this.AgIntegracao = 15;
    this.EmTransbordo = 17;
    this.Anulada = 18;
    this.Todas = 99;
    this.PermiteCTeManual = 101;
};

EnumSituacaoCargaValidacaoStatusViagemMonitoramentoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.SituacoesCarga.ComLogistica, value: this.NaLogistica },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.NovaCarga, value: this.Nova },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.CalculoDeFrete, value: this.CalculoFrete },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.AguardandoTransportador, value: this.AgTransportador },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.AguardandoNotasFiscais, value: this.AgNFe },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.PendenciasNaEmissao, value: this.PendeciaDocumentos },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.AguardandoImpressaoDosDocumentos, value: this.AgImpressaoDocumentos },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.ProntoParaTransporte, value: this.ProntoTransporte },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.EmTransporte, value: this.EmTransporte },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.PagamentoLiberado, value: this.LiberadoPagamento },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.Finalizada, value: this.Encerrada },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.Cancelada, value: this.Cancelada },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.AguardandoIntegracao, value: this.AgIntegracao },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.EmTransbordo, value: this.EmTransbordo },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.Anulada, value: this.Anulada },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.Todas, value: this.Todas },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.PermiteCTeManual, value: this.PermiteCTeManual }
        ];
    },

    obterOpcoesTipoTransito: function () {
        return [{ text: Localization.Resources.Enumeradores.SituacoesCarga.EmTransporte, value: this.EmTransporte }];
    }
};

var EnumSituacaoCargaValidacaoStatusViagemMonitoramento = Object.freeze(new EnumSituacaoCargaValidacaoStatusViagemMonitoramentoHelper());
