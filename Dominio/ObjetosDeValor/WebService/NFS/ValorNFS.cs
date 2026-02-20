
namespace Dominio.ObjetosDeValor.WebService.NFS
{
    public class ValorNFS
    {
        public decimal ValorServicos { get; set; }
        public decimal ValorDeducoes { get; set; }
        public decimal ValorPIS { get; set; }
        public decimal ValorCOFINS { get; set; }
        public decimal ValorINSS { get; set; }
        public decimal ValorIR { get; set; }
        public decimal ValorCSLL { get; set; }
        public bool ISSRetido { get; set; }
        public decimal ValorISSRetido { get; set; }
        public decimal ValorOutrasRetencoes { get; set; }
        public decimal ValorDescontoIncondicionado { get; set; }
        public decimal ValorDescontoCondicionado { get; set; }
        public decimal AliquotaISS { get; set; }
        public decimal BaseCalculoISS { get; set; }
        public decimal ValorISS { get; set; }
        public decimal ValorServicosSemImpostoIncluso { get; set; }
        public Dominio.ObjetosDeValor.CTe.IBSCBS IBSCBS { get; set; }
    }
}
