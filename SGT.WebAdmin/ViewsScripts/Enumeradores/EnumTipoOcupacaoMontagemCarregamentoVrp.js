var EnumTipoOcupacaoMontagemCarregamentoVrpHelper = function () {
    this.Peso = 0;
    this.MetroCubico = 1;
    this.Pallet = 2;
}

EnumTipoOcupacaoMontagemCarregamentoVrpHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoOcupacaoMontagemCarregamentoVrp.Peso, value: this.Peso },
            { text: Localization.Resources.Enumeradores.TipoOcupacaoMontagemCarregamentoVrp.MetroCubico, value: this.MetroCubico },
            { text: Localization.Resources.Enumeradores.TipoOcupacaoMontagemCarregamentoVrp.Pallet, value: this.Pallet }
        ];
    }
}

var EnumTipoOcupacaoMontagemCarregamentoVrp = Object.freeze(new EnumTipoOcupacaoMontagemCarregamentoVrpHelper());