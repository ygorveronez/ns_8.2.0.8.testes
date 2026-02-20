var EnumTipoTipoSubareaClienteHelper = function () {
    this.Todos = 0;
    this.Portaria = 1;
    this.Patio = 2;
    this.Estacionamento = 3;
    this.Balanca = 4;
    this.Carregamento = 5;
    this.Descarregamento = 6;
};

EnumTipoTipoSubareaClienteHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Balança", value: this.Balanca },
            { text: "Carregamento", value: this.Carregamento },
            { text: "Descarregamento", value: this.Descarregamento },
            { text: "Estacionamento", value: this.Estacionamento },
            { text: "Pátio", value: this.Patio },
            { text: "Portaria", value: this.Portaria },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    },
    obterDescricao: function (tipo) {
        lista = this.obterOpcoes();
        for (var i = 0; i < lista.length; i++) {
            if (lista[i].value == tipo)
                return lista[i].text;
        }
    }
}

var EnumTipoTipoSubareaCliente = Object.freeze(new EnumTipoTipoSubareaClienteHelper());