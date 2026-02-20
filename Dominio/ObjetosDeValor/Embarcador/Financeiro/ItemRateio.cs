namespace Dominio.ObjetosDeValor.Embarcador.Financeiro
{
    public class ItemRateio
    {
        public int Codigo { get; set; }
        public decimal ValorTotal { get; set; }
        public decimal ValorRateado { get; set; }
        public decimal ValorMoedaTotal { get; set; }
        public decimal ValorMoedaRateado { get; set; }
        public decimal ValorAcrescimoRateado { get; set; }
        public int Parcela { get; set; }
    }
}
