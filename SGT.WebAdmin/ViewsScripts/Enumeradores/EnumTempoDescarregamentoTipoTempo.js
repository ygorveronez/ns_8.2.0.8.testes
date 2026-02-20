var EnumTempoDescarregamentoTipoTempoHelper = function () {
    this.Cliente = 0;
    this.Tonelada = 1;
}

EnumTempoDescarregamentoTipoTempoHelper.prototype = {
    obterDescricao: function (tipoTempo) {
        switch (tipoTempo) {
            case this.Cliente: return "Cliente";
            case this.Tonelada: return "Tonelada";
            default: return undefined;
        }
    },
    obterOpcoes: function () {
        return [
            { text: "Cliente", value: this.Cliente },
            { text: "Tonelada", value: this.Tonelada }
        ];
    }
}

var EnumTempoDescarregamentoTipoTempo = Object.freeze(new EnumTempoDescarregamentoTipoTempoHelper());