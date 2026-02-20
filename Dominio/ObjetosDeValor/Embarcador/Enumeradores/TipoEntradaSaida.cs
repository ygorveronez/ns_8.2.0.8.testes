namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoEntradaSaida
    {
        Todos = 0,
        Entrada = 1,
        Saida = 2
    }

    public static class TipoEntradaSaidaHelper
    {
        public static string ObterDescricao(this TipoEntradaSaida tipo)
        {
            switch (tipo)
            {
                case TipoEntradaSaida.Entrada: return "Entrada";
                case TipoEntradaSaida.Saida: return "Saída";
                default: return string.Empty;
            }
        }

        public static TipoEntradaSaida ObterTipoInverso(this TipoEntradaSaida tipo)
        {
            switch (tipo)
            {
                case TipoEntradaSaida.Entrada: return TipoEntradaSaida.Saida;
                case TipoEntradaSaida.Saida: return TipoEntradaSaida.Entrada;
                default: return TipoEntradaSaida.Entrada;
            }
        }
    }
}