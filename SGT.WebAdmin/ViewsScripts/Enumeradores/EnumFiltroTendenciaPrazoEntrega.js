var EnumFiltroTendenciaPrazoEntregaHelper = function () {
        this.Todos = 99,
        this.Nenhum = 0,
        this.Adiantado = 1,
        this.NoPrazo = 2,
        this.TendenciaEntrega = 3,
        this.Atrasado = 4
};

EnumFiltroTendenciaPrazoEntregaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Todos", value: this.Todos },
            { text: "Nenhum", value: this.Nenhum },
            { text: "Adiantado", value: this.Adiantado },
            { text: "No Prazo", value: this.NoPrazo },
            { text: "Atrasado", value: this.Atrasado }
        ];
    }
}

var EnumFiltroTendenciaPrazoEntrega = Object.freeze(new EnumFiltroTendenciaPrazoEntregaHelper());