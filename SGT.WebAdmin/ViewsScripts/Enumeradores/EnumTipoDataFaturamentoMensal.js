var EnumTipoDataFaturamentoMensalHelper = function () {
    this.Todos = "";
    this.DataEmissao = 1;
    this.DataVencimento = 2;
    this.DataQuitacao = 3;
};

EnumTipoDataFaturamentoMensalHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Data Emissão", value: this.DataEmissao },
            { text: "Data Vencimento", value: this.DataVencimento },
            { text: "Data Quitação", value: this.DataQuitacao }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoDataFaturamentoMensal = Object.freeze(new EnumTipoDataFaturamentoMensalHelper());