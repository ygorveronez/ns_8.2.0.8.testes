var EnumTipoManutencaoOrdemServicoFrotaHelper = function () {
    this.Todos = "";
    this.Preventiva = 0;
    this.Corretiva = 1;
    this.PreventivaECorretiva = 2;
};

EnumTipoManutencaoOrdemServicoFrotaHelper.prototype = {
    ObterOpcoes: function () {
        return [
            { text: "Preventiva", value: this.Preventiva },
            { text: "Corretiva", value: this.Corretiva },
            { text: "Corretiva e Preventiva", value: this.PreventivaECorretiva }
        ];
    },
    ObterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.ObterOpcoes());
    }
};

var EnumTipoManutencaoOrdemServicoFrota = Object.freeze(new EnumTipoManutencaoOrdemServicoFrotaHelper());