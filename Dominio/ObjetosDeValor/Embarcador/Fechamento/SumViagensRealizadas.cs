namespace Dominio.ObjetosDeValor.Embarcador.Fechamento
{
    public class SumViagensRealizadas
    {
        public int TotalViagens { get; set; }

        public decimal ValorTotalPagoTabela { get; set; }

        public decimal AdicionalKM { get; set; }

        public decimal Total
        {
            get
            {
                return ValorTotalPagoTabela + AdicionalKM;
            }
        }

        public decimal ValorMedioPagoTabela {
            get
            {
                return ValorTotalPagoTabela / TotalViagens;
            }
        }
    }
}
