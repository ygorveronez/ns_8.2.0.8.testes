var EnumSituacaoDiarioBordoSemanalHelper = function () {
    this.Todas = 0;
    this.Aberto = 1;
    this.EntregueParcial = 2;
    this.EntregueCompleto = 3;
    this.Cancelado = 4;
};

EnumSituacaoDiarioBordoSemanalHelper.prototype = {
    ObterOpcoes: function () {
        return [
            { text: "Aberto", value: this.Aberto },
            { text: "Entregue Parcialmente", value: this.EntregueParcial },
            { text: "Entregue Completo", value: this.EntregueCompleto },
            { text: "Cancelado", value: this.Cancelado }
        ];
    },
    ObterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.ObterOpcoes());
    }
};

var EnumSituacaoDiarioBordoSemanal = Object.freeze(new EnumSituacaoDiarioBordoSemanalHelper());
