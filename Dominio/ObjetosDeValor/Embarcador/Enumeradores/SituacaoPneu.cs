namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoPneu
    {
        Todos = 0,
        Disponivel = 1,
        EmUso = 2,
        Reforma = 3,
        Sucata = 4
    }

    public static class SituacaoPneuHelper
    {
        public static string ObterDescricao(this SituacaoPneu vida)
        {
            switch (vida)
            {
                case SituacaoPneu.Todos: return "Todos";
                case SituacaoPneu.Disponivel: return "Disponível";
                case SituacaoPneu.EmUso: return "Em Uso";
                case SituacaoPneu.Reforma: return "Reforma";
                case SituacaoPneu.Sucata: return "Sucata";
                default: return string.Empty;
            }
        }
    }
}
