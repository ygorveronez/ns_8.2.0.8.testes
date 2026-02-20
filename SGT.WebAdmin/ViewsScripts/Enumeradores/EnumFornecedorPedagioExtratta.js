var EnumFornecedorPedagioExtrattaHelper = function () {
    this.Todos = "";
    this.Moedeiro = 1;
    this.ViaFacil = 2;
    this.MoveMais = 3;
    this.Veloe = 4;
};

EnumFornecedorPedagioExtrattaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Moedeiro (Extratta)", value: this.Moedeiro },
            { text: "Via Fácil (Sem Parar)", value: this.ViaFacil },
            { text: "Move Mais", value: this.MoveMais },
            { text: "Veloe", value: this.Veloe }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumFornecedorPedagioExtratta = Object.freeze(new EnumFornecedorPedagioExtrattaHelper());