namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoAplicacaoGatilhoTracking 
    {
        AplicarSempre = 0,
        Coleta = 1,
        Entrega = 2,
    }

    public static class TipoAplicacaoGatilhoTrackingHelper
    {
        public static string ObterDescricao(this TipoAplicacaoGatilhoTracking tipo)
        {
            switch (tipo)
            {
                case TipoAplicacaoGatilhoTracking.AplicarSempre: return "Aplicar Sempre";
                case TipoAplicacaoGatilhoTracking.Coleta: return "Coleta";
                case TipoAplicacaoGatilhoTracking.Entrega: return "Entrega";
                default: return string.Empty;
            }
        }
    }
}
