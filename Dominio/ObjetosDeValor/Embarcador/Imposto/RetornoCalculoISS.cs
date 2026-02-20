namespace Dominio.ObjetosDeValor.Embarcador.Imposto
{
    public class RetornoCalculoISS
    {
        public int IDProposta { get; set; }
        public decimal AliquotaOrigem { get; set; }
        public decimal AliquotaDestino { get; set; }
        public decimal ValorISS { get; set; }
        public decimal ValorTotalComImposto { get; set; }
    }
}
