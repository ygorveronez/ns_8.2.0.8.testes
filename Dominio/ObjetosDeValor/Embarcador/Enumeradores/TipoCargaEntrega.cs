namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoCargaEntrega
    {
        Entrega = 1,
        Coleta = 2,
        Fronteira = 3,
    }


    public static class TipoCargaEntregaHelper
    {

        public static string ObterDescricao(this TipoCargaEntrega situacaoEnterga)
        {
            switch (situacaoEnterga)
            {
                case TipoCargaEntrega.Entrega: return "Entrega";
                case TipoCargaEntrega.Coleta: return "Coleta";
                case TipoCargaEntrega.Fronteira: return "Fronteira";
                default: return "";
            }
        }

    }
}

