namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum StatusTitulo
    {
        Todos = 0,
        EmAberto = 1,
        Atrazada = 2,
        Quitada = 3,
        Cancelado = 4,
        EmNegociacao = 5,
        Bloqueado = 6,
        Antecipado = 7
    }

    public static class StatusTituloHelper
    {
        public static string ObterDescricao(this StatusTitulo status)
        {
            switch (status)
            {
                case StatusTitulo.Atrazada: return "Atrasado";
                case StatusTitulo.Cancelado: return "Cancelado";
                case StatusTitulo.EmAberto: return "Em Aberto";
                case StatusTitulo.EmNegociacao: return "Em Negociação";
                case StatusTitulo.Quitada: return "Quitado";
                case StatusTitulo.Bloqueado: return "Bloqueado";
                case StatusTitulo.Antecipado: return "Antecipado";
                default: return string.Empty;
            }
        }
    }
}
