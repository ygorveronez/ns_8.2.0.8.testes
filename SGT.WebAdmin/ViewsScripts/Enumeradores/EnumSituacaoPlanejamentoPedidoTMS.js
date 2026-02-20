var EnumSituacaoPlanejamentoPedidoTMSHelper = function () {
    this.Todas = "";
    this.Pendente = 0;
    this.CheckListOK = 1;
    this.CargaGerouDocumentacao = 2;
    this.CargaPossuiAcertoAberto = 3;
    this.PassouPelaGuarita = 4;
    this.CargaCanceladaAnulada = 5;
    this.AvisoAoMotorista = 6;
    this.MotoristaCiente = 7;
    this.Devolucao = 8;
};

EnumSituacaoPlanejamentoPedidoTMSHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Carga Cancelada/Anulada", value: this.CargaCanceladaAnulada },
            { text: "Carga Gerou Documentação", value: this.CargaGerouDocumentacao },
            { text: "Carga Possui Acerto Aberto", value: this.CargaPossuiAcertoAberto },
            { text: "Passou pela Guarita", value: this.PassouPelaGuarita },
            { text: "Check List OK", value: this.CheckListOK },
            { text: "Pendente", value: this.Pendente },
            { text: "Aviso ao Motorista", value: this.AvisoAoMotorista },
            { text: "Motorista Ciente", value: this.MotoristaCiente },
            { text: "Devolução", value: this.Devolucao }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    }
};

var EnumSituacaoPlanejamentoPedidoTMS = Object.freeze(new EnumSituacaoPlanejamentoPedidoTMSHelper());