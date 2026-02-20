var EnumTipoIrregularidadeHelper = function () {
    this.Nenhum = 1;
    this.ValorPrestacao = 2;
    this.SemLink = 3;
    this.CTeCancelado = 5;
    this.AliquotaICMSValorICMS = 7;
    this.CNPJTransportadora = 8;
    this.CSTICMS = 9;
    this.NFeVinculadaFrete = 10;
    this.TomadorFreteUnilever = 11;
    this.ValorPrestacaoServico = 12;
    this.MunicipioPrestacaoServico = 13;
    this.ValorTotalReceber = 15;
    this.CFOP = 16;
    this.RemetenteDestinatarioExpedidorRecebedor = 17;
    this.AliquotaISSValorISS = 18
};

EnumTipoIrregularidadeHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Gerais.Geral.NaoSelecionado, value: this.Nenhum },
            { text: "Valor Prestação", value: this.ValorPrestacao },
            { text: "Sem Link", value: this.SemLink },
            { text: "CT-e Cancelado", value: this.CTeCancelado },
            { text: "Aliquota ICMS/Valor ICMS", value: this.AliquotaICMSValorICMS },
            { text: "CNPJ Transportadora", value: this.CNPJTransportadora },
            { text: "CST ICMS", value: this.CSTICMS },
            { text: "NF-e Vinculada ao Frete", value: this.NFeVinculadaFrete },
            { text: "Tomador do Frete Unilever", value: this.TomadorFreteUnilever },
            { text: "Valor Prestação Serviço", value: this.ValorPrestacaoServico },
            { text: "Município Prestção Serviço", value: this.MunicipioPrestacaoServico },
            { text: "Valor Total a Receber", value: this.ValorTotalReceber },
            { text: "CFOP", value: this.CFOP },
            { text: "Remetente/Destinatario/Expedidor e Recebedor", value: this.RemetenteDestinatarioExpedidorRecebedor },
            { text: "Aliquota ISS/ValorISS", value: this.AliquotaISSValorISS }
        ];
    },
};

var EnumTipoIrregularidade = Object.freeze(new EnumTipoIrregularidadeHelper);