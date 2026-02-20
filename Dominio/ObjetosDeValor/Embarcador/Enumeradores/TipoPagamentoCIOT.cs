namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    /**
     * <summary>
     * ##### SemPgto : 0
     * ##### Cartao : 1
     * ##### Deposito : 2
     * ##### Transferencia : 3
     * </summary>
     * */


    public enum TipoPagamentoCIOT
    {
        SemPgto = 0,
        Cartao = 1,
        Deposito = 2,
        Transferencia = 3,
        PIX = 4,
        BBC = 5,
    }
    public static class TipoPagamentoCIOTHelper
    {
        public static TipoPagamentoCIOT ObterTipoPagamento(int codigo)
        {
            switch (codigo)
            {
                case 0: return TipoPagamentoCIOT.SemPgto;
                case 1: return TipoPagamentoCIOT.Cartao;
                case 2: return TipoPagamentoCIOT.Deposito;
                case 3: return TipoPagamentoCIOT.Transferencia;
                case 4: return TipoPagamentoCIOT.PIX;
                case 5: return TipoPagamentoCIOT.BBC;
                default: return TipoPagamentoCIOT.SemPgto;
            }
        }

        public static int ObterNumeradorTipoPagamento(this TipoPagamentoCIOT tipo)
        {
            switch (tipo)
            {
                case TipoPagamentoCIOT.SemPgto: return 0;
                case TipoPagamentoCIOT.Cartao: return 1;
                case TipoPagamentoCIOT.Deposito: return 2;
                case TipoPagamentoCIOT.Transferencia: return 3;
                case TipoPagamentoCIOT.PIX: return 4;
                case TipoPagamentoCIOT.BBC: return 5;
                default: return 0;
            }
        }
        public static string ObterDescricao(this TipoPagamentoCIOT operadoraCIOT)
        {
            switch (operadoraCIOT)
            {
                case TipoPagamentoCIOT.SemPgto: return Localization.Resources.Enumeradores.TipoPagamentoCIOT.SemPagamento;
                case TipoPagamentoCIOT.Cartao: return Localization.Resources.Enumeradores.TipoPagamentoCIOT.Cartao;
                case TipoPagamentoCIOT.Deposito: return Localization.Resources.Enumeradores.TipoPagamentoCIOT.Deposito;
                case TipoPagamentoCIOT.Transferencia: return Localization.Resources.Enumeradores.TipoPagamentoCIOT.Transferencia;
                case TipoPagamentoCIOT.PIX: return Localization.Resources.Enumeradores.TipoPagamentoCIOT.PIX;
                case TipoPagamentoCIOT.BBC: return Localization.Resources.Enumeradores.TipoPagamentoCIOT.BBC;
                default: return "";
            }
        }
    }


}
