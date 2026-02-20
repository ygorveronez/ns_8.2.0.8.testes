namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoPedagio
    {
        Todos = 0,
        Inconsistente = 1,
        Lancado = 2,
        Fechado = 3,
    }

    public static class SituacaoPedagioHelper
    {
        public static string ObterDescricao(this SituacaoPedagio situacao)
        {
            switch (situacao)
            {
                case SituacaoPedagio.Inconsistente: return "Inconsistente";
                case SituacaoPedagio.Lancado: return "Lançado";
                case SituacaoPedagio.Fechado: return "Fechado";
                default: return string.Empty;
            }
        }
    }
}
