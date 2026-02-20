
var EnumTipoEnvioArquivoHelper = function () {
    this.Todos = 0;
    this.Manual = 1;
    this.FTP = 2;
    this.Email = 3;
};

EnumTipoEnvioArquivoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Manual", value: this.Manual },
            { text: "FTP", value: this.FTP }//,
            //{ text: "E-mail", value: this.Email } Descomentar se implementar a leitura por e-mail.
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todos }].concat(this.obterOpcoes());
    }
}

var EnumTipoEnvioArquivo = Object.freeze(new EnumTipoEnvioArquivoHelper());