var EnumSituacaoDocumentoHelper = function () {
    this.Todos = "";
    this.ComDocumentos = 2;
    this.SemDocumentos = 1;

};

EnumSituacaoDocumentoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Todos", value: this.Todos },
            { text: "Vinculados", value: this.ComDocumentos },
            { text: "Não Vinculados", value: this.SemDocumentos }
        ];
    },
}

var EnumSituacaoDocumento = Object.freeze(new EnumSituacaoDocumentoHelper());