var EnumTipoProvisaoHelper = function () {
    this.Nenhum = 0;
    this.ProvisaoPorNotaFiscal = 1;
    this.ProvisaoPorCTe = 2;
};

EnumTipoProvisaoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Provisão por Nota Fiscal", value: this.ProvisaoPorNotaFiscal },
            { text: "Provisão por CT-e", value: this.ProvisaoPorCTe }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Nenhum", value: this.Todas }].concat(this.obterOpcoes());
    }
};

var EnumTipoProvisao = Object.freeze(new EnumTipoProvisaoHelper());