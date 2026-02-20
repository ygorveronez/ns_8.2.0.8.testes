var EnumTipoJanelaCarregamentoHelper = function () {
    this.Calendario = 0;
    this.Tabela = 1;
}

EnumTipoJanelaCarregamentoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoJanelaCarregamento.Calendario, value: this.Calendario },
            { text: Localization.Resources.Enumeradores.TipoJanelaCarregamento.Tabela, value: this.Tabela }
        ];
    }
}

var EnumTipoJanelaCarregamento = Object.freeze(new EnumTipoJanelaCarregamentoHelper());