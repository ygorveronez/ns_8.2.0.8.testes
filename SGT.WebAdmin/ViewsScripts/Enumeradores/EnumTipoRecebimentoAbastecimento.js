var EnumTipoRecebimentoAbastecimentoHelper = function () {
    this.Todos = "";
    this.Sistema = 0;
    this.CTF = 1;
    this.WSPosto = 2;
    this.ImportacaoXML = 3;
    this.Integracao = 4;
};

EnumTipoRecebimentoAbastecimentoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Sistema", value: this.Sistema },
            { text: "CTF", value: this.CTF },
            { text: "WS - Posto", value: this.WSPosto },
            { text: "Importação XML", value: this.ImportacaoXML },
            { text: "Integração", value: this.Integracao }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoRecebimentoAbastecimento = Object.freeze(new EnumTipoRecebimentoAbastecimentoHelper());