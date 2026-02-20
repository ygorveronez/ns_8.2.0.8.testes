namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum ModalidadePagamentoEletronico
    {
        CC_CreditoContaCorrente = 1,
        OP_ChequeOP = 2,
        DOC_DOCCompre = 3,
        CCR_CreditoConta = 4,
        TDC_TEDCip = 5,
        TDS_TEDSTR = 6,
        TT_TituloTerceiro = 7,
        TRB_Tributos = 8,
        CCT_ContasConsumoTributo = 9,
        PIX = 10
    }

    public static class ModalidadePagamentoEletronicoHelper
    {
        public static string ObterDescricao(this ModalidadePagamentoEletronico modalidade)
        {
            switch (modalidade)
            {
                case ModalidadePagamentoEletronico.CC_CreditoContaCorrente: return "CC - Crédito em conta corrente";
                case ModalidadePagamentoEletronico.OP_ChequeOP: return "OP - Cheque OP";
                case ModalidadePagamentoEletronico.DOC_DOCCompre: return "DOC - DOC COMPRE";
                case ModalidadePagamentoEletronico.CCR_CreditoConta: return "CCR - Crédito Conta";
                case ModalidadePagamentoEletronico.TDC_TEDCip: return "Boleto";
                case ModalidadePagamentoEletronico.TDS_TEDSTR: return "TDS - TED STR";
                case ModalidadePagamentoEletronico.TT_TituloTerceiro: return "TT - Titulos de Terceiro";
                case ModalidadePagamentoEletronico.TRB_Tributos: return "TRB - Tributos (Sem Código de Barras)";
                case ModalidadePagamentoEletronico.CCT_ContasConsumoTributo: return "CCT - Contas de Consumo/Tributos (Com Código de Barras)";
                case ModalidadePagamentoEletronico.PIX: return "PIX";
                default: return string.Empty;
            }
        }

        public static string ObterNumero(this ModalidadePagamentoEletronico modalidade)
        {
            switch (modalidade)
            {
                case ModalidadePagamentoEletronico.CC_CreditoContaCorrente: return "CC";
                case ModalidadePagamentoEletronico.OP_ChequeOP: return "OP";
                case ModalidadePagamentoEletronico.DOC_DOCCompre: return "DOC";
                case ModalidadePagamentoEletronico.TDC_TEDCip: return "TDC";
                case ModalidadePagamentoEletronico.TDS_TEDSTR: return "TDS";
                case ModalidadePagamentoEletronico.TT_TituloTerceiro: return "TT";
                case ModalidadePagamentoEletronico.TRB_Tributos: return "TRB";
                case ModalidadePagamentoEletronico.CCT_ContasConsumoTributo: return "CCT";
                case ModalidadePagamentoEletronico.PIX: return "PIX";
                default: return string.Empty;
            }
        }
    }
}
