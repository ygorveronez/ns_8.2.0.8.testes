var EnumEtapaAgendamentoColetaHelper = function () {
    this.Todas = "";
    this.DadosTransporte = 1;
    this.NFe = 2;
    this.NFeCancelada = 5;
    this.Emissao = 3;
    this.AguardandoAceite = 4;
    this.DocumentoParaTransporte = 6;
}

EnumEtapaAgendamentoColetaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Dados Transporte", value: this.DadosTransporte },
            { text: "NF-e", value: this.NFe },
            { text: "NF-e Cancelada", value: this.NFeCancelada },
            { text: "Emissão", value: this.Emissao },
            { text: "Aguardando Aceite", value: this.AguardandoAceite },
            { text: "Documento para Transporte", value: this.DocumentoParaTransporte }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    }
}

var EnumEtapaAgendamentoColeta = Object.freeze(new EnumEtapaAgendamentoColetaHelper());