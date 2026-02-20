namespace Dominio.ObjetosDeValor.Enumerador
{
    public enum TipoComponentePagamento
    {
        ValePedagio = 1,
        Impostos = 2,
        Despesas = 3,
        FreteValor = 4,
        Outros = 5
    }

    public static class TipoComponentePagamentoHelper
    {
        public static string ObterDescricao(this TipoComponentePagamento acao)
        {
            switch (acao)
            {
                case TipoComponentePagamento.ValePedagio: return "ValePedagio";
                case TipoComponentePagamento.Impostos: return "Impostos";
                case TipoComponentePagamento.Despesas: return "Despesas";
                case TipoComponentePagamento.FreteValor: return "FreteValor";
                default: return "Outros";
            }
        }
    }
}
