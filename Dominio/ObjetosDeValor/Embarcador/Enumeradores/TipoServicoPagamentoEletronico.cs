namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoServicoPagamentoEletronico
    {
        Padrao = 0,
        Cobranca = 1,
        BloquetoEletronico = 3,
        ConciliacaoBancaria = 4,
        Debitos = 5,
        CustodiadeCheques = 6,
        GestaodeCaixa = 7,
        ConsultaInformacaoMargem = 8,
        AverbacaodaConsignacaoRetencao = 9,
        PagamentoDividendos = 10,
        ManutencaodaConsignacao = 11,
        ConsignacaodeParcelas = 12,
        GlosadaConsignacao = 13,
        ConsultadeTributosapagar = 14,
        PagamentoFornecedor = 20,
        PagamentodeContasTributoseImpostos = 22,
        Compror = 25,
        ComprorRotativo = 26,
        AlegacaodoSacado = 29,
        PagamentoSalarios = 30,
        Pagamentodehonorarios = 32,
        Pagamentodebolsaauxilio = 33,
        Pagamentodeprebenda = 34,
        Vendor = 40,
        VendoraTermo = 21,
        PagamentoSinistrosSegurados = 50,
        PagamentoDespesasViajanteemTransito = 60,
        PagamentoAutorizado = 70,
        PagamentoCredenciados = 75,
        PagamentodeRemuneracao = 77,
        PagamentoRepresentantesVendedoresAutorizados = 80,
        PagamentoBeneficios = 90,
        PagamentosDiversos = 98,
        ExclusivoBradesco = 99
    }

    public static class TipoServicoPagamentoEletronicoHelper
    {
        public static string ObterDescricao(this TipoServicoPagamentoEletronico finalidadePagamento)
        {
            switch (finalidadePagamento)
            {
                case TipoServicoPagamentoEletronico.Padrao: return "Padrão";
                case TipoServicoPagamentoEletronico.Cobranca: return "01 = Cobrança";
                case TipoServicoPagamentoEletronico.BloquetoEletronico:
                    return "03 = Bloqueto Eletrônico";
                case TipoServicoPagamentoEletronico.ConciliacaoBancaria:
                    return "04 = Conciliação Bancária";
                case TipoServicoPagamentoEletronico.Debitos:
                    return "05 = Débitos";
                case TipoServicoPagamentoEletronico.CustodiadeCheques:
                    return "06 = Custódia de Cheques";
                case TipoServicoPagamentoEletronico.GestaodeCaixa:
                    return "07 = Gestão de Caixa";
                case TipoServicoPagamentoEletronico.ConsultaInformacaoMargem:
                    return "08 = Consulta / Informação Margem";
                case TipoServicoPagamentoEletronico.AverbacaodaConsignacaoRetencao:
                    return "09 = Averbação da Consignação/ Retenção";
                case TipoServicoPagamentoEletronico.PagamentoDividendos:
                    return "10 = Pagamento Dividendos";
                case TipoServicoPagamentoEletronico.ManutencaodaConsignacao:
                    return "11 = Manutenção da Consignação";
                case TipoServicoPagamentoEletronico.ConsignacaodeParcelas:
                    return "12 = Consignação de Parcelas";
                case TipoServicoPagamentoEletronico.GlosadaConsignacao:
                    return "13 = Glosa da Consignação(INSS)";
                case TipoServicoPagamentoEletronico.ConsultadeTributosapagar:
                    return "14 = Consulta de Tributos a pagar";
                case TipoServicoPagamentoEletronico.PagamentoFornecedor:
                    return "20 = Pagamento Fornecedor";
                case TipoServicoPagamentoEletronico.PagamentodeContasTributoseImpostos:
                    return "22 = Pagamento de Contas, Tributos e Impostos";
                case TipoServicoPagamentoEletronico.Compror:
                    return "25 = Compror";
                case TipoServicoPagamentoEletronico.ComprorRotativo:
                    return "26 = Compror Rotativo";
                case TipoServicoPagamentoEletronico.AlegacaodoSacado:
                    return "29 = Alegação do Sacado";
                case TipoServicoPagamentoEletronico.PagamentoSalarios:
                    return "30 = Pagamento Salários";
                case TipoServicoPagamentoEletronico.Pagamentodehonorarios:
                    return "32 = Pagamento de honorários";
                case TipoServicoPagamentoEletronico.Pagamentodebolsaauxilio:
                    return "33 = Pagamento de bolsa auxílio";
                case TipoServicoPagamentoEletronico.Pagamentodeprebenda:
                    return "34 = Pagamento de prebenda(remuneração a padres e sacerdotes)";
                case TipoServicoPagamentoEletronico.Vendor:
                    return "40 = Vendor";
                case TipoServicoPagamentoEletronico.VendoraTermo:
                    return "41 = Vendor a Termo";
                case TipoServicoPagamentoEletronico.PagamentoSinistrosSegurados:
                    return "50 = Pagamento Sinistros Segurados";
                case TipoServicoPagamentoEletronico.PagamentoDespesasViajanteemTransito:
                    return "60 = Pagamento Despesas Viajante em Trânsito";
                case TipoServicoPagamentoEletronico.PagamentoAutorizado:
                    return "70 = Pagamento Autorizado";
                case TipoServicoPagamentoEletronico.PagamentoCredenciados:
                    return "75 = Pagamento Credenciados";
                case TipoServicoPagamentoEletronico.PagamentodeRemuneracao:
                    return "77 = Pagamento de Remuneração";
                case TipoServicoPagamentoEletronico.PagamentoRepresentantesVendedoresAutorizados:
                    return "80 = Pagamento Representantes / Vendedores Autorizados";
                case TipoServicoPagamentoEletronico.PagamentoBeneficios:
                    return "90 = Pagamento Benefícios";
                case TipoServicoPagamentoEletronico.PagamentosDiversos:
                    return "98 = Pagamentos Diversos";
                case TipoServicoPagamentoEletronico.ExclusivoBradesco: return "99 = Exclusivo Bradesco";
                default: return string.Empty;
            }
        }

        public static string ObterNumero(this TipoServicoPagamentoEletronico finalidadePagamento)
        {
            switch (finalidadePagamento)
            {
                case TipoServicoPagamentoEletronico.Padrao: return "00";
                case TipoServicoPagamentoEletronico.Cobranca: return "01";
                case TipoServicoPagamentoEletronico.BloquetoEletronico:
                    return "03";
                case TipoServicoPagamentoEletronico.ConciliacaoBancaria:
                    return "04";
                case TipoServicoPagamentoEletronico.Debitos:
                    return "05";
                case TipoServicoPagamentoEletronico.CustodiadeCheques:
                    return "06";
                case TipoServicoPagamentoEletronico.GestaodeCaixa:
                    return "07";
                case TipoServicoPagamentoEletronico.ConsultaInformacaoMargem:
                    return "08";
                case TipoServicoPagamentoEletronico.AverbacaodaConsignacaoRetencao:
                    return "09";
                case TipoServicoPagamentoEletronico.PagamentoDividendos:
                    return "10";
                case TipoServicoPagamentoEletronico.ManutencaodaConsignacao:
                    return "11";
                case TipoServicoPagamentoEletronico.ConsignacaodeParcelas:
                    return "12";
                case TipoServicoPagamentoEletronico.GlosadaConsignacao:
                    return "13";
                case TipoServicoPagamentoEletronico.ConsultadeTributosapagar:
                    return "14";
                case TipoServicoPagamentoEletronico.PagamentoFornecedor:
                    return "20";
                case TipoServicoPagamentoEletronico.PagamentodeContasTributoseImpostos:
                    return "22";
                case TipoServicoPagamentoEletronico.Compror:
                    return "25";
                case TipoServicoPagamentoEletronico.ComprorRotativo:
                    return "26";
                case TipoServicoPagamentoEletronico.AlegacaodoSacado:
                    return "29";
                case TipoServicoPagamentoEletronico.PagamentoSalarios:
                    return "30";
                case TipoServicoPagamentoEletronico.Pagamentodehonorarios:
                    return "32";
                case TipoServicoPagamentoEletronico.Pagamentodebolsaauxilio:
                    return "33";
                case TipoServicoPagamentoEletronico.Pagamentodeprebenda:
                    return "34";
                case TipoServicoPagamentoEletronico.Vendor:
                    return "40";
                case TipoServicoPagamentoEletronico.VendoraTermo:
                    return "41";
                case TipoServicoPagamentoEletronico.PagamentoSinistrosSegurados:
                    return "50";
                case TipoServicoPagamentoEletronico.PagamentoDespesasViajanteemTransito:
                    return "60";
                case TipoServicoPagamentoEletronico.PagamentoAutorizado:
                    return "70";
                case TipoServicoPagamentoEletronico.PagamentoCredenciados:
                    return "75";
                case TipoServicoPagamentoEletronico.PagamentodeRemuneracao:
                    return "77";
                case TipoServicoPagamentoEletronico.PagamentoRepresentantesVendedoresAutorizados:
                    return "80";
                case TipoServicoPagamentoEletronico.PagamentoBeneficios:
                    return "90";
                case TipoServicoPagamentoEletronico.PagamentosDiversos:
                    return "98";
                case TipoServicoPagamentoEletronico.ExclusivoBradesco: return "99";
                default: return string.Empty;
            }
        }
    }
}
