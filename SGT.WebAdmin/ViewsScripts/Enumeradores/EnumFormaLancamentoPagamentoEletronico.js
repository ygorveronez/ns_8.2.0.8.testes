var EnumFormaLancamentoPagamentoEletronicoHelper = function () {
    this.Padrao = 0;
    this.CreditoemContaCorrente = 1;
    this.ChequePagamentoAdministrativo = 2;
    this.DOCTED = 3;
    this.CartaoSalario = 4;
    this.CreditoemContaPoupança = 5;
    this.OPDisposicao = 10;
    this.PagamentodeContaseTributoscomCodigodeBarras = 11;
    this.TributoDARFNormal = 16;
    this.TributoGPS = 17;
    this.TributoDARFSimples = 18;
    this.TributoIPTU = 19;
    this.PagamentocomAutenticacao = 20;
    this.TributoDARJ = 21;
    this.TributoGARESPICMS = 22;
    this.TributoGARESPDR = 23;
    this.TributoGARESPITCMD = 24;
    this.TributoIPVA = 25;
    this.TributoLicenciamento = 26;
    this.TributoDPVAT = 27;
    this.LiquidacaodeTitulosdoProprioBanco = 30;
    this.PagamentodeTitulosdeOutrosBancos = 31;
    this.ExtratodeContaCorrente = 40;
    this.TEDOutraTitularidade = 41;
    this.TEDMesmaTitularidade = 43;
    this.TEDparaTransferenciadeContaInvestimento = 44;
    this.PIXTransferencia = 45;
    this.PIXQRCode = 47;
    this.DebitoemContaCorrente = 50;
    this.ExtratoparaGestaodeCaixa = 70;
    this.DepositoJudicialemContaCorrente = 71;
    this.DepositoJudicialemPoupança = 72;
    this.ExtratodeContaInvestimento = 73;
    this.Cadastrodefavorecidos = 99
};

EnumFormaLancamentoPagamentoEletronicoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Padrao", value: this.Padrao },
            { text: "01 = Crédito em Conta Corrente", value: this.CreditoemContaCorrente },
            { text: "02 = Cheque Pagamento / Administrativo", value: this.ChequePagamentoAdministrativo },
            { text: "03 = DOC / TED(1)(2)", value: this.DOCTED },
            { text: "04 = Cartão Salário(somente para Tipo de Serviço = 30)", value: this.CartaoSalario },
            { text: "05 = Crédito em Conta Poupança", value: this.CreditoemContaPoupança },
            { text: "10 = OP à Disposição", value: this.OPDisposicao },
            { text: "11 = Pagamento de Contas e Tributos com Código de Barras", value: this.PagamentodeContaseTributoscomCodigodeBarras },
            { text: "16 = Tributo - DARF Normal", value: this.TributoDARFNormal },
            { text: "17 = Tributo - GPS(Guia da Previdência Social)", value: this.TributoGPS },
            { text: "18 = Tributo - DARF Simples", value: this.TributoDARFSimples },
            { text: "19 = Tributo - IPTU – Prefeituras", value: this.TributoIPTU },
            { text: "20 = Pagamento com Autenticação", value: this.PagamentocomAutenticacao },
            { text: "21 = Tributo – DARJ", value: this.TributoDARJ },
            { text: "22 = Tributo - GARE - SP ICMS", value: this.TributoGARESPICMS },
            { text: "23 = Tributo - GARE - SP DR", value: this.TributoGARESPDR },
            { text: "24 = Tributo - GARE - SP ITCMD", value: this.TributoGARESPITCMD },
            { text: "25 = Tributo - IPVA", value: this.TributoIPVA },
            { text: "26 = Tributo - Licenciamento", value: this.TributoLicenciamento },
            { text: "27 = Tributo – DPVAT", value: this.TributoDPVAT },
            { text: "30 = Liquidação de Títulos do Próprio Banco", value: this.LiquidacaodeTitulosdoProprioBanco },
            { text: "31 = Pagamento de Títulos de Outros Bancos", value: this.PagamentodeTitulosdeOutrosBancos },
            { text: "40 = Extrato de Conta Corrente", value: this.ExtratodeContaCorrente },
            { text: "41 = TED – Outra Titularidade(1)", value: this.TEDOutraTitularidade },
            { text: "43 = TED – Mesma Titularidade(1)", value: this.TEDMesmaTitularidade },
            { text: "44 = TED para Transferência de Conta Investimento", value: this.TEDparaTransferenciadeContaInvestimento },
            { text: "45 = Pix Transferência", value: this.PIXTransferencia },
            { text: "47 = Pix QR-CODE", value: this.PIXQRCode },
            { text: "50 = Débito em Conta Corrente", value: this.DebitoemContaCorrente },
            { text: "70 = Extrato para Gestão de Caixa", value: this.ExtratoparaGestaodeCaixa },
            { text: "71 = Depósito Judicial em Conta Corrente", value: this.DepositoJudicialemContaCorrente },
            { text: "72 = Depósito Judicial em Poupança", value: this.DepositoJudicialemPoupança },
            { text: "73 = Extrato de Conta Investimento", value: this.ExtratodeContaInvestimento },
            { text: "99 = Cadastro de favorecidos(4)", value: this.Cadastrodefavorecidos }

        ];
    }
};

var EnumFormaLancamentoPagamentoEletronico = Object.freeze(new EnumFormaLancamentoPagamentoEletronicoHelper());