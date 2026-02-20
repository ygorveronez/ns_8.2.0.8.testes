var EnumTipoGrupoCargaHelper = function () {
    this.Nenhum = 0;
    this.Inbound = 1;
    this.Outbound = 2;
};

EnumTipoGrupoCargaHelper.prototype = {
    obterOpcoes: function () {
        var opcoes = [];
        opcoes.push({ text: "Nenhum", value: this.Nenhum });
        opcoes.push({ text: "Inbound", value: this.Inbound });
        opcoes.push({ text: "Outbound", value: this.Outbound });

        return opcoes;
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: null }].concat(this.obterOpcoes());
    },
    obterOpcoesPesquisaIntegracaoLBC: function () {
        return [{ text: "Selecione uma opção", value: null }].concat(this.obterOpcoes());
    }
};

var EnumTipoGrupoCarga = Object.freeze(new EnumTipoGrupoCargaHelper());
