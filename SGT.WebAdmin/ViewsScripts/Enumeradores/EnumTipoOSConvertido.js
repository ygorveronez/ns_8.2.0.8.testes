var EnumTipoOSConvertidoHelper = function () {
    this.CargaCheia = 0;
    this.CustoExtra = 1;
    this.Todos = 2;
    this.NaoInformado = 9;
};

EnumTipoOSConvertidoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Carga Cheia", value: this.CargaCheia },
            { text: "Custo Extra", value: this.CustoExtra },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    },
    obterOpcoesPedido: function () {
        return [{ text: "Não Informado", value: this.NaoInformado }].concat(this.obterOpcoes());
    }
}

var EnumTipoOSConvertido = Object.freeze(new EnumTipoOSConvertidoHelper());