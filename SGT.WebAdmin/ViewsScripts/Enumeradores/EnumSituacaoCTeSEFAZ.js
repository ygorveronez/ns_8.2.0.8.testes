var EnumSituacaoCTeSEFAZHelper = function () {
    this.Autorizada = 1;
    this.Cancelada = 2;
    this.Denegada = 3;
    this.Rejeitada = 4;
    this.Pendente = 5;
    this.Enviada = 6;
    this.Inutilizada = 7;
    this.EmDigitacao = 8;
    this.EmCancelamento = 9;
    this.EmInutilizacao = 10;
    this.Anulado = 11;
    this.AguardandoAssinatura = 12;
    this.AguardandoAssinaturaCancelamento = 13;
    this.AguardandoAssinaturaInutilizacao = 14;
    this.AguardandoEmissaoEmail = 15;
    this.AguardandoNFSe = 16;
    this.ContingenciaFSDA = 17;
}

EnumSituacaoCTeSEFAZHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Autorizado", value: this.Autorizada },
            { text: "Cancelado", value: this.Cancelada },
            { text: "Denegado", value: this.Denegada },
            { text: "Rejeitado", value: this.Rejeitada },
            { text: "Pendente", value: this.Pendente },
            { text: "Enviado", value: this.Enviada },
            { text: "Inutilizado", value: this.Inutilizada },
            { text: "Em Digitação", value: this.EmDigitacao },
            { text: "Em Cancelamento", value: this.EmCancelamento },
            { text: "Em Inutilização", value: this.EmInutilizacao },
            { text: "Anulado", value: this.Anulado },
            { text: "Ag. Assinatura", value: this.AguardandoAssinatura },
            { text: "Ag. Assinatura Cancelamento", value: this.AguardandoAssinaturaCancelamento },
            { text: "Ag. Assinatura Inutilização", value: this.AguardandoAssinaturaInutilizacao },
            { text: "Ag. Emissão e-mail", value: this.AguardandoEmissaoEmail },
            { text: "Ag. NFS-e", value: this.AguardandoNFSe },
            { text: "Contingência FSDA", value: this.ContingenciaFSDA }
        ];
    }
}

var EnumSituacaoCTeSEFAZ = Object.freeze(new EnumSituacaoCTeSEFAZHelper());