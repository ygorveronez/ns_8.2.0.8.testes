using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.TruckPad
{
    public class retMovimentoFinanceiro
    {
        public string code { get; set; }
        public List<retMovimentoFinanceiroInstallments> installments { get; set; }
    }

    public class retMovimentoFinanceiroInstallments
    {
        public string id { get; set; }
        public decimal value_money { get; set; }
        public string type { get; set; }
        public string status { get; set; }
        public string note { get; set; }
    }
}