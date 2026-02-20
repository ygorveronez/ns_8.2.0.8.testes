namespace Dominio.ObjetosDeValor.Embarcador.CIOT.TruckPad
{
    public class envAutorizacaoPagamento
    {
        /// <summary>
        /// Possui desconto
        /// </summary>
        public bool has_discount { get; set; }

        /// <summary>
        /// Dados de desconto
        /// </summary>
        public envAutorizacaoPagamentoDiscount discount { get; set; }

        /// <summary>
        /// Pagamento flexível
        /// </summary>
        public envAutorizacaoPagamentoFlexiblePayment flexible_payment { get; set; }
    }

    public class envAutorizacaoPagamentoDiscount
    {
        /// <summary>
        /// Valor do desconto
        /// </summary>
        public decimal value_money { get; set; }

        /// <summary>
        /// Motivo do desconto
        /// </summary>
        public string reason { get; set; }
    }

    public class envAutorizacaoPagamentoFlexiblePayment
    {
        /// <summary>
        /// Chave PIX
        /// </summary>
        public string key { get; set; }

        /// <summary>
        /// <para>Recebedor do pagamento</para>
        /// <para>"owner"</para>
        /// <para>"driver"</para>
        /// </summary>
        public string receiver { get; set; }

        /// <summary>
        /// <para>Tipo de pagamento flexível</para>
        /// <para>"bbc"</para>
        /// <para>"pix"</para>
        /// </summary>
        public string type { get; set; }
    }
}