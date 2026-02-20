var EnumSituacaoLoteChamadoOcorrenciaHelper = function () {
    this.EmEdicao = 1;
    this.AgAprovacao = 2;
    this.Aprovado = 3;
    this.Rejeitado = 4;
};

EnumSituacaoLoteChamadoOcorrenciaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Em Edição", value: this.EmEdicao },
            { text: "Ag. Aprovação", value: this.AgAprovacao },
            { text: "Aprovado", value: this.Aprovado },
            { text: "Rejeitado", value: this.Rejeitado },
        ];
    }
};

var EnumSituacaoLoteChamadoOcorrencia = Object.freeze(new EnumSituacaoLoteChamadoOcorrenciaHelper());