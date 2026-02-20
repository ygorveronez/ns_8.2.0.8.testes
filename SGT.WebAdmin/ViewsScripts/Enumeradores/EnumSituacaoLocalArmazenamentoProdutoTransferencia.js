var EnumSituacaoLocalArmazenamentoLocalTransferenciaHelper = function () {
    this.AgTransferencia = 1;
    this.Transferido = 2;
    this.Cancelado = 3;
    this.ProblemaTransferencia = 4;

};

EnumSituacaoLocalArmazenamentoLocalTransferenciaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Ag. Transferência", value: this.AgTransferencia },
            { text: "Transferido", value: this.Transferido },
            { text: "Cancelado", value: this.Cancelado },
            { text: "Problema Transferência", value: this.ProblemaTransferencia },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    }
};

var EnumSituacaoLocalArmazenamentoLocalTransferencia = Object.freeze(new EnumSituacaoLocalArmazenamentoLocalTransferenciaHelper());