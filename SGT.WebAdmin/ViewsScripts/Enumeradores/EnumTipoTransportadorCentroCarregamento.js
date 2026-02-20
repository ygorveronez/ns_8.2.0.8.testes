var EnumTipoTransportadorCentroCarregamentoHelper = function () {
    this.Nenhum = 0;
    this.Todos = 1;
    this.TodosComTipoVeiculoCarga = 2;
    this.TodosCentroCarregamento = 3;
    this.TodosCentroCarregamentoComTipoVeiculoCarga = 4;
    this.PorPrioridadeDeRota = 5;
    this.PorGrupoRegional = 6;
    this.TransportadorExclusivo = 7;
}

EnumTipoTransportadorCentroCarregamentoHelper.prototype = {
    obterOpcoes: function () {
        var opcoes = [];

        opcoes.push({ text: Localization.Resources.Gerais.Geral.Todos, value: this.Todos });
        opcoes.push({ text: Localization.Resources.Enumeradores.TipoTransportadorCentroCarregamento.TodosComTipoDeVeiculoDaCarga, value: this.TodosComTipoVeiculoCarga });
        opcoes.push({ text: Localization.Resources.Enumeradores.TipoTransportadorCentroCarregamento.TodosDoCentroDeCarregamento, value: this.TodosCentroCarregamento });
        opcoes.push({ text: Localization.Resources.Enumeradores.TipoTransportadorCentroCarregamento.TodosDoCentroDeCarregamentoComTipoDeVeiculoDaCarga, value: this.TodosCentroCarregamentoComTipoVeiculoCarga });

        if (!_CONFIGURACAO_TMS.DisponibilizarCargaAutomaticamenteParaTransportadorComMenorValorFreteTabela)
            opcoes.push({ text: Localization.Resources.Enumeradores.TipoTransportadorCentroCarregamento.PorPrioridadeDeTransportadorNaRota, value: this.PorPrioridadeDeRota });

        opcoes.push({ text: Localization.Resources.Enumeradores.TipoTransportadorCentroCarregamento.PorGrupoRegional, value: this.PorGrupoRegional });

        return opcoes;
    },
    obterOpcoesPermissaoLiberarCargaTransportadorExclusivo: function () {
        return this.obterOpcoes().concat([{ text: Localization.Resources.Enumeradores.TipoTransportadorCentroCarregamento.TransportadorExclusivo, value: this.TransportadorExclusivo }]);
    },
    obterOpcoesSecundario: function () {
        var opcoes = [];

        opcoes.push({ text: Localization.Resources.Gerais.Geral.Nenhum, value: this.Nenhum });
        opcoes.push({ text: Localization.Resources.Gerais.Geral.Todos, value: this.Todos });
        opcoes.push({ text: Localization.Resources.Enumeradores.TipoTransportadorCentroCarregamento.TodosComTipoDeVeiculoDaCarga, value: this.TodosComTipoVeiculoCarga });
        opcoes.push({ text: Localization.Resources.Enumeradores.TipoTransportadorCentroCarregamento.TodosDoCentroDeCarregamento, value: this.TodosCentroCarregamento });
        opcoes.push({ text: Localization.Resources.Enumeradores.TipoTransportadorCentroCarregamento.TodosDoCentroDeCarregamentoComTipoDeVeiculoDaCarga, value: this.TodosCentroCarregamentoComTipoVeiculoCarga });
        opcoes.push({ text: Localization.Resources.Enumeradores.TipoTransportadorCentroCarregamento.PorGrupoRegional, value: this.PorGrupoRegional });

        return opcoes;
    },
}

var EnumTipoTransportadorCentroCarregamento = Object.freeze(new EnumTipoTransportadorCentroCarregamentoHelper());