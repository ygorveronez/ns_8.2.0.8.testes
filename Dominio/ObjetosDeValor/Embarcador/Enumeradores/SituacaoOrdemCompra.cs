namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoOrdemCompra
    {
        AgAprovacao = 1,
        Aberta = 2,
        AgRetorno = 3,
        Finalizada = 4,
        Cancelada = 5,
        SemRegra = 6,
        Rejeitada = 7,
        Aprovada = 8,
        Incompleta = 9
    }

    public static class SituacaoOrdemCompraHelper
    {
        public static string ObterDescricao(this SituacaoOrdemCompra situacao)
        {
            switch (situacao)
            {
                case SituacaoOrdemCompra.AgAprovacao: return "Ag. Aprovação";
                case SituacaoOrdemCompra.Aberta: return "Aberta";
                case SituacaoOrdemCompra.AgRetorno: return "Ag. Retorno";
                case SituacaoOrdemCompra.Finalizada: return "Finalizada";
                case SituacaoOrdemCompra.Cancelada: return "Cancelada";
                case SituacaoOrdemCompra.SemRegra: return "Sem Regra";
                case SituacaoOrdemCompra.Rejeitada: return "Rejeitada";
                case SituacaoOrdemCompra.Aprovada: return "Aprovada";
                case SituacaoOrdemCompra.Incompleta: return "Incompleta";
                default: return string.Empty;
            }
        }
    }
}
