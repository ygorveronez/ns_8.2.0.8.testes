var EnumTipoImpressaoHelper = function() {
    this.Nenhum = 0;
    this.PrestacaoServico = 1;
    this.Coleta = 2;
    this.Romaneio = 3;
    this.Minuta = 4;
    this.Transferencia = 5;
    this.Distribuicao = 6;
};

EnumTipoImpressaoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoImpressao.Nenhum, value: this.Nenhum },
            { text: Localization.Resources.Enumeradores.TipoImpressao.PrestacaoServico, value: this.PrestacaoServico },
            { text: Localization.Resources.Enumeradores.TipoImpressao.Coleta, value: this.Coleta },
            { text: Localization.Resources.Enumeradores.TipoImpressao.Romaneio, value: this.Romaneio },
            { text: Localization.Resources.Enumeradores.TipoImpressao.Minuta, value: this.Minuta },
            { text: Localization.Resources.Enumeradores.TipoImpressao.Transferencia, value: this.Transferencia },
            { text: Localization.Resources.Enumeradores.TipoImpressao.Distribuicao, value: this.Distribuicao }
        ];
    },
};

var EnumTipoImpressao = Object.freeze(new EnumTipoImpressaoHelper());