namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoEnvioArquivo
    {
        Todos = 0,
        Manual = 1,
        FTP = 2,
        Email = 3
    }

    public static class TipoEnvioArquivoHelper
    {
        public static string ObterDescricao(this TipoEnvioArquivo tipoEnvioArquivo)
        {
            switch (tipoEnvioArquivo)
            {
                case TipoEnvioArquivo.Manual: return "Manual";
                case TipoEnvioArquivo.FTP: return "FTP";
                case TipoEnvioArquivo.Email: return "E-mail";
                default: return "";
            }
        }
    }
}
