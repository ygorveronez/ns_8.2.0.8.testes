var EnumTipoAbrangenciaExcecaoCapacidadeCarregamentoHelper = function () {
    this.Dia = 0;
    this.Periodo = 1;
}

EnumTipoAbrangenciaExcecaoCapacidadeCarregamentoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoAbrangenciaExcecaoCapacidadeCarregamento.Dia, value: this.Dia },
            { text: Localization.Resources.Enumeradores.TipoAbrangenciaExcecaoCapacidadeCarregamento.Periodo, value: this.Periodo }
        ];
    }
}

var EnumTipoAbrangenciaExcecaoCapacidadeCarregamento = Object.freeze(new EnumTipoAbrangenciaExcecaoCapacidadeCarregamentoHelper());