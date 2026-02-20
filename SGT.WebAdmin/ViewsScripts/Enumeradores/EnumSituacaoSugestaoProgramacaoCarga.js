var EnumSituacaoSugestaoProgramacaoCargaHelper = function () {
    this.Todas = "";
    this.Gerada = 1;
    this.Publicada = 2;
    this.Cancelada = 3;
}

EnumSituacaoSugestaoProgramacaoCargaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Cancelada", value: this.Cancelada },
            { text: "Gerada", value: this.Gerada },
            { text: "Publicada", value: this.Publicada }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    }
}

var EnumSituacaoSugestaoProgramacaoCarga = Object.freeze(new EnumSituacaoSugestaoProgramacaoCargaHelper());
