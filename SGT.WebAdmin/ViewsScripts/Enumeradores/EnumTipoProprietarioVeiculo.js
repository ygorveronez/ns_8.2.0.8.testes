let EnumTipoProprietarioVeiculoHelper = function () {
    this.TACAgregado = 0;
    this.TACIndependente = 1;
    this.Outros = 2;
    this.NaoAplicado = 3;
    this.Todos = 4;
};

EnumTipoProprietarioVeiculoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoProprietarioVeiculo.TACAgregado, value: this.TACAgregado },
            { text: Localization.Resources.Enumeradores.TipoProprietarioVeiculo.TACIndependente, value: this.TACIndependente },
            { text: Localization.Resources.Enumeradores.TipoProprietarioVeiculo.Outros, value: this.Outros },
            { text: Localization.Resources.Enumeradores.TipoProprietarioVeiculo.NaoAplicado, value: this.NaoAplicado }
        ];
    },
    obterOpcoesPessoaFisica: function () {
        return [
            { text: "Selecione...", value: "" },
            { text: Localization.Resources.Enumeradores.TipoProprietarioVeiculo.TACAgregado, value: this.TACAgregado },
            { text: Localization.Resources.Enumeradores.TipoProprietarioVeiculo.TACIndependente, value: this.TACIndependente }
        ];
    },
    obterOpcoesPessoaJuridica: function () {
        return [
            { text: "Selecione...", value: "" },
            { text: Localization.Resources.Enumeradores.TipoProprietarioVeiculo.EquiparadoTAC, value: this.EquiparadoTAC },
            { text: Localization.Resources.Enumeradores.TipoProprietarioVeiculo.NaoAplicado, value: this.NaoAplicado }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Gerais.Geral.Todos, value: "" }].concat(this.obterOpcoes());
    },
    obterOpcoesMotivoChamado: function () {
        return [{ text: Localization.Resources.Gerais.Geral.Todos, value: this.Todos }].concat(this.obterOpcoes());
    }
};

let EnumTipoProprietarioVeiculo = Object.freeze(new EnumTipoProprietarioVeiculoHelper());