var EnumModuloExclusivoBloqueadoHelper = function () {
    this.Todos = "";
    this.Nao = false;
    this.Sim = true;
};

EnumModuloExclusivoBloqueadoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Todos", value: this.Todos },
            { text: "Sim", value: this.Sim },
            { text: "Não", value: this.Nao }
        ];
    },
}

var EnumModuloExclusivoBloqueado = Object.freeze(new EnumModuloExclusivoBloqueadoHelper());
