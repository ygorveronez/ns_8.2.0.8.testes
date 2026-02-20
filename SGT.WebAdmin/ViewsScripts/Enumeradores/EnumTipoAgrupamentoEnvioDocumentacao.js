var EnumTipoAgrupamentoEnvioDocumentacaoHelper = function () {
    this.Todos = 0;
    this.VVDTerminalDestino = 1;
    this.BookingVVDTerminalDestino = 2;
    this.ContainerVVDTerminalDestino = 3;
    this.Nenhum = 4;
    this.CTeVVDTerminalDestino = 5;
};

EnumTipoAgrupamentoEnvioDocumentacaoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoAgrupamentoEnvioDocumentacao.VVDTerminalDeDestino, value: this.VVDTerminalDestino },
            { text: Localization.Resources.Enumeradores.TipoAgrupamentoEnvioDocumentacao.BookingVVDTerminalDeDestino, value: this.BookingVVDTerminalDestino },
            { text: Localization.Resources.Enumeradores.TipoAgrupamentoEnvioDocumentacao.ContainerVVDTerminalDeDestino, value: this.ContainerVVDTerminalDestino },
            { text: Localization.Resources.Enumeradores.TipoAgrupamentoEnvioDocumentacao.CTeVVDTerminalDeDestino, value: this.CTeVVDTerminalDestino },
            { text: Localization.Resources.Enumeradores.TipoAgrupamentoEnvioDocumentacao.Nenhum, value: this.Nenhum }
        ];
    }
};

var EnumTipoAgrupamentoEnvioDocumentacao = Object.freeze(new EnumTipoAgrupamentoEnvioDocumentacaoHelper());