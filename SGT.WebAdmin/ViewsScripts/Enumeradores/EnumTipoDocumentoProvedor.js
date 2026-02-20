var EnumTipoDocumentoProvedorHelper = function () {
    this.Nenhum = 0;
    this.CTe = 1;
    this.CTeComplementar = 2;
    this.NFSe = 3;
};

EnumTipoDocumentoProvedorHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "CTe", value: this.CTe },
            { text: "CTe Complementar", value: this.CTeComplementar },
            { text: "NFSe", value: this.NFSe },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Nenhum", value: this.Nenhum }].concat(this.obterOpcoes());
    },
    obterOpcoesPesquisaRelatorio: function () {
        return [{ text: "Todos", value: this.Nenhum }].concat(this.obterOpcoes());
    }
};

var EnumTipoDocumentoProvedor = Object.freeze(new EnumTipoDocumentoProvedorHelper());