var EnumSituacaoMDFeManualHelper = function () {
    this.EmDigitacao = 0;
    this.EmEmissao = 1;
    this.Finalizado = 2;
    this.Rejeicao = 3;
    this.Cancelado = 4;
    this.ProcessandoIntegracao = 5;
    this.AgIntegracao = 6;
    this.FalhaIntegracao = 7;
};

EnumSituacaoMDFeManualHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Em Digitação", value: this.EmDigitacao },
            { text: "Em Emissão", value: this.EmEmissao },
            { text: "Finalizado", value: this.Finalizado },
            { text: "Rejeição", value: this.Rejeicao },
            { text: "Cancelado", value: this.Cancelado },
            { text: "Processando Integração", value: this.ProcessandoIntegracao },
            { text: "Ag. Integração", value: this.AgIntegracao },
            { text: "Falha na Integração", value: this.FalhaIntegracao }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    }
};

var EnumSituacaoMDFeManual = Object.freeze(new EnumSituacaoMDFeManualHelper());