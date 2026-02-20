var EnumTipoAutomatizacaoTipoCargaHelper = function () {
    this.NaoHabilitado = 0;
    this.PorPrioridade = 1;
    this.PorValor = 2;
};

EnumTipoAutomatizacaoTipoCargaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoAutomatizacaoTipoCarga.Desabilitado, value: this.NaoHabilitado },
            { text: Localization.Resources.Enumeradores.TipoAutomatizacaoTipoCarga.PorPrioridade, value: this.PorPrioridade },
            { text: Localization.Resources.Enumeradores.TipoAutomatizacaoTipoCarga.PorValor, value: this.PorValor }
        ];
    }
};

var EnumTipoAutomatizacaoTipoCarga = Object.freeze(new EnumTipoAutomatizacaoTipoCargaHelper());