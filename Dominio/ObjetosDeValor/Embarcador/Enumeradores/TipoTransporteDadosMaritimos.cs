namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoTransporteDadosMaritimos
    {
        NaoDefinido = 0,
        Cheio = 1,
        Picado = 2,
        Solto = 3
    }

    public static class TipoTransporteDadosMaritimosHelper
    {
        public static string ObterDescricao(this TipoTransporteDadosMaritimos tipo)
        {
            switch (tipo)
            {
                case TipoTransporteDadosMaritimos.Cheio: return "Cheio";
                case TipoTransporteDadosMaritimos.Picado: return "Picado";
                case TipoTransporteDadosMaritimos.Solto: return "Solto";
                default: return string.Empty;
            }
        }
    }
}