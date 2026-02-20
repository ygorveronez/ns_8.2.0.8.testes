var EnumGatilhoAutomatizacaoNaoComparecimentoHelper = function () {
    this.Todos = "";
    this.CargaSemVeiculoInformado = 1;
    this.CargaNaoAgendada = 2;
    this.VeiculoSemRegistroChegada = 3;
};

EnumGatilhoAutomatizacaoNaoComparecimentoHelper.prototype = {
    obterDescricao: function (valor) {
        switch (valor) {
            case this.CargaNaoAgendada: return Localization.Resources.Enumeradores.GatilhoAutomatizacaoNaoComparecimento.CargaNaoAgendada;
            case this.CargaSemVeiculoInformado: return Localization.Resources.Enumeradores.GatilhoAutomatizacaoNaoComparecimento.CargaSemVeiculoInformado;
            case this.VeiculoSemRegistroChegada: return Localization.Resources.Enumeradores.GatilhoAutomatizacaoNaoComparecimento.VeiculoSemRegistroDeChegada;
            default: return "";
        }
    },
    obterOpcoes: function () {
        return [
            { text: this.obterDescricao(this.CargaNaoAgendada), value: this.CargaNaoAgendada },
            { text: this.obterDescricao(this.CargaSemVeiculoInformado), value: this.CargaSemVeiculoInformado },
            { text: this.obterDescricao(this.VeiculoSemRegistroChegada), value: this.VeiculoSemRegistroChegada }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Gerais.Geral.Todos, value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumGatilhoAutomatizacaoNaoComparecimento = Object.freeze(new EnumGatilhoAutomatizacaoNaoComparecimentoHelper());
