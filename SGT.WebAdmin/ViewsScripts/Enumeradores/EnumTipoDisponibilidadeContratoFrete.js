var EnumTipoDisponibilidadeContratoFreteHelper = function () {
    this.TracaoComOuSemReboque = 0;
    this.TodosVeiculos = 1;
    this.Tracao = 2;
    this.Reboque = 3;
    this.TracaoComCarroceria = 4;
};

EnumTipoDisponibilidadeContratoFreteHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Tração com/sem Reboque", value: this.TracaoComOuSemReboque },
            { text: "Todos Veículos", value: this.TodosVeiculos },
            { text: "Tração", value: this.Tracao },
            { text: "Reboque", value: this.Reboque },
            { text: "Traçao com Carroceria", value: this.TracaoComCarroceria }
        ];
    },
}

var EnumTipoDisponibilidadeContratoFrete = Object.freeze(new EnumTipoDisponibilidadeContratoFreteHelper());