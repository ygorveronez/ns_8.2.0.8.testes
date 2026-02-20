namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoPagamentoAcertoDespesa
    {
        Outros = 0,
        Motorista = 1,
        Empresa = 2
    }

    public static class TipoPagamentoAcertoDespesaHelper
    {
        public static string ObterDescricao(this TipoPagamentoAcertoDespesa tipoPagamento)
        {
            switch (tipoPagamento)
            {
                case TipoPagamentoAcertoDespesa.Outros: return "Outros";
                case TipoPagamentoAcertoDespesa.Motorista: return "Motorista";
                case TipoPagamentoAcertoDespesa.Empresa: return "Empresa";
                default: return string.Empty;
            }
        }
    }
}
