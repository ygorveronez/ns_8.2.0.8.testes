namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoEmissaoDocumentoOcorrencia
    {
        Todos = 0,
        SomenteFilialEmissora = 1,
        SomenteSubcontratada = 2
    }

    public static class TipoEmissaoDocumentoOcorrenciaHelper
    {
        public static string ObterDescricao(this TipoEmissaoDocumentoOcorrencia tipo)
        {
            switch (tipo)
            {
                case TipoEmissaoDocumentoOcorrencia.SomenteFilialEmissora: return "Somente Filial Emissora";
                case TipoEmissaoDocumentoOcorrencia.SomenteSubcontratada: return "Somente Subcontratada";
                case TipoEmissaoDocumentoOcorrencia.Todos: return "Todos";
                default: return string.Empty;
            }
        }
    }
}
