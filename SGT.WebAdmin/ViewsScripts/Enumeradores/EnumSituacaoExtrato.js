var EnumSituacaoExtratoHelper = function () {
    this.Todas = 0;
    this.SemExtrato = 1;
    this.EmExtrato = 2;
    this.SemValePedagio = 3;
};

EnumSituacaoExtratoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Sem Extrato", value: this.SemExtrato },
            { text: "Em Extrato", value: this.EmExtrato },
            { text: "Sem Vale Pedagio", value: this.SemValePedagio },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    }
};

var EnumSituacaoExtrato = Object.freeze(new EnumSituacaoExtratoHelper());