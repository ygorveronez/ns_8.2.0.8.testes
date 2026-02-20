var EnumMotivoServicoVeiculoHelper = function () {
    this.Todos = "";
    this.Outros = 0;
    this.Conserto = 1;
    this.Reforma = 2;
    this.ConsertoEReforma = 3;
};

EnumMotivoServicoVeiculoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Outros", value: this.Outros },
            { text: "Conserto", value: this.Conserto },
            { text: "Reforma", value: this.Reforma },
            { text: "Conserto + Reforma", value: this.ConsertoEReforma }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumMotivoServicoVeiculo = Object.freeze(new EnumMotivoServicoVeiculoHelper());