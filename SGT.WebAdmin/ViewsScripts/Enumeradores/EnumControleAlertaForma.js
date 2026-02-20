var EnumControleAlertaFormaHelper = function () {
    this.PainelAlerta = 1;
    this.Notificacao = 2;
    this.Email = 3;
};

EnumControleAlertaFormaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.ControleAlertaForma.PainelDeAlerta, value: this.PainelAlerta },
            { text: Localization.Resources.Enumeradores.ControleAlertaForma.Notificacao, value: this.Notificacao },
            { text: Localization.Resources.Enumeradores.ControleAlertaForma.Email, value: this.Email }
        ];
    }
};

var EnumControleAlertaForma = Object.freeze(new EnumControleAlertaFormaHelper());