var EnumTipoRotaCargaHelper = function () {

    this.Nenhuma = 0;
    this.Distribuicao = 1;
    this.Transbordo = 2;
    this.Praca = 3;
    this.Retorno = 4;

}

EnumTipoRotaCargaHelper.prototype = {
    obterOpcoes: function () {

        return [
            { text: "Nenhuma", value: this.Nenhuma },
            { text: "Distribuição", value: this.Distribuicao },
            { text: "Transbordo", value: this.Transbordo },
            { text: "Praça", value: this.Praca },
            { text: "Retorno", value: this.Retorno }

        ];
    },
}

var EnumTipoRotaCarga = Object.freeze(new EnumTipoRotaCargaHelper());