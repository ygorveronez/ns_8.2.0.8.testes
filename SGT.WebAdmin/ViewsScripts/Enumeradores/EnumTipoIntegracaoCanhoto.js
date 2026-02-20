var EnumTipoIntegracaoCanhotoHelper = function () {
    this.Todos = -1;
    this.Nenhum = 0;
    this.FTP = 1;
};

EnumTipoIntegracaoCanhotoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Nenhum", value: this.Nenhum },
            { text: "FTP", value: this.FTP }
            
        ];
    }
};

var EnumTipoIntegracaoCanhoto = Object.freeze(new EnumTipoIntegracaoCanhotoHelper());