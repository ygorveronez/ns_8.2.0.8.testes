namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoEnvioTransporteMaritimo
    {
        TON = 1,
        FCL = 2
    }

    public static class TipoEnvioTransporteMaritimoHelper
    {
        public static string ObterDescricao(this TipoEnvioTransporteMaritimo tipoEnvio)
        {
            switch (tipoEnvio)
            {
                case TipoEnvioTransporteMaritimo.TON: return "1 - TON";
                default: return "2 - FCL";
            }
        }
    }
}
