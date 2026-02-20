var EnumTipoContainerPneuHelper = function () {
    this.Veiculo = 1;
    this.Estepe = 2;
    this.Estoque = 3;
    this.Reforma = 4;
    this.EnvioEstoque = 5;
    this.EnvioSucata = 6;
    this.EnvioReforma = 7;
}

EnumTipoContainerPneuHelper.prototype = {
    IsMovimentacaoParaVeiculo: function (tipoContainerPneu) {
        return (
            (tipoContainerPneu === this.Veiculo) ||
            (tipoContainerPneu === this.Estepe)
        );
    }
}

var EnumTipoContainerPneu = Object.freeze(new EnumTipoContainerPneuHelper());