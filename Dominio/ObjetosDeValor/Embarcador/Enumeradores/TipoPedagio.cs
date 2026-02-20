namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoPedagio
    {
        Todos = 0,
        Debito = 1,
        Credito = 2
    }

    public static class TipoPedagioHelper
    {
        public static string ObterDescricao(this TipoPedagio tipo)
        {
            switch (tipo)
            {
                case TipoPedagio.Debito: return "Débito";
                case TipoPedagio.Credito: return "Crédito";
                default: return string.Empty;
            }
        }
    }
}
