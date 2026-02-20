var EnumTiposFreeTimeHelper = function () {
    this.Nenhum = 0;
    this.Coleta = 1;
    this.Fronteira = 2;
    this.Entrega = 3;
    this.LocalParqueamento = 4;
    this.Todos = 99;
};

EnumTiposFreeTimeHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoFreeTimeTabelaFrete.Nenhum, value: this.Nenhum },
            { text: Localization.Resources.Enumeradores.TipoFreeTimeTabelaFrete.Coleta, value: this.Coleta },
            { text: Localization.Resources.Enumeradores.TipoFreeTimeTabelaFrete.Fronteira, value: this.Fronteira },
            { text: Localization.Resources.Enumeradores.TipoFreeTimeTabelaFrete.Entrega, value: this.Entrega },
            { text: Localization.Resources.Enumeradores.TipoFreeTimeTabelaFrete.LocalParqueamento, value: this.LocalParqueamento },
            { text: Localization.Resources.Enumeradores.TipoFreeTimeTabelaFrete.Todos, value: this.Todos },
        ];
    },
};

var EnumTiposFreeTime = Object.freeze(new EnumTiposFreeTimeHelper());