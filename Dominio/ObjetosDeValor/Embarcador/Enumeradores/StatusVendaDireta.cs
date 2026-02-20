namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum StatusVendaDireta
    {
        Todos = 0,
        Pendente = 1,
        Finalizado = 2,
        Cancelado = 3
    }

    public static class StatusVendaDiretaHelper
    {
        public static string ObterDescricao(this StatusVendaDireta status)
        {
            switch (status)
            {
                case StatusVendaDireta.Pendente: return "Pendente";
                case StatusVendaDireta.Finalizado: return "Finalizado";
                case StatusVendaDireta.Cancelado: return "Cancelado";
                default: return string.Empty;
            }
        }
    }
}
