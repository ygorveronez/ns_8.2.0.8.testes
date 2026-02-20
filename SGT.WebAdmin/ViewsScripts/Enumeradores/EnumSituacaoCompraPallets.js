var EnumSituacaoCompraPalletsHelper = function () {
    this.Todas = "";
    this.AgFinalizacao = 1;
    this.Finalizado = 2;
    this.Cancelado = 3;
}

EnumSituacaoCompraPalletsHelper.prototype = {
    obterDescricao: function (situacao) {
        switch (situacao) {
            case this.AgFinalizacao: return "Ag. Finalização";
            case this.Finalizado: return "Finalizado";
            case this.Cancelado: return "Cancelado";
            default: return "Todas";
        }
    },
    obterOpcoes: function () {
        return [
            { text: "Todas", value: this.Todas },
            { text: "Ag. Finalização", value: this.AgFinalizacao },
            { text: "Finalizado", value: this.Finalizado },
            { text: "Cancelado", value: this.Cancelado }
        ];
    }
}

var EnumSituacaoCompraPallets = Object.freeze(new EnumSituacaoCompraPalletsHelper());