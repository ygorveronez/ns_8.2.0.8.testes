namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum StatusFinanceiroDocumentoEntrada
    {
        Aberto = 1,
        Renegociado = 2,
        Pago = 3,
        ContratoFinanciamento = 4
    }

    public static class StatusFinanceiroDocumentoEntradaHelper
    {
        public static string ObterDescricao(this StatusFinanceiroDocumentoEntrada status)
        {
            switch (status)
            {
                case StatusFinanceiroDocumentoEntrada.Aberto: return "Aberto";
                case StatusFinanceiroDocumentoEntrada.Renegociado: return "Renegociado";
                case StatusFinanceiroDocumentoEntrada.Pago: return "Pago";
                case StatusFinanceiroDocumentoEntrada.ContratoFinanciamento: return "Contrato de Financiamento";
                default: return string.Empty;
            }
        }
    }
}
