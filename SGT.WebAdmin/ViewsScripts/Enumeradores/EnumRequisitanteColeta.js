var EnumRequisitanteColetaHelper = function () {
    this.Remetente = 0;
    this.Destinatario = 1;
    this.Outros = 2;
    this.Redespacho = 3;
    this.Terceiro = 4;
    this.Recebedor = 5;
};

EnumRequisitanteColetaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.RequisitanteColeta.Remetente, value: this.Remetente },
            { text: Localization.Resources.Enumeradores.RequisitanteColeta.Destinatario, value: this.Destinatario },
            { text: Localization.Resources.Enumeradores.RequisitanteColeta.Outros, value: this.Outros},
            { text: Localization.Resources.Enumeradores.RequisitanteColeta.Redespacho, value: this.Redespacho },
            { text: Localization.Resources.Enumeradores.RequisitanteColeta.Terceiro, value: this.Terceiro },
            { text: Localization.Resources.Enumeradores.RequisitanteColeta.Recebedor, value: this.Recebedor }
        ];
    },
}

var EnumRequisitanteColeta = Object.freeze(new EnumRequisitanteColetaHelper());