namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoDocumentoContribuinte
    {
        AgDocTransportador = 0,
        AgAnaliseDocumentacao = 1,
        Aprovado = 2,
        Rejeitado = 3
    }

    public static class SituacaoDocumentoContribuinteHelper
    {
        public static string ObterDescricao(this SituacaoDocumentoContribuinte situacao)
        {
            switch (situacao)
            {
                case SituacaoDocumentoContribuinte.AgDocTransportador: return "Ag. Doc. Transportador";
                case SituacaoDocumentoContribuinte.AgAnaliseDocumentacao: return "Ag. Análise Documentação";
                case SituacaoDocumentoContribuinte.Aprovado: return "Aprovado";
                case SituacaoDocumentoContribuinte.Rejeitado: return "Rejeitado";
                default: return "";
            }
        }
    }
}
