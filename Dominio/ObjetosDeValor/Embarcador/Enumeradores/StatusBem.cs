namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum StatusBem
    {
        Todos = 0,
        Aberto = 1,
        Finalizado = 2,
        Cancelado = 3
    }

    public static class StatusBemHelper
    {
        public static string ObterDescricao(this StatusBem status)
        {
            switch (status)
            {
                case StatusBem.Aberto: return "Aberto";
                case StatusBem.Finalizado: return "Finalizado";
                case StatusBem.Cancelado: return "Cancelado";
                default: return string.Empty;
            }
        }
    }
}
