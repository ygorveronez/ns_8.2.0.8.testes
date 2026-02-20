var EnumTipoPagamentoMDFeHelper = function () {
    this.PIX = 1;
    this.Banco = 2;
    this.Ipef = 3;
};

EnumTipoPagamentoMDFeHelper.prototype.ObterOpcoes = function () {
    return [
        { value: "", text: Localization.Resources.Enumeradores.TipoPagamentoCIOT.NaoSelecionado },
        { value: this.PIX, text: "PIX" },
        { value: this.Banco, text:  "Banco" },
        { value: this.Ipef, text:  "Ipef" } 
    ];
};


EnumTipoPagamentoMDFeHelper.prototype.ObterOpcoesSemNaoSelecionado = function () {
    return [
        { value: this.PIX, text: "PIX" },
        { value: this.Banco, text: "Banco" },
        { value: this.Ipef, text: "Ipef" }
    ];
};


var EnumTipoPagamentoMDFe = Object.freeze(new EnumTipoPagamentoMDFeHelper());