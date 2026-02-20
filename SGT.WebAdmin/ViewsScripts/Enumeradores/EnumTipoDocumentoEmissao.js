var EnumTipoDocumentoEmissaoHelper = function () {
    this.Nenhum = -1;
    this.CTe = 0;
    this.NFSe = 1;
    this.Outros = 2;
    this.NFS = 3;
    this.Subcontratacao = 4;
    this.Todos = 99;
}

EnumTipoDocumentoEmissaoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "CT-e", value: this.CTe },
            { text: "NFS-e", value: this.NFSe }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoDocumentoEmissao = Object.freeze(new EnumTipoDocumentoEmissaoHelper());