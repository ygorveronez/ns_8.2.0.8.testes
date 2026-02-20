var EnumVerificarTipoDeOperacaoHelper = function () {
    this.NaoVerificar = 1;
    this.Algum = 2;
    this.Nenhum = 3;
}

EnumVerificarTipoDeOperacaoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Não verificar", value: this.NaoVerificar },
            { text: "Algum dos tipos", value: this.Algum },
            { text: "Nenhum dos tipos", value: this.Nenhum }
        ];
    }
}

var EnumVerificarTipoDeOperacao = Object.freeze(new EnumVerificarTipoDeOperacaoHelper());