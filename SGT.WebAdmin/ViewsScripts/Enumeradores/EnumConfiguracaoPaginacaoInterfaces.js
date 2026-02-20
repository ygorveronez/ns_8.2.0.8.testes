var EnumConfiguracaoPaginacaoInterfacesHelper = function () {
    this.NaoInformada = 0;
    this.Cargas_Carga = 1;
    this.PagamentosMotoristas_PagamentoMotoristaTMS = 2;
};

EnumConfiguracaoPaginacaoInterfacesHelper.prototype.ObterOpcoes = function () {
    return [
        { text: "#Cargas/Carga", value: this.Cargas_Carga },
        { text: "#PagamentosMotoristas/PagamentoMotoristaTMS", value: this.PagamentosMotoristas_PagamentoMotoristaTMS }
    ];
};

EnumConfiguracaoPaginacaoInterfacesHelper.prototype.obterDescricao =  function (valor) {
    switch (valor) {
        case this.Cargas_Carga: return "#Cargas/Carga";
        case this.PagamentosMotoristas_PagamentoMotoristaTMS: return "#PagamentosMotoristas/PagamentoMotoristaTMS";

        default: return "";
    }
   
};

var EnumConfiguracaoPaginacaoInterfaces = Object.freeze(new EnumConfiguracaoPaginacaoInterfacesHelper());