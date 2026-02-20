var EnumTipoRegraQualidadeMonitoramentoHelper = function () {
    this.Todas = null;
    this.PreEmbarque = 1;
    this.DeslocamentoParaOrigem = 2;
    this.PreCheckin = 3;
    this.EmCarregamento = 4;
    this.SaidaOrigem = 5;
    this.EmViagem = 6;
    this.ChegadaDestino = 7;
    this.Descarga = 8;
    this.SaidaDestino = 9;
};

EnumTipoRegraQualidadeMonitoramentoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Pré embarque", value: this.PreEmbarque },
            { text: "Deslocamento para origem", value: this.DeslocamentoParaOrigem },
            { text: "Pré Check In", value: this.PreCheckin },
            { text: "Em carregamento", value: this.EmCarregamento },
            { text: "Saída origem", value: this.SaidaOrigem },
            { text: "Em viagem", value: this.EmViagem },
            { text: "Chegada destino", value: this.ChegadaDestino },
            { text: "Descarga", value: this.Descarga },
            { text: "Saída destino", value: this.SaidaDestino },

        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    }
}

var EnumTipoRegraQualidadeMonitoramento = Object.freeze(new EnumTipoRegraQualidadeMonitoramentoHelper());
