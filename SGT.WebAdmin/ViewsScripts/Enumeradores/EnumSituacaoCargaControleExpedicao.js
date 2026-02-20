var EnumSituacaoCargaControleExpedicaoHelper = function () {
    this.Todas = 0;
    this.AguardandoLiberacao = 1;
    this.Liberada = 2;
    this.PlacaDivergente = 3;
    this.AgInicioCarregamento = 4;
}

EnumSituacaoCargaControleExpedicaoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Ag. Inicio Carregamento", value: this.AgInicioCarregamento },
            { text: "Ag. Liberação", value: this.AguardandoLiberacao },
            { text: "Com divergência", value: this.PlacaDivergente },
            { text: "Liberada", value: this.Liberada }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    }
};

var EnumSituacaoCargaControleExpedicao = Object.freeze(new EnumSituacaoCargaControleExpedicaoHelper());