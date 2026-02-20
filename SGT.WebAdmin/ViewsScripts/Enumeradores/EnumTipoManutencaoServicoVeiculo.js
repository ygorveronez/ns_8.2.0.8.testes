var EnumTipoManutencaoServicoVeiculoHelper = function () {
    this.Todos = "";
    this.Outros = 0;
    this.Preventiva = 1;
    this.Corretiva = 2;
    this.Preditiva = 3;
    this.Detectiva = 4;
    this.PreventivaECorretiva = 5;
};

EnumTipoManutencaoServicoVeiculoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Outros", value: this.Outros },
            { text: "Preventiva", value: this.Preventiva },
            { text: "Corretiva", value: this.Corretiva }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoManutencaoServicoVeiculo = Object.freeze(new EnumTipoManutencaoServicoVeiculoHelper());