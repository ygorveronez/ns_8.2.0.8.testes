var EnumTipoArquivoContabilQuestorHelper = function () {
    this.Padrao = 1;
    this.Contabil = 2;
    this.PadraoTransben = 3;
};

EnumTipoArquivoContabilQuestorHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Padrão", value: this.Padrao },
            { text: "Contábil", value: this.Contabil },
            { text: "Padrão Transben", value: this.PadraoTransben }
        ];
    }
};

var EnumTipoArquivoContabilQuestor = Object.freeze(new EnumTipoArquivoContabilQuestorHelper());