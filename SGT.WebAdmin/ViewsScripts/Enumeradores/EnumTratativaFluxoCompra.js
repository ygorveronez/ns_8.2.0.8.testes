var EnumTratativaFluxoCompraHelper = function () {
    this.Todos = 0;
    this.Pendente = 1;
    this.Concluido = 2;
};

EnumTratativaFluxoCompraHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Pendente", value: this.Pendente },
            { text: "Concluído", value: this.Concluido },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    },

    obterDescricao: function (valor) {
        switch (valor) {
            case this.Pendente:
                return "Pendente";
            case this.Concluido:
                return "Concluído";
            default:
                return "";
        }
    },

    obterValorAtravesDescricao: function (descricao) {
        switch (descricao) {
            case "Pendente":
                return this.Pendente;
            case "Concluído":
                return this.Concluido;
            default:
                return "";
        }
    }
};

var EnumTratativaFluxoCompra = Object.freeze(new EnumTratativaFluxoCompraHelper());