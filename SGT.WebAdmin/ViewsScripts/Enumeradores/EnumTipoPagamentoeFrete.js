var EnumTipoPagamentoeFreteHelper = function () {
    this.NaoSelecionado = "";
    this.TransferenciaBancaria = 1;
    this.eFrete = 2;
    this.Parceiro = 3;
    this.Outros = 4;
};

EnumTipoPagamentoeFreteHelper.prototype.obterOpcoes = function () {
    return [
        { value: this.NaoSelecionado, text: "Não selecionado" },
        { value: this.TransferenciaBancaria, text: "Transferência Bancária" },
        { value: this.eFrete, text: "eFrete" },
        { value: this.Parceiro, text: "Parceiro" },
        { value: this.Outros, text: "Outros" }
    ];
};

EnumTipoPagamentoeFreteHelper.prototype.obterOpcoesPesquisa = function () {
    return [
        { value: this.NaoSelecionado, text: "Todos" },
        { value: this.Transferencia, text: "Transferência Bancária" },
        { value: this.eFrete, text: "eFrete" },
        { value: this.Parceiro, text: "Parceiro" },
        { value: this.Outros, text: "Outros" }
    ];
};

var EnumTipoPagamentoeFrete = Object.freeze(new EnumTipoPagamentoeFreteHelper());