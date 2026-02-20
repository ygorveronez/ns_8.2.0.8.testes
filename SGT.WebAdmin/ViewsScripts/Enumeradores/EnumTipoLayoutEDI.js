var EnumTipoLayoutEDIHelper = function () {
    this.CONEMB = 0;
    this.DOCCOB = 1;
    this.NOTFIS = 2;
    this.OCOREN = 3;
    this.SEGURO = 4;
    this.EBS = 5;
    this.OUTRO = 6;
    this.INTDNE = 9;
    this.CONEMB_MB = 10;
    this.DESPESACOMPLEMENTAR = 11;
    this.NOTFIS_NOVA_IMPORTACAO = 12;
    this.INTNC = 13;
    this.EBSProduto = 14;
    this.EBSNotaEntrada = 15;
    this.EAI = 16;
    this.INTPFAR = 17;
    this.OCOREN_NFS = 18;
    this.PREFAT = 19;
    this.CONEMB_CANCELAMENTO = 20;
    this.EBSBaixas = 21;
    this.EBSComissaoMotorista = 22;
    this.PROV = 23;
    this.INTNC_CANCELAMENTO = 24;
    this.GEN = 25;
    this.PROV_INTPFAR = 26;
    this.QuestorComissaoMotorista = 27;
    this.MasterSAF = 28;
    this.MasterSAFCancelamento = 29;
    this.RPSNotaServico = 30;
    this.RetornoRPSNotaServico = 31;
    this.IntegracaoCarregamento = 32;
    this.OcorenOTIF = 33;
    this.TransportationPlann = 34;
    this.VGM = 35;
    this.CONEMB_NF = 36;
    this.AGRO = 37;
    this.CONEMB_CT_IMP = 38;
    this.CONEMB_CT_EXP = 39;
    this.DOCCOB_CT = 40;
    this.Cliente = 41;
    this.Pedido = 42;
    this.ImportsysCTe = 43;
    this.ImportsysVP = 44;
    this.UVT_RN = 45;
    this.DOCCOB_VAXXINOVA = 46;
    this.CONEMB_VOLKS = 47;

    this.Todos = 99;
};

EnumTipoLayoutEDIHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "CONEMB", value: this.CONEMB },
            { text: "CONEMB MB", value: this.CONEMB_MB },
            { text: "CONEMB NF", value: this.CONEMB_NF },
            { text: "CONEMB Cancelamento", value: this.CONEMB_CANCELAMENTO },
            { text: "DOCCOB", value: this.DOCCOB },
            { text: "NOTFIS", value: this.NOTFIS },
            { text: "NOTFIS - Nova Importação", value: this.NOTFIS_NOVA_IMPORTACAO },
            { text: "OCOREN", value: this.OCOREN },
            { text: "OCOREN NFS", value: this.OCOREN_NFS },
            { text: "PREFAT", value: this.PREFAT },
            { text: "SEGURO", value: this.SEGURO },
            { text: "DESPESA COMPLEMENTAR", value: this.DESPESACOMPLEMENTAR },
            { text: "EBS", value: this.EBS },
            { text: "EBS Produto", value: this.EBSProduto },
            { text: "EBS Nota Entrada", value: this.EBSNotaEntrada },
            { text: "EBS Baixas", value: this.EBSBaixas },
            { text: "EBS Comissão Motorista", value: this.EBSComissaoMotorista },
            { text: "INTDNE", value: this.INTDNE },
            { text: "INTPFAR", value: this.INTPFAR },
            { text: "PROV INTPFAR", value: this.PROV_INTPFAR },
            { text: "INTNC", value: this.INTNC },
            { text: "INTNC Cancelamento", value: this.INTNC_CANCELAMENTO },
            { text: "EAI", value: this.EAI },
            { text: "PROV", value: this.PROV },
            { text: "GEN", value: this.GEN },
            { text: "OUTRO", value: this.OUTRO },
            { text: "MasterSAF", value: this.MasterSAF },
            { text: "MasterSAF Cancelamento", value: this.MasterSAFCancelamento },
            { text: "RPS de Nota de Serviço", value: this.RPSNotaServico },
            { text: "Retorno RPS de Nota de Serviço", value: this.RetornoRPSNotaServico },
            { text: "Questor Comissão Motorista", value: this.QuestorComissaoMotorista },
            { text: "Integração Carregamento", value: this.IntegracaoCarregamento },
            { text: "OCOREN OTIF", value: this.OcorenOTIF },
            { text: "Transportation Plann", value: this.TransportationPlann },
            { text: "VGM", value: this.VGM },
            { text: "AGRO", value: this.AGRO },
            { text: "CONEMB CT IMP", value: this.CONEMB_CT_IMP },
            { text: "CONEMB CT EXP", value: this.CONEMB_CT_EXP },
            { text: "DOCCOB CT", value: this.DOCCOB_CT },
            { text: "Cliente", value: this.Cliente },
            { text: "Pedido", value: this.Pedido },
            { text: "ImportsysCTe", value: this.ImportsysCTe },
            { text: "ImportsysVP", value: this.ImportsysVP },
            { text: "UVT RN", value: this.UVT_RN },
            { text: "DOCCOB VAXXINOVA", value: this.DOCCOB_VAXXINOVA },
            { text: "CONEMB VOLKS", value: this.CONEMB_VOLKS },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoLayoutEDI = Object.freeze(new EnumTipoLayoutEDIHelper());