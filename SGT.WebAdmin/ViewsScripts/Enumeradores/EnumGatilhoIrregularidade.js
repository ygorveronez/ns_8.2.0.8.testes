var EnumGatilhoIrregularidadeHelper = function () {
    this.NaoSelecionado = 0;
    this.MIROBloqueioR = 1;
    this.PendenteSubstituicaoDocumento = 2;
    this.SemLink = 3;
    this.SemCalculo = 4;
    this.CTeCancelado = 5;
    this.NFeCancelada = 6;
    this.AliquotaICMSValorICMS = 7;
    this.CNPJTransportadora = 8;
    this.CSTICMS = 9;
    this.NFeVinculadaAoFrete = 10;
    this.TomadorFreteUnilever = 11;
    this.ValorPrestacaoServico = 12;
    this.MunicipioPrestacaoServico = 13;
    this.ValidarDadosNFSe = 14;
    this.ValorTotalReceber = 15;
    this.CFOP = 16;
    this.Participantes = 17;
    this.AliquotaISSValorISS = 18;
};

EnumGatilhoIrregularidadeHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Não selecionado", value: this.NaoSelecionado },
            { text: "1 - MIRO Bloqueio R - Sem tratativas prévia de preço Módulo de Controle", value: this.MIROBloqueioR },
            { text: "2 - Pendente de substituição de documento", value: this.PendenteSubstituicaoDocumento },
            { text: "3 - Sem Link", value: this.SemLink },
            { text: "4 - Sem Cálculo - Falta Pré CT-e", value: this.SemCalculo },
            { text: "5 - CT-e Cancelado", value: this.CTeCancelado },
            { text: "6 - NF-e Cancelada", value: this.NFeCancelada },
            { text: "7 - Alíquota ICMS/Valor ICMS", value: this.AliquotaICMSValorICMS },
            { text: "8 - CNPJ Transportadora", value: this.CNPJTransportadora },
            { text: "9 - CST ICMS", value: this.CSTICMS },
            { text: "10 - NF-e Vinculada ao Frete", value: this.NFeVinculadaAoFrete },
            { text: "11 - Tomador do Frete Unilever", value: this.TomadorFreteUnilever },
            { text: "12 - Valor prestação serviço", value: this.ValorPrestacaoServico },
            { text: "13 - Município Prestação Serviço", value: this.MunicipioPrestacaoServico },
            { text: "14 - Validar dados NFS-e", value: this.ValidarDadosNFSe },
            { text: "15 - Valor Total a Receber", value: this.ValorTotalReceber },
            { text: "16 - CFOP", value: this.CFOP },
            { text: "17 - Remetente / Destinatário / Expedidor e Recebedor", value: this.Participantes },
            { text: "18 - Alíquota ISS/Valor ISS", value: this.AliquotaISSValorISS },
        ];
    },
};

var EnumGatilhoIrregularidade = Object.freeze(new EnumGatilhoIrregularidadeHelper);