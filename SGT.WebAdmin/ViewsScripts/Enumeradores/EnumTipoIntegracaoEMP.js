var EnumTipoIntegracaoEMPHelper = function () {
    this.CTe = 0;
    this.Carga = 1;
    this.CTeFatura = 2;
    this.CartaCorrecao = 3;
    this.Ocorrencia = 4;
    this.CTeManual = 5;
    this.NaoInformado = 6;
    this.Vessel = 7;
    this.Customer = 8;
    this.Booking = 9;
    this.Schedule = 12;
    this.CEMercante = 13;
    this.Fatura = 14,
    this.Boleto = 15,
    this.FaturaNFTP = 16,
    this.CTeNFTP = 17,
    this.NdNFTP = 18,
    this.CTeComplementarNFTP = 19,
    this.OcorrenciaNFTP = 20
};

EnumTipoIntegracaoEMPHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Todos", value: this.NaoInformado },
            { text: "CT-e", value: this.CTe },
            { text: "Carga", value: this.Carga },
            { text: "CT-e Fatura", value: this.CTeFatura },
            { text: "Carta de Correção", value: this.CartaCorrecao },
            { text: "Ocorrência", value: this.Ocorrencia },
            { text: "CT-e Manual", value: this.CTeManual },
            { text: "Vessel", value: this.Vessel },
            { text: "Customer", value: this.Customer },
            { text: "Booking", value: this.Booking },
            { text: "Schedule", value: this.Schedule },
            { text: "CE Mercante", value: this.CEMercante },
            { text: "Fatura", value: this.Fatura },
            { text: "Boleto", value: this.Boleto },
            { text: "Fatura NFTP", value: this.FaturaNFTP },
            { text: "CTe NFTP", value: this.CTeNFTP },
            { text: "ND NFTP", value: this.NdNFTP },
            { text: "CTe Complementar NFTP", value: this.CTeComplementarNFTP },
            { text: "Ocorrencia NFTP", value: this.OcorrenciaNFTP }
        ];
    },
};

var EnumTipoIntegracaoEMP = Object.freeze(new EnumTipoIntegracaoEMPHelper());