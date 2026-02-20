var EnumSituacaoBoletoTituloHelper = function () {
    this.Todos = 0;
    this.ComBoleto = 1;
    this.SemBoleto = 2;
};

EnumSituacaoBoletoTituloHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Com Boleto", value: this.ComBoleto },
            { text: "Sem Boleto", value: this.SemBoleto }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumSituacaoBoletoTitulo = Object.freeze(new EnumSituacaoBoletoTituloHelper());