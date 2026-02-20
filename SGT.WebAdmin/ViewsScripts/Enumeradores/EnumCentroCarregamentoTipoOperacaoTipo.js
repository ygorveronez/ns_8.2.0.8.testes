var EnumCentroCarregamentoTipoOperacaoTipoHelper = function () {
    this.CapacMaiorVeiculo = 0;
    this.TotalCliente = 1;
}

EnumCentroCarregamentoTipoOperacaoTipoHelper.prototype = {
    obterDescricao: function (valor) {
        switch (valor) {
            case this.CapacMaiorVeiculo: return Localization.Resources.Enumeradores.CentroCarregamentoTipoOperacaoTipo.CapacidadeMaiorVeiculo;
            case this.TotalCliente: return Localization.Resources.Enumeradores.CentroCarregamentoTipoOperacaoTipo.PesoTotalCliente;
            default: return "";
        }
    },
    obterOpcoes: function () {
        return [
            { text: this.obterDescricao(this.CapacMaiorVeiculo), value: this.CapacMaiorVeiculo },
            { text: this.obterDescricao(this.TotalCliente), value: this.TotalCliente }
        ];
    }    
}

var EnumCentroCarregamentoTipoOperacaoTipo = Object.freeze(new EnumCentroCarregamentoTipoOperacaoTipoHelper());