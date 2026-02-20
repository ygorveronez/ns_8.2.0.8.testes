namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoDocumentoPagamento
    {
        Liberado = 1,
        Bloqueado = 2
    }

    public static class SituacaoDocumentoPagamentoHelper
    {
        public static string ObterDescricao(this SituacaoDocumentoPagamento tipo)
        {
            switch (tipo)
            {
                case SituacaoDocumentoPagamento.Liberado: return "Liberado";
                case SituacaoDocumentoPagamento.Bloqueado: return "Bloqueado";
                default: return string.Empty;
            }
        }
    }
}
