namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoParadaEventoSuperApp
    {
        Coleta = 0,
        Entrega = 1,
        Ambos = 2
    }

    public static class TipoParadaEventoSuperAppHelper
    {
        public static string ObterDescricao(this TipoParadaEventoSuperApp TipoParada)
        {
            switch (TipoParada)
            {
                case TipoParadaEventoSuperApp.Coleta: return "Coleta";
                case TipoParadaEventoSuperApp.Entrega: return "Entrega";
                case TipoParadaEventoSuperApp.Ambos: return "Ambos";
                default: return string.Empty;
            }
        }
    }
}