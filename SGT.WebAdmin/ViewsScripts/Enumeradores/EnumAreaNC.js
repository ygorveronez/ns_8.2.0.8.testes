var EnumAreaNCHelper = function () {
    this.Todos = null;
    this.NaoSelecionado = 0;
    this.Transporte = 1;
    this.Planejamento = 2;
    this.Comex = 3;
    this.Entradas = 4;
};

EnumAreaNCHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Transporte", value: this.Transporte },
            { text: "Planejamento", value: this.Planejamento },
            { text: "Comex", value: this.Comex },
            { text: "Entradas", value: this.Entradas },
        ];
    },
    obterOpcoesNaoSelecionado: function () {
        return [
            { text: "Transporte", value: this.Transporte },
            { text: "Planejamento", value: this.Planejamento },
            { text: "Comex", value: this.Comex },
            { text: "Entradas", value: this.Entradas },
            { text: "Nenhum", value: this.NaoSelecionado },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumAreaGrupoNC = Object.freeze(new EnumAreaNCHelper());