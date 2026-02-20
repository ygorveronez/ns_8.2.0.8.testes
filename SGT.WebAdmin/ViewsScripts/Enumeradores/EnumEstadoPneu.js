var EnumEstadoPneuHelper = function () {
    this.Todos = "";
    this.PneuNovo = 1;
    this.PneuUsado = 2;
    this.PneuRecauchutadoNovo = 3;
    this.PneuRecauchutadoUsado = 4;
};

EnumEstadoPneuHelper.prototype = {
    obterDescricao: function (estadoPneu) {
        switch (estadoPneu) {
            case this.PneuNovo: return "Pneu Novo";
            case this.PrimeiroRecape: return "Pneu Usado";
            case this.SegundoRecape: return "Pneu Recauchutado Novo";
            case this.TerceiroRecape: return "Pneu Recauchutado Usado";
            default: return undefined;
        }
    },
    obterOpcoes: function () {
        return [
            { text: "Pneu Novo", value: this.PneuNovo },
            { text: "Pneu Usado", value: this.PneuUsado },
            { text: "Pneu Recauchutado Novo", value: this.PneuRecauchutadoNovo },
            { text: "Pneu Recauchutado Usado", value: this.PneuRecauchutadoUsado },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [
            { text: "Pneus Novos", value: this.PneuNovo },
            { text: "Pneus Usados", value: this.PneuUsado },
            { text: "Pneus Recauchutados Novos", value: this.PneuRecauchutadoNovo },
            { text: "Pneus Recauchutados Usados", value: this.PneuRecauchutadoUsado },
        ];
    },
    obterOpcoesPesquisaSimples: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoesPesquisa());
    },
    
};

var EnumEstadoPneu = Object.freeze(new EnumEstadoPneuHelper());