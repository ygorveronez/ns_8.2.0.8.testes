var EnumTipoCapacidadeCarregamentoHelper = function () {
    this.Todos = null;
    this.Peso = 1;
    this.Volume = 2;
    this.CubagemVolume = 3;
};

EnumTipoCapacidadeCarregamentoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoCapacidadeCarregamento.Peso, value: this.Peso },
            { text: Localization.Resources.Enumeradores.TipoCapacidadeCarregamento.Volume, value: this.Volume },
            { text: Localization.Resources.Enumeradores.TipoCapacidadeCarregamento.CubagemVolume, value: this.CubagemVolume }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Gerais.Geral.Todos, value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoCapacidadeCarregamento = Object.freeze(new EnumTipoCapacidadeCarregamentoHelper());
