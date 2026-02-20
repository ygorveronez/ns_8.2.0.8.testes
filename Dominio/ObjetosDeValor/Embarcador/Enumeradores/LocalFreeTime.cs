namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum LocalFreeTime
    {
        Nenhum = 0,
        Coleta = 1,
        Fronteira = 2,
        Entrega = 3,
        LocalParqueamento = 4,
        Todos = 99
    }

    public static class LocalFreeTimeHelper
    {
        public static string ObterDescricao(this LocalFreeTime local)
        {
            switch (local)
            {
                case LocalFreeTime.Nenhum: return "Nenhum";
                case LocalFreeTime.Coleta: return "Coleta";
                case LocalFreeTime.Fronteira: return "Fronteira";
                case LocalFreeTime.Entrega: return "Entrega";
                case LocalFreeTime.LocalParqueamento: return "Local de Parqueamento";
                case LocalFreeTime.Todos: return "Todos";
                default: return string.Empty;
            }
        }
    }
}
