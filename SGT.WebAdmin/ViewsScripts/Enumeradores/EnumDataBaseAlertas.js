var EnumDataBaseHelper = function () {
    this.PrevisaoEntrega = 0;
    this.DataAgendamento = 1;

}

EnumDataBaseHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Cargas.ConfiguracaoAlertaCarga.PrevisaoEntrega, value: this.PrevisaoEntrega },
            { text: Localization.Resources.Cargas.ConfiguracaoAlertaCarga.DataAgendamento, value: this.DataAgendamento },
        ];
    }
}

var EnumDataBaseAlertas = Object.freeze(new EnumDataBaseHelper());

