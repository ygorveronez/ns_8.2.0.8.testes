var EnumSituacaoIntegracaoProcessamentoEDIFTPHelper = function () {
    this.Todos = "";
    this.AgIntegracao = 1;
    this.Integrado = 2;
    this.Falha = 3;
};

EnumSituacaoIntegracaoProcessamentoEDIFTPHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Todos", value: this.Todos },
            { text: "Ag. Integração", value: this.AgIntegracao },
            { text: "Falha", value: this.Falha },
            { text: "Integrado", value: this.Integrado }
        ];
    }
}

var EnumSituacaoIntegracaoProcessamentoEDIFTP = Object.freeze(new EnumSituacaoIntegracaoProcessamentoEDIFTPHelper());