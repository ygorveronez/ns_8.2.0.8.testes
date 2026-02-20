namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoDocumentoFaturamento
    {
        Todos = 0,
        Autorizado = 1,
        Cancelado = 2,
        Anulado = 3,
        EmFechamento = 4,
        Liquidado = 5,
        EmCancelamento = 6
    }

    public static class SituacaoDocumentoFaturamentoHelper
    {
        public static string ObterDescricao(this SituacaoDocumentoFaturamento situacao)
        {
            switch (situacao)
            {
                case SituacaoDocumentoFaturamento.Autorizado: return "Autorizado";
                case SituacaoDocumentoFaturamento.Cancelado: return "Cancelado";
                case SituacaoDocumentoFaturamento.Anulado: return "Anulado";
                case SituacaoDocumentoFaturamento.EmFechamento: return "Em Fechamento";
                case SituacaoDocumentoFaturamento.Liquidado: return "Liquidado";
                case SituacaoDocumentoFaturamento.EmCancelamento: return "Em Cancelamento";
                default: return string.Empty;
            }
        }
    }
}
