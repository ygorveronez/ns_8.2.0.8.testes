namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoLogSistema
    {
        Error = 1,
        Info = 2,
        Advertencia = 3,
        Debug = 4
    }

    public static class TipoLogSistemalHelper
    {
        public static string ObterDescricao(this TipoLogSistema tipoLogSistema)
        {
            switch (tipoLogSistema)
            {
                case TipoLogSistema.Error: return "Erro";
                case TipoLogSistema.Info: return "Informação";
                case TipoLogSistema.Advertencia: return "Advertência";
                case TipoLogSistema.Debug: return "Debug";
                default: return string.Empty;
            }
        }
    }
}
