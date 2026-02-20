var EnumTipoRodadoVeiculoHelper = function () {
    this.NaoAplicado = '00';
    this.Truck = '01';
    this.Toco = '02';
    this.Cavalo = '03';
    this.Van = '04';
    this.Utilitario = '05';
    this.Outros = '06';
};

EnumTipoRodadoVeiculoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoRodadoVeiculo.NaoAplicado, value: this.NaoAplicado },
            { text: Localization.Resources.Enumeradores.TipoRodadoVeiculo.Truck, value: this.Truck },
            { text: Localization.Resources.Enumeradores.TipoRodadoVeiculo.Toco, value: this.Toco },
            { text: Localization.Resources.Enumeradores.TipoRodadoVeiculo.Cavalo, value: this.Cavalo },
            { text: Localization.Resources.Enumeradores.TipoRodadoVeiculo.Van, value: this.Van },
            { text: Localization.Resources.Enumeradores.TipoRodadoVeiculo.Utilitario, value: this.Utilitario },
            { text: Localization.Resources.Enumeradores.TipoRodadoVeiculo.Outros, value: this.Outros }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Gerais.Geral.Todos, value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoRodadoVeiculo = Object.freeze(new EnumTipoRodadoVeiculoHelper());