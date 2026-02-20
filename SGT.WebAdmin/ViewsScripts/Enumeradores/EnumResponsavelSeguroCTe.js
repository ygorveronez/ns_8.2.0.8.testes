var EnumResponsavelSeguroCTeHelper = function () {
    this.Remetente = 0;
    this.Expedidor = 1;
    this.Recebedor = 2;
    this.Destinatario = 3;
    this.Emitente = 4;
    this.Tomador = 5;
}

EnumResponsavelSeguroCTeHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.ResponsavelSeguroCTe.Remetente, value: this.Remetente },
            { text: Localization.Resources.Enumeradores.ResponsavelSeguroCTe.Expedidor, value: this.Expedidor },
            { text: Localization.Resources.Enumeradores.ResponsavelSeguroCTe.Recebedor, value: this.Recebedor },
            { text: Localization.Resources.Enumeradores.ResponsavelSeguroCTe.Destinatario, value: this.Destinatario },
            { text: Localization.Resources.Enumeradores.ResponsavelSeguroCTe.Emitente, value: this.Emitente },
            { text: Localization.Resources.Enumeradores.ResponsavelSeguroCTe.Tomador, value: this.Tomador }
        ];
    }
}

var EnumResponsavelSeguroCTe = Object.freeze(new EnumResponsavelSeguroCTeHelper());
