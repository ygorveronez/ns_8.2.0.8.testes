namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoFuncionarioComissao
    {
        Todos = 0,
        Aprovada = 1,
        AgAprovacao = 2,
        Rejeitada = 3,
        SemRegra = 4,
        Finalizado = 5,
        Cancelado = 6,
    }

    public static class SituacaoFuncionarioComissaoHelper
    {
        public static string ObterDescricao(this SituacaoFuncionarioComissao situacao)
        {
            switch (situacao)
            {
                case SituacaoFuncionarioComissao.Aprovada: return "Aprovada";
                case SituacaoFuncionarioComissao.AgAprovacao: return "Ag. Aprovação";
                case SituacaoFuncionarioComissao.Rejeitada: return "Rejeitada";
                case SituacaoFuncionarioComissao.SemRegra: return "Sem Regra";
                case SituacaoFuncionarioComissao.Finalizado: return "Finalizado";
                case SituacaoFuncionarioComissao.Cancelado: return "Cancelado";
                default: return "";
            }
        }
    }
}
