namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoDaEntrega
    {
        Entrega = 1,
        Coleta = 2
    }

    public static class TipoDaEntregaHelper
    {
        public static string ObterDescricao(this TipoDaEntrega tipo)
        {
            switch (tipo)
            {
                case TipoDaEntrega.Entrega: return "Entrega";
                case TipoDaEntrega.Coleta: return "Coleta";
                default: return string.Empty;
            }
        }

        public static int ObterCentroCusto(this TipoDaEntrega tipo)
        {
            switch (tipo)
            {
                case TipoDaEntrega.Entrega: return 50340;
                case TipoDaEntrega.Coleta: return 50280;
                default: return 0;
            }
        }
    }
}
