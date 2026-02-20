namespace Dominio.ObjetosDeValor.Enumerador
{
    public enum TipoIntegracaoCanhoto
    {
        Nenhum = 0,
        FTP = 1
    }

    public static class TipoIntegracaoCanhotoHelper
    {
        public static string ObterDescricao(this TipoIntegracaoCanhoto tipoIntegracaoCanhoto)
        {
            switch (tipoIntegracaoCanhoto)
            {
                case TipoIntegracaoCanhoto.FTP: return "FTP";
                default: return string.Empty;
            }
        }
    }
}