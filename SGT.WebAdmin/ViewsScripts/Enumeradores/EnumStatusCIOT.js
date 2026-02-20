var EnumStatusCIOTHelper = function () {
    this.Pendente = 0;
    this.Autorizado = 1;
    this.Encerrado = 2;
    this.Cancelado = 3;
    this.Aberto = 4;
    this.Salvo = 5;
    this.Rejeitado = 9;
    this.Rejeitado_Evento = 10;
}

EnumStatusCIOTHelper.prototype = {
    descricoes: function () {
        return [
            { text: "Pendente", value: this.Pendente },
            { text: "Autorizado", value: this.Autorizado },
            { text: "Encerrado", value: this.Encerrado },
            { text: "Cancelado", value: this.Cancelado },
            { text: "Aberto", value: this.Aberto },
            { text: "Salvo", value: this.Salvo },
            { text: "Rejeitado", value: this.Rejeitado },
            { text: "Evento Rejeitado", value: this.Rejeitado_Evento },
        ];
    },

    obterOpcoes: function (options, opcaoTodos) {
        var _options = options || [];
        var arrayOptions = [];
        var _descricoes = this.descricoes();

        if (_options.length == 0) {
            for (var i in this) {
                if (this.hasOwnProperty(i)) _options.push(this[i]);
            }
        }

        if (opcaoTodos)
            arrayOptions.push({ text: 'Todos', value: '' });

        for (var i in _descricoes) {
            if ($.inArray(_descricoes[i].value, _options) > -1)
                arrayOptions.push(_descricoes[i]);
        }

        return arrayOptions;
    }
}

var EnumStatusCIOT = Object.freeze(new EnumStatusCIOTHelper());