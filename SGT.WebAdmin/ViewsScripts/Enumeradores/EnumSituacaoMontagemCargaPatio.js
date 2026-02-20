var EnumSituacaoMontagemCargaPatioHelper = function () {
    this.Todas = "";
    this.AguardandoMontagemCarga = 1;
    this.MontagemCargaFinalizada = 2;
};

EnumSituacaoMontagemCargaPatioHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Aguardando Montagem de Carga", value: this.AguardandoMontagemCarga },
            { text: "Montagem de Carga Finalizada", value: this.MontagemCargaFinalizada }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    }
}

var EnumSituacaoMontagemCargaPatio = Object.freeze(new EnumSituacaoMontagemCargaPatioHelper());
