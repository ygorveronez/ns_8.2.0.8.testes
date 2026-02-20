var EnumTipoOperacaoMovimentacaoEstoquePalletHelper = function () {
    this.ClienteEntrada = 7;
    this.ClienteSaida = 8;
    this.ClienteFilial = 9;
    this.ClienteTransportador = 15;
    this.FilialEntrada = 0;
    this.FilialSaida = 1;
    this.FilialCliente = 2;
    this.FilialTransportador = 3;
    this.TransportadorCliente = 14;
    this.TransportadorEntrada = 4;
    this.TransportadorSaida = 5;
    this.TransportadorFilial = 6;
}

EnumTipoOperacaoMovimentacaoEstoquePalletHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Filial Entrada (+)", value: this.FilialEntrada },
            { text: "Filial Saída (-)", value: this.FilialSaida },
            { text: "Filial para Cliente", value: this.FilialCliente },
            { text: "Filial para Transportador", value: this.FilialTransportador },
            { text: "Transportador Entrada (+)", value: this.TransportadorEntrada },
            { text: "Transportador Saída (-)", value: this.TransportadorSaida },
            { text: "Transportador para Filial", value: this.TransportadorFilial },
            { text: "Cliente Entrada (+)", value: this.ClienteEntrada },
            { text: "Cliente Saída (-)", value: this.ClienteSaida },
            { text: "Cliente para Filial", value: this.ClienteFilial }
        ];
    },
    obterOpcoesTMS: function () {
        return [
            { text: "Empresa/Filial Entrada (+)", value: this.TransportadorEntrada },
            { text: "Empresa/Filial Saída (-)", value: this.TransportadorSaida },
            { text: "Empresa/Filial para Cliente", value: this.TransportadorCliente },
            { text: "Cliente Entrada (+)", value: this.ClienteEntrada },
            { text: "Cliente Saída (-)", value: this.ClienteSaida },
            { text: "Cliente para Empresa/Filial", value: this.ClienteTransportador }
        ];
    }
}

var EnumTipoOperacaoMovimentacaoEstoquePallet = Object.freeze(new EnumTipoOperacaoMovimentacaoEstoquePalletHelper());