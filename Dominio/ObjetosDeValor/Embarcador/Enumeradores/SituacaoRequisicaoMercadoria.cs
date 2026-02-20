namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoRequisicaoMercadoria
    {
        Todos = 0,
        Aprovada = 1,
        AgAprovacao = 2,
        Rejeitada = 3,
        SemRegra = 4,
        Finalizado = 5,
        Cancelado = 6,
    }

    public static class SituacaoRequisicaoMercadoriaHelper
    {
        public static string ObterDescricao(this SituacaoRequisicaoMercadoria situacao)
        {
            switch (situacao)
            {
                case SituacaoRequisicaoMercadoria.Aprovada: return "Aprovada";
                case SituacaoRequisicaoMercadoria.AgAprovacao: return "Ag. Aprovação";
                case SituacaoRequisicaoMercadoria.Rejeitada: return "Rejeitada";
                case SituacaoRequisicaoMercadoria.SemRegra: return "Sem Regra";
                case SituacaoRequisicaoMercadoria.Finalizado: return "Finalizado";
                case SituacaoRequisicaoMercadoria.Cancelado: return "Cancelado";
                default: return "";
            }
        }
    }
}
