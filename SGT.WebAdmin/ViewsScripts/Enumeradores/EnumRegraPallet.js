var EnumRegraPalletHelper = function () {
    this.Nenhuma = 0;
    this.Devolucao = 1;
    this.CanhotoAssinado = 2;
    this.ValePallet = 3;
    this.Estoque = 4;
    this.Emprestimo = 5; // Essa situação não deve estar disponível para seleção
    this.Transferencia = 6; // Essa situação não deve estar disponível para seleção
};

EnumRegraPalletHelper.prototype = {
    obterOpcoesPadrao: function () {
        return [
            { text: "Devolução no Ato", value: this.Devolucao },
            { text: "Canhoto Assinado e Carimbado", value: this.CanhotoAssinado },
            { text: "Vale Pallet", value: this.ValePallet },
            { text: "Estoque (Pulmão)", value: this.Estoque }
        ];
    },

    obterOpcoes: function () {
        return [
            { text: "Nenhuma", value: this.Nenhuma }
        ].concat(this.obterOpcoesPadrao());
    },

    obterOpcoesPesquisa: function () {
        return [
            { text: "Todos", value: this.Nenhuma },
            { text: "Empréstimo", value: this.Emprestimo },
            { text: "Transferência", value: this.Transferencia }
        ].concat(this.obterOpcoesPadrao());
    }
};

var EnumRegraPallet = Object.freeze(new EnumRegraPalletHelper());
