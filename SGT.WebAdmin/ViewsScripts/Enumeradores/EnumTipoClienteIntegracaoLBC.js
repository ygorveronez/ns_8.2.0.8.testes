var EnumTipoClienteIntegracaoLBCHelper = function () {
    this.Nenhum = 0;
    this.Aeroporto = 1;
    this.Porto = 2;
    this.TP = 3;
    this.Fornecedor = 4;
};

EnumTipoClienteIntegracaoLBCHelper.prototype = {
    obterOpcoes: function () {
        var opcoes = [];
        opcoes.push({ text: "Nenhum", value: this.Nenhum });
        opcoes.push({ text: "Aeroporto", value: this.Aeroporto });
        opcoes.push({ text: "Porto", value: this.Porto });
        opcoes.push({ text: "Transit Point", value: this.TP });
        opcoes.push({ text: "Fornecedor", value: this.Fornecedor });

        return opcoes;
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: null }].concat(this.obterOpcoes());
    }
};

var EnumTipoClienteIntegracaoLBC = Object.freeze(new EnumTipoClienteIntegracaoLBCHelper());
