namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoFaturamentoLote
    {
        Faturamento = 1,
        Cancelamento = 2
    }
    public static class TipoFaturamentoLoteHelper
    {
        public static string ObterDescricao(this TipoFaturamentoLote situacao)
        {
            switch (situacao)
            {
                case TipoFaturamentoLote.Faturamento: return "Faturamento";
                case TipoFaturamentoLote.Cancelamento: return "Cancelamento";
                default: return null;
            }
        }
    }
}
