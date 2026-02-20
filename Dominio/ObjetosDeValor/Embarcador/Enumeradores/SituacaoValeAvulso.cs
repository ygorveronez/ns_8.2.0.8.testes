namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoValeAvulso
    {
        Todos = 0,
        Aberto = 1,
        Cancelado = 2,
        Finalizado = 3,
    }

    public static class SituacaoValeAvulsoHelper
    {
        public static string ObterDescricao(this SituacaoValeAvulso situacao)
        {
            switch (situacao)
            {
                case SituacaoValeAvulso.Todos: return "Todos";
                case SituacaoValeAvulso.Aberto: return "Aberto";
                case SituacaoValeAvulso.Cancelado: return "Cancelado";
                case SituacaoValeAvulso.Finalizado: return "Finalizado";
                default: return string.Empty;
            }
        }
    }
}
