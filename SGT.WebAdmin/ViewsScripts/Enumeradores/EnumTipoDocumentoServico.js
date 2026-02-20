var EnumTipoDocumentoServicoHelper = function () {
    this.Todas = "";
    this.NotaFiscal = 1;
    this.CupomFiscal = 2;
    this.Contrato = 3;
    this.Recibo = 4;
    this.NotaFiscalConjugada = 5;
    this.NotaFiscalServicoEletronica = 7;
    this.CupomFiscalConjugado = 8;
};

EnumTipoDocumentoServicoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "1 - Nota Fiscal", value: this.NotaFiscal },
            { text: "2 - Cupom Fiscal", value: this.CupomFiscal },
            { text: "3 - Contrato", value: this.Contrato },
            { text: "4 - Recibo", value: this.Recibo },
            { text: "5 - Nota Fiscal Conjugada", value: this.NotaFiscalConjugada },
            { text: "7 - Nota Fiscal de Serviço Eletrônica", value: this.NotaFiscalServicoEletronica },
            { text: "8 - Cupom Fiscal Conjugado", value: this.CupomFiscalConjugado }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todas }].concat(this.obterOpcoes());
    },
    obterOpcoesCadastro: function () {
        return [{ text: "Nenhum", value: this.Todas }].concat(this.obterOpcoes());
    }
};

var EnumTipoDocumentoServico = Object.freeze(new EnumTipoDocumentoServicoHelper());