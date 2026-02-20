namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum FormaLancamentoPagamentoEletronico
    {
        Padrao = 0,
        CreditoemContaCorrente = 1,
        ChequePagamentoAdministrativo = 2,
        DOCTED = 3,
        CartaoSalario = 4,
        CreditoemContaPoupança = 5,
        OPDisposicao = 10,
        PagamentodeContaseTributoscomCodigodeBarras = 11,
        TributoDARFNormal = 16,
        TributoGPS = 17,
        TributoDARFSimples = 18,
        TributoIPTU = 19,
        PagamentocomAutenticacao = 20,
        TributoDARJ = 21,
        TributoGARESPICMS = 22,
        TributoGARESPDR = 23,
        TributoGARESPITCMD = 24,
        TributoIPVA = 25,
        TributoLicenciamento = 26,
        TributoDPVAT = 27,
        LiquidacaodeTitulosdoProprioBanco = 30,
        PagamentodeTitulosdeOutrosBancos = 31,
        ExtratodeContaCorrente = 40,
        TEDOutraTitularidade = 41,
        TEDMesmaTitularidade = 43,
        TEDparaTransferenciadeContaInvestimento = 44,
        PIXTransferencia = 45,
        PIXQRCode = 47,
        DebitoemContaCorrente = 50,
        ExtratoparaGestaodeCaixa = 70,
        DepositoJudicialemContaCorrente = 71,
        DepositoJudicialemPoupança = 72,
        ExtratodeContaInvestimento = 73,
        Cadastrodefavorecidos = 99
    }

    public static class FormaLancamentoPagamentoEletronicoHelper
    {
        public static string ObterDescricao(this FormaLancamentoPagamentoEletronico finalidadePagamento)
        {
            switch (finalidadePagamento)
            {
                case FormaLancamentoPagamentoEletronico.Padrao: return "Padrao";
                case FormaLancamentoPagamentoEletronico.CreditoemContaCorrente:
                    return "01 = Crédito em Conta Corrente";
                case FormaLancamentoPagamentoEletronico.ChequePagamentoAdministrativo:
                    return "02 = Cheque Pagamento / Administrativo";
                case FormaLancamentoPagamentoEletronico.DOCTED:
                    return "03 = DOC / TED(1)(2)";
                case FormaLancamentoPagamentoEletronico.CartaoSalario:
                    return "04 = Cartão Salário(somente para Tipo de Serviço = 30)";
                case FormaLancamentoPagamentoEletronico.CreditoemContaPoupança:
                    return "05 = Crédito em Conta Poupança";
                case FormaLancamentoPagamentoEletronico.OPDisposicao:
                    return "10 = OP à Disposição";
                case FormaLancamentoPagamentoEletronico.PagamentodeContaseTributoscomCodigodeBarras:
                    return "11 = Pagamento de Contas e Tributos com Código de Barras";
                case FormaLancamentoPagamentoEletronico.TributoDARFNormal:
                    return "16 = Tributo - DARF Normal";
                case FormaLancamentoPagamentoEletronico.TributoGPS:
                    return "17 = Tributo - GPS(Guia da Previdência Social)";
                case FormaLancamentoPagamentoEletronico.TributoDARFSimples:
                    return "18 = Tributo - DARF Simples";
                case FormaLancamentoPagamentoEletronico.TributoIPTU:
                    return "19 = Tributo - IPTU – Prefeituras";
                case FormaLancamentoPagamentoEletronico.PagamentocomAutenticacao:
                    return "20 = Pagamento com Autenticação";
                case FormaLancamentoPagamentoEletronico.TributoDARJ:
                    return "21 = Tributo – DARJ";
                case FormaLancamentoPagamentoEletronico.TributoGARESPICMS:
                    return "22 = Tributo - GARE - SP ICMS";
                case FormaLancamentoPagamentoEletronico.TributoGARESPDR:
                    return "23 = Tributo - GARE - SP DR";
                case FormaLancamentoPagamentoEletronico.TributoGARESPITCMD:
                    return "24 = Tributo - GARE - SP ITCMD";
                case FormaLancamentoPagamentoEletronico.TributoIPVA:
                    return "25 = Tributo - IPVA";
                case FormaLancamentoPagamentoEletronico.TributoLicenciamento:
                    return "26 = Tributo - Licenciamento";
                case FormaLancamentoPagamentoEletronico.TributoDPVAT:
                    return "27 = Tributo – DPVAT";
                case FormaLancamentoPagamentoEletronico.LiquidacaodeTitulosdoProprioBanco:
                    return "30 = Liquidação de Títulos do Próprio Banco";
                case FormaLancamentoPagamentoEletronico.PagamentodeTitulosdeOutrosBancos:
                    return "31 = Pagamento de Títulos de Outros Bancos";
                case FormaLancamentoPagamentoEletronico.ExtratodeContaCorrente:
                    return "40 = Extrato de Conta Corrente";
                case FormaLancamentoPagamentoEletronico.TEDOutraTitularidade:
                    return "41 = TED – Outra Titularidade(1)";
                case FormaLancamentoPagamentoEletronico.TEDMesmaTitularidade:
                    return "43 = TED – Mesma Titularidade(1)";
                case FormaLancamentoPagamentoEletronico.TEDparaTransferenciadeContaInvestimento:
                    return "44 = TED para Transferência de Conta Investimento";
                case FormaLancamentoPagamentoEletronico.PIXTransferencia:
                    return "45 = Pix Transferência";
                case FormaLancamentoPagamentoEletronico.PIXQRCode:
                    return "47 = Pix QR-CODE";
                case FormaLancamentoPagamentoEletronico.DebitoemContaCorrente:
                    return "50 = Débito em Conta Corrente";
                case FormaLancamentoPagamentoEletronico.ExtratoparaGestaodeCaixa:
                    return "70 = Extrato para Gestão de Caixa";
                case FormaLancamentoPagamentoEletronico.DepositoJudicialemContaCorrente:
                    return "71 = Depósito Judicial em Conta Corrente";
                case FormaLancamentoPagamentoEletronico.DepositoJudicialemPoupança:
                    return "72 = Depósito Judicial em Poupança";
                case FormaLancamentoPagamentoEletronico.ExtratodeContaInvestimento:
                    return "73 = Extrato de Conta Investimento";
                case FormaLancamentoPagamentoEletronico.Cadastrodefavorecidos: return "99 = Cadastro de favorecidos(4)";
                default: return string.Empty;
            }
        }

        public static string ObterNumero(this FormaLancamentoPagamentoEletronico finalidadePagamento)
        {
            switch (finalidadePagamento)
            {
                case FormaLancamentoPagamentoEletronico.Padrao: return "00";
                case FormaLancamentoPagamentoEletronico.CreditoemContaCorrente:
                    return "01";
                case FormaLancamentoPagamentoEletronico.ChequePagamentoAdministrativo:
                    return "02";
                case FormaLancamentoPagamentoEletronico.DOCTED:
                    return "03";
                case FormaLancamentoPagamentoEletronico.CartaoSalario:
                    return "04";
                case FormaLancamentoPagamentoEletronico.CreditoemContaPoupança:
                    return "05";
                case FormaLancamentoPagamentoEletronico.OPDisposicao:
                    return "10";
                case FormaLancamentoPagamentoEletronico.PagamentodeContaseTributoscomCodigodeBarras:
                    return "11";
                case FormaLancamentoPagamentoEletronico.TributoDARFNormal:
                    return "16";
                case FormaLancamentoPagamentoEletronico.TributoGPS:
                    return "17";
                case FormaLancamentoPagamentoEletronico.TributoDARFSimples:
                    return "18";
                case FormaLancamentoPagamentoEletronico.TributoIPTU:
                    return "19";
                case FormaLancamentoPagamentoEletronico.PagamentocomAutenticacao:
                    return "20";
                case FormaLancamentoPagamentoEletronico.TributoDARJ:
                    return "21";
                case FormaLancamentoPagamentoEletronico.TributoGARESPICMS:
                    return "22";
                case FormaLancamentoPagamentoEletronico.TributoGARESPDR:
                    return "23";
                case FormaLancamentoPagamentoEletronico.TributoGARESPITCMD:
                    return "24";
                case FormaLancamentoPagamentoEletronico.TributoIPVA:
                    return "25";
                case FormaLancamentoPagamentoEletronico.TributoLicenciamento:
                    return "26";
                case FormaLancamentoPagamentoEletronico.TributoDPVAT:
                    return "27";
                case FormaLancamentoPagamentoEletronico.LiquidacaodeTitulosdoProprioBanco:
                    return "30";
                case FormaLancamentoPagamentoEletronico.PagamentodeTitulosdeOutrosBancos:
                    return "31";
                case FormaLancamentoPagamentoEletronico.ExtratodeContaCorrente:
                    return "40";
                case FormaLancamentoPagamentoEletronico.TEDOutraTitularidade:
                    return "41";
                case FormaLancamentoPagamentoEletronico.TEDMesmaTitularidade:
                    return "43";
                case FormaLancamentoPagamentoEletronico.TEDparaTransferenciadeContaInvestimento:
                    return "44";
                case FormaLancamentoPagamentoEletronico.PIXTransferencia:
                    return "45";
                case FormaLancamentoPagamentoEletronico.PIXQRCode:
                    return "47";
                case FormaLancamentoPagamentoEletronico.DebitoemContaCorrente:
                    return "50";
                case FormaLancamentoPagamentoEletronico.ExtratoparaGestaodeCaixa:
                    return "70";
                case FormaLancamentoPagamentoEletronico.DepositoJudicialemContaCorrente:
                    return "71";
                case FormaLancamentoPagamentoEletronico.DepositoJudicialemPoupança:
                    return "72";
                case FormaLancamentoPagamentoEletronico.ExtratodeContaInvestimento:
                    return "73";
                case FormaLancamentoPagamentoEletronico.Cadastrodefavorecidos: return "99";
                default: return string.Empty;
            }
        }
    }
}
