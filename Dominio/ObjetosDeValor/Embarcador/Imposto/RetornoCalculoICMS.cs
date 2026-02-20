namespace Dominio.ObjetosDeValor.Embarcador.Imposto
{
    public class RetornoCalculoICMS
    {
        public int IDProposta { get; set; }
        public decimal Aliquota { get; set; }
        public decimal ValorImposto { get; set; }
        public decimal ValorTotalComImposto { get; set; }
    }
}
