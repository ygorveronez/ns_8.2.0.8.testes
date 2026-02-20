var EnumTipoMotivoIrregularidadeHelper = function () {
    this.Todos = "";
    this.NaoSelecionado = 0;
    this.ProblemaComercial = 1;
    this.ErroFaturamento = 2;
    this.ProblemaOperacional = 3;
    this.Desacordo = 4;
}

EnumTipoMotivoIrregularidadeHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Não selecionado", value: this.NaoSelecionado },
            { text: "Problema comercial", value: this.ProblemaComercial },
            { text: "Erro faturamento", value: this.ErroFaturamento },
            { text: "Problema operacional", value: this.ProblemaOperacional },
            { text: "Desacordo", value: this.Desacordo },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todos }].concat(this.obterOpcoes());
    },
}

var EnumTipoMotivoIrregularidade = Object.freeze(new EnumTipoMotivoIrregularidadeHelper());