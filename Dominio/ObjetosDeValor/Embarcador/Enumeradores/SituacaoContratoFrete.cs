namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoContratoFrete
    {
        todos = 0,
        Aprovado = 1,
        AgAprovacao = 2,
        Rejeitado = 3,
        Finalizada = 4,
        Cancelado = 5,
        SemRegra = 6,
        Aberto = 7
    }

    public static class SituacaoContratoFreteHelper
    {
        public static string ObterDescricao(this SituacaoContratoFrete situacao)
        {
            switch (situacao)
            {
                case SituacaoContratoFrete.todos: return "Todos";
                case SituacaoContratoFrete.Aprovado: return "Aprovado";
                case SituacaoContratoFrete.AgAprovacao: return "Ag. Aprovação";
                case SituacaoContratoFrete.Rejeitado: return "Rejeitado";
                case SituacaoContratoFrete.Finalizada: return "Finalizado";
                case SituacaoContratoFrete.Cancelado: return "Cancelado";
                case SituacaoContratoFrete.SemRegra: return "Sem Regra";
                case SituacaoContratoFrete.Aberto: return "Aberto";
                default: return "";
            }
        }
    }
}
