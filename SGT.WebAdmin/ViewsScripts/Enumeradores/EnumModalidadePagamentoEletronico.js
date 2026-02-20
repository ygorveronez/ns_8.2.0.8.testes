var EnumModalidadePagamentoEletronico = function () {
    this.CC_CreditoContaCorrente = 1;
    this.OP_ChequeOP = 2;
    this.DOC_DOCCompre = 3;
    this.CCR_CreditoConta = 4;
    this.TDC_TEDCip = 5;
    this.TDS_TEDSTR = 6;
    this.TT_TituloTerceiro = 7;
    this.TRB_Tributos = 8;
    this.CCT_ContasConsumoTributo = 9;
    this.PIX = 10;
};

EnumModalidadePagamentoEletronico.prototype = {
    obterOpcoes: function () {
        return [
            { text: "CC - Crédito em conta corrente", value: this.CC_CreditoContaCorrente },
            { text: "OP - Cheque OP", value: this.OP_ChequeOP },
            { text: "DOC - DOC COMPRE", value: this.DOC_DOCCompre },
            { text: "CCR - Crédito Conta", value: this.CCR_CreditoConta },
            { text: "Boleto", value: this.TDC_TEDCip },
            { text: "TDS - TED STR", value: this.TDS_TEDSTR },
            { text: "TT - Titulos de Terceiro", value: this.TT_TituloTerceiro },
            { text: "TRB - Tributos (Sem Código de Barras)", value: this.TRB_Tributos },
            { text: "CCT - Contas de Consumo/Tributos (Com Código de Barras)", value: this.CCT_ContasConsumoTributo },
            { text: "PIX", value: this.PIX }
        ];
    }
};

var EnumModalidadePagamentoEletronico = Object.freeze(new EnumModalidadePagamentoEletronico());