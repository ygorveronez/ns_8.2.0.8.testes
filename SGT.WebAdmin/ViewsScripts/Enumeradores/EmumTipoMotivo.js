
var EnumTipoMotivoHelper = function () {
    this.Todos = "";
    this.RejeicaoDadosNFeColeta = 1;
};

EnumTipoMotivoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Rejeição de dados da NF-e da coleta", value: this.RejeicaoDadosNFeColeta }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    },

};

var EnumTipoMotivo = Object.freeze(new EnumTipoMotivoHelper());