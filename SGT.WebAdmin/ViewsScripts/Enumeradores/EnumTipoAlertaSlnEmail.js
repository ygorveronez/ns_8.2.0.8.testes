var EnumTipoAlertaSlnEmailHelper = function () {
    this.TempoFaltante = 1;
    this.TempoExcedido = 2;
};

EnumTipoAlertaSlnEmailHelper.prototype = {
    obterOpcoes: function () {
        return [
            { value: this.TempoFaltante, text: Localization.Resources.Enumeradores.TipoAlertaSlnEmail.AlertaTempoFaltante },
            { value: this.TempoExcedido, text: Localization.Resources.Enumeradores.TipoAlertaSlnEmail.AlertaTempoExcedido }
        ];
    },
    obterDescricao: function (tipoAlerta) {
        switch (tipoAlerta) {
            case EnumTipoAlertaSlnEmail.TempoExcedido:
                return Localization.Resources.Enumeradores.TipoAlertaSlnEmail.AlertaTempoExcedido;
            case EnumTipoAlertaSlnEmail.TempoFaltante:
                return Localization.Resources.Enumeradores.TipoAlertaSlnEmail.AlertaTempoFaltante;
        }
    }
};

var EnumTipoAlertaSlnEmail = Object.freeze(new EnumTipoAlertaSlnEmailHelper());