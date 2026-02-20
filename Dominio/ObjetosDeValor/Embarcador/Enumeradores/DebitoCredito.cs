namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum DebitoCredito
    {
        /// <summary>
        /// Sempre entra na conta, mais, positivo, sempre pra dentro
        /// </summary>
        Debito = 1,
        /// <summary>
        /// Sempre sai da conta, menos, negativo, sempre pra fora
        /// </summary>
        Credito = 2,        
        ContasAPagar = 3,
        ContasAReceber = 4,
        NotaDebito = 5,
        NotaCredito = 6

    }

    public static class DebitoCreditoHelper
    {
        public static string ObterDescricao(this DebitoCredito tipoDebitoCredito)
        {
            switch (tipoDebitoCredito)
            {
                case DebitoCredito.Debito: return "Débito";
                case DebitoCredito.Credito: return "Crédito";                
                case DebitoCredito.ContasAPagar: return "Contas a Pagar";
                case DebitoCredito.ContasAReceber: return "Contas a Pagar";
                case DebitoCredito.NotaDebito: return "Nota de Débito";
                case DebitoCredito.NotaCredito: return "Nota de Crédito";
                default: return string.Empty;
            }
        }
    }
}
