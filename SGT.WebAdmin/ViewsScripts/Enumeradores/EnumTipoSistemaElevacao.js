var EnumTipoSistemaElevacaoHelper = function () {
    this.Todos = "";
    this.Nenhum = 0;
    this.Elevador = 1;
    this.Escada = 2;
};

EnumTipoSistemaElevacaoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Nenhum", value: this.Nenhum },
            { text: "Elevador", value: this.Elevador },
            { text: "Escada", value: this.Escada }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoSistemaElevacao = Object.freeze(new EnumTipoSistemaElevacaoHelper());