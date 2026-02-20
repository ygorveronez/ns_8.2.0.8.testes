var EnumTipoTransportadorTerceiroCentroCarregamentoHelper = function () {
    this.Todos = 1;
    this.TodosComTipoVeiculoCarga = 2;
    this.TodosCentroCarregamento = 3;
    this.TodosCentroCarregamentoComTipoVeiculoCarga = 4;
    this.PorPrioridadeFilaCarregamento = 8;

}

EnumTipoTransportadorTerceiroCentroCarregamentoHelper.prototype = {
    obterOpcoes: function () {
        var opcoes = [];

        opcoes.push({ text: Localization.Resources.Gerais.Geral.Todos, value: this.Todos });
        opcoes.push({ text: Localization.Resources.Enumeradores.TipoTransportadorCentroCarregamento.TodosComTipoDeVeiculoDaCarga, value: this.TodosComTipoVeiculoCarga });
        opcoes.push({ text: Localization.Resources.Enumeradores.TipoTransportadorCentroCarregamento.TodosDoCentroDeCarregamento, value: this.TodosCentroCarregamento });
        opcoes.push({ text: Localization.Resources.Enumeradores.TipoTransportadorCentroCarregamento.TodosDoCentroDeCarregamentoComTipoDeVeiculoDaCarga, value: this.TodosCentroCarregamentoComTipoVeiculoCarga });
        opcoes.push({ text: Localization.Resources.Enumeradores.TipoTransportadorCentroCarregamento.PorFilaCarregamento, value: this.PorPrioridadeFilaCarregamento });

        return opcoes;
    }
    ,obterOpcoesSecundario: function () {
        var opcoes = [];

        opcoes.push({ text: Localization.Resources.Gerais.Geral.Todos, value: this.Todos });
        opcoes.push({ text: Localization.Resources.Enumeradores.TipoTransportadorCentroCarregamento.TodosComTipoDeVeiculoDaCarga, value: this.TodosComTipoVeiculoCarga });
        opcoes.push({ text: Localization.Resources.Enumeradores.TipoTransportadorCentroCarregamento.TodosDoCentroDeCarregamento, value: this.TodosCentroCarregamento });
        opcoes.push({ text: Localization.Resources.Enumeradores.TipoTransportadorCentroCarregamento.TodosDoCentroDeCarregamentoComTipoDeVeiculoDaCarga, value: this.TodosCentroCarregamentoComTipoVeiculoCarga });

        return opcoes;
    },
}

var EnumTipoTransportadorTerceiroCentroCarregamento = Object.freeze(new EnumTipoTransportadorTerceiroCentroCarregamentoHelper());