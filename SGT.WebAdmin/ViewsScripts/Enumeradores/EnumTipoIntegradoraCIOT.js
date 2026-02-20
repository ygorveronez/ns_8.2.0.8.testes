var EnumTipoIntegradoraCIOTHelper = function () {
    this.SigaFacil = 1;
    this.PamCard = 2;
    this.EFrete = 3;
}

EnumTipoIntegradoraCIOTHelper.prototype = {
    descricoes: function () {
        return [
            { text: "Siga Fácil", value: this.SigaFacil },
            { text: "PamCard", value: this.PamCard },
            { text: "E-Frete", value: this.EFrete },
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

var EnumTipoIntegradoraCIOT = Object.freeze(new EnumTipoIntegradoraCIOTHelper());