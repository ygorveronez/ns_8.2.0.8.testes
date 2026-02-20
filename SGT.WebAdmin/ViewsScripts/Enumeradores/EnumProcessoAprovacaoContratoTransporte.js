var EnumProcessoAprovacaoContratoTransporteHelper = function () {
    this.AprovacaoDiretaVisivelFornecedor = 0;
    this.AprovacaoDiretaNaoVisivelFornecedor = 1;
};

EnumProcessoAprovacaoContratoTransporteHelper.prototype = {
    ObterOpcoes: function () {
        return [
            { text: "Aprovação Direta - Visível ao fornecedor", value: this.AprovacaoDiretaVisivelFornecedor },
            { text: "Aprovação Direta - Não visível ao fornecedor", value: this.AprovacaoDiretaNaoVisivelFornecedor },
        ];
    },
    ObterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: "" }].concat(this.ObterOpcoes());
    }
}

var EnumProcessoAprovacaoContratoTransporte = Object.freeze(new EnumProcessoAprovacaoContratoTransporteHelper());