var EnumCategoriaOSHelper = function () {
    this.Todos = null;
    this.Trucking = 0;
    this.Hibrida = 1;
    this.Negocio = 2;
};

EnumCategoriaOSHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Trucking", value: this.Trucking },
            { text: "Híbrida", value: this.Hibrida },
            { text: "Negócio", value: this.Negocio },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    },
    obterOpcoesCadastroPedido: function () {
        return [{ text: "Nenhuma", value: this.Todos }].concat(this.obterOpcoes());
    }
}

var EnumCategoriaOS = Object.freeze(new EnumCategoriaOSHelper());