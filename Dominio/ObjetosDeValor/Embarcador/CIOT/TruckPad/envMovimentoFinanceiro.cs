using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.TruckPad
{
    public class envMovimentoFinanceiro
    {
        public List<envMovimentoFinanceiroInstallments> installments { get; set; }
    }

    public class envMovimentoFinanceiroInstallments
    {
        /// <summary>
        ///<para>Tipo da parcela</para>
        ///<para>"addition" = Acrescimo</para>
        ///<para>"advance" = Adiantamento</para>
        ///<para>"final_balance" = Saldo final</para>
        ///<para>"stay" = Estadia</para>
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// Valor da parcela (base x100)
        /// </summary>
        public decimal value_money { get; set; }

        /// <summary>
        ///<para>Tipo de efetividade da parcela</para>
        ///<para>"manual"</para>
        ///<para>"automatic"</para>
        /// </summary>
        public string effectiveness { get; set; }

        /// <summary>
        ///<para>Status da parcela</para>
        ///<para>"pending"</para>
        ///<para>"released"</para>
        ///<para>"bank_processing"</para>
        ///<para>"error"</para>
        ///<para>"deleted"</para>
        /// </summary>
        public string status { get; set; }

        /// <summary>
        /// Identificação opcional da parcela
        /// </summary>
        public string identification { get; set; }

        /// <summary>
        /// Endereço opcional de origem
        /// </summary>
        public string origin_address { get; set; }

        /// <summary>
        /// Endereço opcional de destino
        /// </summary>
        public string destination_address { get; set; }

        /// <summary>
        /// ID externo da parcela que o cliente utiliza
        /// </summary>
        public string external_client_id { get; set; }

        /// <summary>
        /// Pagamento flexível
        /// </summary>
        public envMovimentoFinanceiroInstallmentsFlexible_payment flexible_payment { get; set; }

    }

    public class envMovimentoFinanceiroInstallmentsFlexible_payment
    {
        /// <summary>
        /// Chave PIX
        /// </summary>
        public string key { get; set; }

        /// <summary>
        ///<para>Recebedor do pagamento</para>
        ///<para>"owner"</para>
        ///<para>"driver"</para>
        /// </summary>
        public string receiver { get; set; }

        /// <summary>
        ///<para>Tipo de pagamento flexível</para>
        ///<para>"bbc"</para>
        ///<para>"pix"</para>
        /// </summary>
        public string type { get; set; }
    }
}