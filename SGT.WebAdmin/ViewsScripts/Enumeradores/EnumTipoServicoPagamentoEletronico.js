var EnumTipoServicoPagamentoEletronicoHelper = function () {
    this.Padrao = 0;
    this.Cobranca = 1;
    this.BloquetoEletronico = 3;
    this.ConciliacaoBancaria = 4;
    this.Debitos = 5;
    this.CustodiadeCheques = 6;
    this.GestaodeCaixa = 7;
    this.ConsultaInformacaoMargem = 8;
    this.AverbacaodaConsignacaoRetencao = 9;
    this.PagamentoDividendos = 10;
    this.ManutencaodaConsignacao = 11;
    this.ConsignacaodeParcelas = 12;
    this.GlosadaConsignacao = 13;
    this.ConsultadeTributosapagar = 14;
    this.PagamentoFornecedor = 20;
    this.PagamentodeContasTributoseImpostos = 22;
    this.Compror = 25;
    this.ComprorRotativo = 26;
    this.AlegacaodoSacado = 29;
    this.PagamentoSalarios = 30;
    this.Pagamentodehonorarios = 32;
    this.Pagamentodebolsaauxilio = 33;
    this.Pagamentodeprebenda = 34;
    this.Vendor = 40;
    this.VendoraTermo = 21;
    this.PagamentoSinistrosSegurados = 50;
    this.PagamentoDespesasViajanteemTransito = 60;
    this.PagamentoAutorizado = 70;
    this.PagamentoCredenciados = 75;
    this.PagamentodeRemuneracao = 77;
    this.PagamentoRepresentantesVendedoresAutorizados = 80;
    this.PagamentoBeneficios = 90;
    this.PagamentosDiversos = 98;
    this.ExclusivoBradesco = 99;
};

EnumTipoServicoPagamentoEletronicoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Padrão", value: this.Padrao },
            { text: "01 = Cobrança", value: this.Cobranca },
            { text: "03 = Bloqueto Eletrônico", value: this.BloquetoEletronico },
            { text: "04 = Conciliação Bancária", value: this.ConciliacaoBancaria },
            { text: "05 = Débitos", value: this.Debitos },
            { text: "06 = Custódia de Cheques", value: this.CustodiadeCheques },
            { text: "07 = Gestão de Caixa", value: this.GestaodeCaixa },
            { text: "08 = Consulta / Informação Margem", value: this.ConsultaInformacaoMargem },
            { text: "09 = Averbação da Consignação/ Retenção", value: this.AverbacaodaConsignacaoRetencao },
            { text: "10 = Pagamento Dividendos", value: this.PagamentoDividendos },
            { text: "11 = Manutenção da Consignação", value: this.ManutencaodaConsignacao },
            { text: "12 = Consignação de Parcelas", value: this.ConsignacaodeParcelas },
            { text: "13 = Glosa da Consignação(INSS)", value: this.GlosadaConsignacao },
            { text: "14 = Consulta de Tributos a pagar", value: this.ConsultadeTributosapagar },
            { text: "20 = Pagamento Fornecedor", value: this.PagamentoFornecedor },
            { text: "22 = Pagamento de Contas, Tributos e Impostos", value: this.PagamentodeContasTributoseImpostos },
            { text: "25 = Compror", value: this.Compror },
            { text: "26 = Compror Rotativo", value: this.ComprorRotativo },
            { text: "29 = Alegação do Sacado", value: this.AlegacaodoSacado },
            { text: "30 = Pagamento Salários", value: this.PagamentoSalarios },
            { text: "32 = Pagamento de honorários", value: this.Pagamentodehonorarios },
            { text: "33 = Pagamento de bolsa auxílio", value: this.Pagamentodebolsaauxilio },
            { text: "34 = Pagamento de prebenda(remuneração a padres e sacerdotes)", value: this.Pagamentodeprebenda },
            { text: "40 = Vendor", value: this.Vendor },
            { text: "41 = Vendor a Termo", value: this.VendoraTermo },
            { text: "50 = Pagamento Sinistros Segurados", value: this.PagamentoSinistrosSegurados },
            { text: "60 = Pagamento Despesas Viajante em Trânsito", value: this.PagamentoDespesasViajanteemTransito },
            { text: "70 = Pagamento Autorizado", value: this.PagamentoAutorizado },
            { text: "75 = Pagamento Credenciados", value: this.PagamentoCredenciados },
            { text: "77 = Pagamento de Remuneração", value: this.PagamentodeRemuneracao },
            { text: "80 = Pagamento Representantes / Vendedores Autorizados", value: this.PagamentoRepresentantesVendedoresAutorizados },
            { text: "90 = Pagamento Benefícios", value: this.PagamentoBeneficios },
            { text: "98 = Pagamentos Diversos", value: this.PagamentosDiversos },
            { text: "99 = Exclusivo Bradesco", value: this.ExclusivoBradesco }
        ];
    }
};

var EnumTipoServicoPagamentoEletronico = Object.freeze(new EnumTipoServicoPagamentoEletronicoHelper());