namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoDocumentoEntrada
    {
        Aberto = 1,
        Cancelado = 2,
        Finalizado = 3,
        Anulado = 4
    }

    public static class SituacaoDocumentoEntradaHelper
    {
        public static string ObterDescricao(this SituacaoDocumentoEntrada situacao)
        {
            switch (situacao)
            {
                case SituacaoDocumentoEntrada.Aberto: return "Aberto";
                case SituacaoDocumentoEntrada.Cancelado: return "Cancelado";
                case SituacaoDocumentoEntrada.Finalizado: return "Finalizado";
                case SituacaoDocumentoEntrada.Anulado: return "Anulado";
                default: return string.Empty;
            }
        }
    }
}
