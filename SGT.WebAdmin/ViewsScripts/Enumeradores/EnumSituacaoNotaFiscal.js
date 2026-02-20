var EnumSituacaoNotaFiscalHelper = function () {
    this.Nenhum = 0;
    this.Entregue = 1;
    this.EntregueParcial = 2; /*Não Utilizar*/
    this.Devolvida = 3;
    this.DevolvidaParcial = 4;
    this.AgReentrega = 5;
    this.AgEntrega = 6;
    this.NaoEntregue= 7;
};

EnumSituacaoNotaFiscalHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Nenhum", value: this.Nenhum },
            { text: "Entregue", value: this.Entregue },
            { text: "Entregue Parcial", value: this.EntregueParcial },
            { text: "Devolvida", value: this.Devolvida },
            { text: "Devolvida Parcialmente", value: this.DevolvidaParcial },
            { text: "Aguardando Reentrega", value: this.AgReentrega },
            { text: "Aguardando Entrega", value: this.AgEntrega },
            { text: "Não Entregue", value: this.NaoEntregue }

        ];
    },
    obterOpcoesTelaCanhoto: function () {
        return [
            { text: "Aguardando Entrega", value: this.AgEntrega },
            { text: "Entregue", value: this.Entregue },
            { text: "Devolvida", value: this.Devolvida },
            { text: "Devolvida Parcialmente", value: this.DevolvidaParcial },
            { text: "Aguardando Reentrega", value: this.AgReentrega },
            { text: "Não Entregue", value: this.NaoEntregue }

        ];
    },
}

var EnumSituacaoNotaFiscal = Object.freeze(new EnumSituacaoNotaFiscalHelper());