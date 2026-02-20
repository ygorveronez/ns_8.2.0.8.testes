namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoProbe
    {
        NaoDefinido = 0,
        Probe = 1,
        Termografo = 2
    }

    public static class TipoProbeHelper
    {
        public static string ObterDescricao(this TipoProbe tipo)
        {
            switch (tipo)
            {
                case TipoProbe.NaoDefinido: return "Não Definido";
                case TipoProbe.Probe: return "Probe";
                case TipoProbe.Termografo: return "Termógrafo";
                default: return string.Empty;
            }
        }
    }
}
