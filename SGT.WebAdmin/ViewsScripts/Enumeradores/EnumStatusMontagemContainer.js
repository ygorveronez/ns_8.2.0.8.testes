var EnumStatusMontagemContainerHelper = function () {
    this.Todos = 0;
    this.Aberto = 1;
    this.Finalizado = 2;
    this.Expedido = 3;
};

EnumStatusMontagemContainerHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Aberto", value: this.Aberto },
            { text: "Finalizado", value: this.Finalizado },
            { text: "Expedido", value: this.Expedido }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    },
    obterDescricao: function (status) {
        switch (status) {
            case this.Aberto:
                return "Aberto";
            case this.Finalizado:
                return "Finalizado";
            case this.Expedido:
                return "Expedido";
            default:
                return "";
        };
    }
};

var EnumStatusMontagemContainer = Object.freeze(new EnumStatusMontagemContainerHelper());