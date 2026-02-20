var EnumTipoDocumentoGeradoHelper = function () {
    this.Todos = "";
    this.SomenteCargas = 1;
    this.SomenteOcorrencias = 2;
};

EnumTipoDocumentoGeradoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Somente Cargas", value: this.SomenteCargas },
            { text: "Somente Ocorrências", value: this.SomenteOcorrencias },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Cargas e Ocorrências", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoDocumentoGerado = Object.freeze(new EnumTipoDocumentoGeradoHelper());