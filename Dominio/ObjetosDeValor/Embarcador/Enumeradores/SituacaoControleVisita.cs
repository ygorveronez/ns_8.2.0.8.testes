namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoControleVisita
    {
        Todos = 0,
        Aberto = 1,
        Fechado = 2
    }

    public static class SituacaoControleVisitaHelper
    {
        public static string ObterDescricao(this SituacaoControleVisita situacaoControleVisita)
        {
            switch (situacaoControleVisita)
            {
                case SituacaoControleVisita.Todos:
                    return "Todos";
                case SituacaoControleVisita.Aberto:
                    return "Aberto";
                case SituacaoControleVisita.Fechado:
                    return "Fechado";
                default:
                    return string.Empty;
            }
        }
    }
}
