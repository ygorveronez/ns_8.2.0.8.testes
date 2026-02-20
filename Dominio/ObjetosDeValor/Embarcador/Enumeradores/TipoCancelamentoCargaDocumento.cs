namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoCancelamentoCargaDocumento
    {
        Carga = 0,
        Documentos = 1,
        TodosDocumentos = 2
    }

    public static class TipoCancelamentoCargaDocumentoHelper
    {
        public static string ObterDescricao(this TipoCancelamentoCargaDocumento tipoCancelamento)
        {
            switch (tipoCancelamento)
            {
                case TipoCancelamentoCargaDocumento.Carga: return "Carga";
                case TipoCancelamentoCargaDocumento.Documentos: return "Documento";
                case TipoCancelamentoCargaDocumento.TodosDocumentos: return "Todos Documentos";
                default: return string.Empty;
            }
        }
    }
}
