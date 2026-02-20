namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoArquivoCargaIntegracaoEDI
    {
        NaoEspecificado = 0,
        Creacion = 1,
        GoodsIssue = 2,
        Deletion = 3
    }

    public static class TipoArquivoCargaIntegracaoEDIHelper
    {
        public static string ObterDescricao(this TipoArquivoCargaIntegracaoEDI tipoArquivo)
        {
            switch (tipoArquivo)
            {
                case TipoArquivoCargaIntegracaoEDI.Creacion: return "Creacion";
                case TipoArquivoCargaIntegracaoEDI.GoodsIssue: return "Goods Issue";
                case TipoArquivoCargaIntegracaoEDI.Deletion: return "Deletion";
                default: return "";
            }
        }
    }
}
