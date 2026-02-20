var EnumTipoVeiculoHelper = function () {
    this.Todos = '-1';
    this.Tracao = '0';
    this.Reboque = '1';
};

EnumTipoVeiculoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoVeiculo.Tracao, value: this.Tracao },
            { text: Localization.Resources.Enumeradores.TipoVeiculo.Reboque, value: this.Reboque },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Gerais.Geral.Todos, value: this.Todos }].concat(this.obterOpcoes());
    }


};

var EnumTipoVeiculo = Object.freeze(new EnumTipoVeiculoHelper());




var EnumTipoVeiculoLetraHelper = function () {
    this.Todos = 'A';
    this.Propria = 'P';
    this.Terceiros = 'T';
};

EnumTipoVeiculoLetraHelper.prototype = {

    obterOpcoesPropriedade: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoVeiculo.Propria, value: this.Propria },
            { text: Localization.Resources.Enumeradores.TipoVeiculo.Terceiros, value: this.Terceiros },
        ];
    },
    obterOpcoesPesquisaPropriedade: function () {
        return [{ text: Localization.Resources.Gerais.Geral.Todos, value: this.Todos }].concat(this.obterOpcoesPropriedade());
    }

};

var EnumTipoVeiculoLetra = Object.freeze(new EnumTipoVeiculoLetraHelper());

var EnumTipoVeiculoProprietarioHelper = function () {
    this.Todos = 0;
    this.Proprio = 1;
    this.Terceiro = 2;
};

EnumTipoVeiculoProprietarioHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Próprio", value: this.Proprio },
            { text: "Terceiro", value: this.Terceiro }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoVeiculoProprietario = Object.freeze(new EnumTipoVeiculoProprietarioHelper());