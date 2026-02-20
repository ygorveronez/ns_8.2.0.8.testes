namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoTransportadorBidding
    {
        Titular = 1,
        Spot = 2
    }

    public static class TipoTransportadorBiddingHelper
    {
        public static string ObterDescricao(this TipoTransportadorBidding tipo)
        {
            switch (tipo)
            {
                case TipoTransportadorBidding.Titular: return "Titular";
                case TipoTransportadorBidding.Spot: return "Spot";
                default: return string.Empty;
            }
        }
    }
}
