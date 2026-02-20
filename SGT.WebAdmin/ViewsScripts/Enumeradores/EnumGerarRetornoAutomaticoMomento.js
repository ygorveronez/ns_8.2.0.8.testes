var EnumGerarRetornoAutomaticoMomentoHelper = function () {
    this.Nenhum = 0;
    this.ConfirmacaoViagem = 1;
    this.FinalizarEmissaoDocumentos = 2;
};

EnumGerarRetornoAutomaticoMomentoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Nenhum", value: this.Nenhum },
            { text: "Confirmação viagem", value: this.ConfirmacaoViagem },
            { text: "Finalizar emissão documentos", value: this.FinalizarEmissaoDocumentos },
        ];
    }
};

var EnumGerarRetornoAutomaticoMomento = Object.freeze(new EnumGerarRetornoAutomaticoMomentoHelper());