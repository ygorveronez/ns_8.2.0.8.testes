var EnumSituacaoEncerramentoCargaHelper = function () {
    this.Todas = "";
    this.EmEncerramento = 1;
    this.AgEncerramentoDocumentos = 2;
    this.AgEncerramentoCIOT = 3;
    this.AgEncerramentoMDFe = 4;
    this.AgIntegracao = 5;
    this.RejeicaoEncerramento = 6;
    this.Encerrada = 7;
};

EnumSituacaoEncerramentoCargaHelper.prototype = {
    ObterOpcoes: function () {
        return [
            { text: "Em Encerramento", value: this.EmEncerramento },
            { text: "Ag. Encerramento Documentos", value: this.AgEncerramentoDocumentos },
            { text: "Ag. Encerramento CIOT", value: this.AgEncerramentoCIOT },
            { text: "Ag. Encerramento MDF-es", value: this.AgEncerramentoMDFe },
            { text: "Ag. Integrações", value: this.AgIntegracao },
            { text: "Encerramento Rejeitado", value: this.RejeicaoEncerramento },
            { text: "Encerrada", value: this.Encerrada },
        ];
    },
    ObterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.ObterOpcoes());
    }
};

var EnumSituacaoEncerramentoCarga = Object.freeze(new EnumSituacaoEncerramentoCargaHelper());