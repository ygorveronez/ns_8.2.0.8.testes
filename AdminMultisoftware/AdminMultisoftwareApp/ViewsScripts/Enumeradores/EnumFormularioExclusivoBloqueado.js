var EnumFormularioExclusivoBloqueadoHelper = function () {
    this.Todos = "";
    this.Nao = false;
    this.Sim = true;
};

EnumFormularioExclusivoBloqueadoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Todos", value: this.Todos },
            { text: "Sim", value: this.Sim },
            { text: "Não", value: this.Nao }
        ];
    },
}

var EnumFormularioExclusivoBloqueado = Object.freeze(new EnumFormularioExclusivoBloqueadoHelper());
