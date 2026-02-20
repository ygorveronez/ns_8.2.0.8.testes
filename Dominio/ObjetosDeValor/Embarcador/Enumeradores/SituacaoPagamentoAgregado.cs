namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoPagamentoAgregado
    {
        AgAprovacao = 1,
        Finalizado = 2,
        Rejeitada = 3,
        SemRegra = 4,
        Iniciada = 5,
        Cancelado = 6
    }

    public static class SituacaoPagamentoAgregadoHelper
    {
        public static string ObterDescricao(this SituacaoPagamentoAgregado situacao)
        {
            switch (situacao)
            {
                case SituacaoPagamentoAgregado.AgAprovacao: return "Ag. Aprovação";
                case SituacaoPagamentoAgregado.Finalizado: return "Finalizado";
                case SituacaoPagamentoAgregado.Rejeitada: return "Rejeitado";
                case SituacaoPagamentoAgregado.SemRegra: return "Sem Regra";
                case SituacaoPagamentoAgregado.Iniciada: return "Iniciado";
                case SituacaoPagamentoAgregado.Cancelado: return "Cancelado";
                default: return string.Empty;
            }
        }
    }
}
