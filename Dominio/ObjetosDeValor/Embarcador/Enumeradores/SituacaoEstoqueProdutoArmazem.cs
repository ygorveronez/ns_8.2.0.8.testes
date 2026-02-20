namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoEstoqueProdutoArmazem
    {
        Todos = 0,
        EstoqueTotal = 1,
        EstoqueParcial = 2,
        SemEstoque = 3,
    }

    public static class SituacaoEstoqueProdutoArmazemHelper
    {
        public static string ObterDescricao(this SituacaoEstoqueProdutoArmazem situacaoEstoqueProdutoArmazem)
        {
            switch (situacaoEstoqueProdutoArmazem)
            {
                case SituacaoEstoqueProdutoArmazem.Todos:
                    return "Todos";
                case SituacaoEstoqueProdutoArmazem.EstoqueTotal:
                    return "Estoque Total";
                case SituacaoEstoqueProdutoArmazem.EstoqueParcial:
                    return "Estoque Parcial";
                case SituacaoEstoqueProdutoArmazem.SemEstoque:
                    return "Sem Estoque";
                default:
                    return string.Empty;
            }
        }
    }
}
