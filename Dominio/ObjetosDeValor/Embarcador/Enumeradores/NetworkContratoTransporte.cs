namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum NetworkContratoTransporte
    {
        MarketAfrica = 0,
        MarketAsia = 1,
        MarketEurope = 2,
        MarketNamet = 3,
        MarketNorthAmerica = 4,
        MarketSouthAmerica = 5,
    }

    public static class NetworkContratoTransporteHelper
    {
        public static string ObterDescricao(this NetworkContratoTransporte network)
        {
            switch (network)
            {
                case NetworkContratoTransporte.MarketAfrica: return "Market Africa";
                case NetworkContratoTransporte.MarketAsia: return "Market Asia";
                case NetworkContratoTransporte.MarketEurope: return "Market Europe";
                case NetworkContratoTransporte.MarketNamet: return "Market Namet";
                case NetworkContratoTransporte.MarketNorthAmerica: return "Market North America";
                case NetworkContratoTransporte.MarketSouthAmerica: return "Market South America";
                default: return string.Empty;
            }
        }
    }
}