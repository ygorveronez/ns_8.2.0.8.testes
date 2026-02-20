namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoDocumentoGerado
    {
        SomenteCargas = 1,
        SomenteOcorrencias = 2
    }

    public static class TipoDocumentoGeradoHelper
    {
        public static string ObterDescricao(this TipoDocumentoGerado tipo)
        {
            switch (tipo)
            {
                case TipoDocumentoGerado.SomenteCargas: return "Somente Cargas";
                case TipoDocumentoGerado.SomenteOcorrencias: return "Somente Ocorrências";
                default: return string.Empty;
            }
        }
    }
}
