namespace EmissaoCTe.Integracao
{
    public class RetornoImposto
    {
        public decimal ValorFrete { get; set; }
        public string CSTICMS { get; set; }
        public decimal BaseCalculoICMS { get; set; }
        public decimal AliquotaICMS { get; set; }        
        public decimal ValorICMS { get; set; }
        public int CFOP { get; set; }
        public int IBGEMunicipioGerador { get; set; }
        public string Mensagem { get; set; }
    }
}