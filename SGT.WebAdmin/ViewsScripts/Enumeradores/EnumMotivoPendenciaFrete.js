var EnumMotivoPendenciaFreteHelper = function () {
    this.NenhumPendencia = 0;
    this.ProblemaCalculoFrete = 1;
    this.AgOperador = 2;
    this.DivergenciaPreCalculoFrete = 3;
};

EnumMotivoPendenciaFreteHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Nenhuma Pendência", value: this.NenhumPendencia },
            { text: "Problema no Cálculo do Frete", value: this.ProblemaCalculoFrete },
            { text: "Ag. Operador", value: this.AgOperador },
            { text: "Divergência Pré-Cálculo Frete", value: this.DivergenciaPreCalculoFrete },
        ];
    }
};

var EnumMotivoPendenciaFrete = Object.freeze(new EnumMotivoPendenciaFreteHelper());