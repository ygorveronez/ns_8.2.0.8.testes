namespace Dominio.ObjetosDeValor.Embarcador.Fechamento
{
    public class SumFechamento
    {
        public decimal TotalJaPagoTabela { get; set; }

        public decimal TotalAcordado { get; set; }

        public decimal TotalFranquia { get; set; }

        public decimal FranquiaExcedente { get; set; }

        public decimal Diferenca
        {
            get
            {
                return TotalAcordado + FranquiaExcedente - TotalJaPagoTabela;
            }
        }

    }
}
