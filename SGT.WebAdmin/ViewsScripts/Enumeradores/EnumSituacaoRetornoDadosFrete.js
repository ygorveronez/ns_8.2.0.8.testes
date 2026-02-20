var EnumSituacaoRetornoDadosFreteHelper = function () {
    this.FreteValido = 1;
    this.ProblemaCalcularFrete = 2;
    this.RotaNaoEncontrada = 3;
    this.CalculandoFrete = 4;
}

EnumSituacaoRetornoDadosFreteHelper.prototype = {
    isPendenciaFrete: function (situacao) {
        return (situacao == this.ProblemaCalcularFrete) || (situacao == this.RotaNaoEncontrada);
    }
}

var EnumSituacaoRetornoDadosFrete = Object.freeze(new EnumSituacaoRetornoDadosFreteHelper());