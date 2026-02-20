const EnumFormaIntegracaoHelper = function () {
    this.Todas = "";
    this.Manual = 1;
    this.OKColeta = 2;
    this.ClienteFTPOKColeta = 3;
    this.ClienteFTP = 4;
    this.ConflitoClienteFTPOKColeta = 5;
    this.NaoRecebido = 6;
    this.OKColetaManual = 7;
    this.ClienteFTPManual = 8;
};

EnumFormaIntegracaoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Manual", value: this.Manual },
            { text: "OK Coleta", value: this.OKColeta },
            { text: "Cliente/FTP + OK Coleta", value: this.ClienteFTPOKColeta },
            { text: "Cliente/FTP", value: this.ClienteFTP },
            { text: "Conflito Cliente/FTP + OK Coleta", value: this.ConflitoClienteFTPOKColeta },
            { text: "Não Recebido", value: this.NaoRecebido },
            { text: "OK Coleta + Manual", value: this.OKColetaManual },
            { text: "Cliente/FTP + Manual", value: this.ClienteFTPManual },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    }
};

const EnumFormaIntegracao = Object.freeze(new EnumFormaIntegracaoHelper());
