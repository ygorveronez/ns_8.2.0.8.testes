var EnumTipoOSHelper = function () {
    this.CargaCheia = 0;
    this.Armazenagem = 1;
    this.Estadia = 2;
    this.OSAvulsa = 3;
    this.Todos = 4;
    this.NaoInformado = 9;
};

EnumTipoOSHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Carga Cheia", value: this.CargaCheia },
            { text: "Armazenagem", value: this.Armazenagem },
            { text: "Estadia", value: this.Estadia },
            { text: "OS Avulsa", value: this.OSAvulsa },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    },
    obterOpcoesPedido: function () {
        return [{ text: "Não Informado", value: this.NaoInformado }].concat(this.obterOpcoes());
    }
}

var EnumTipoOS = Object.freeze(new EnumTipoOSHelper());