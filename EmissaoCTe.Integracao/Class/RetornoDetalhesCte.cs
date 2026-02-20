namespace EmissaoCTe.Integracao
{
    public class RetornoDetalhesCTe
    {
        public int NumeroCTe { get; set; }
        public int SerieCTe { get; set; }
        public string ChaveCTe { get; set; }
        public string ChaveMDFe { get; set; }
        public string DataEmissao { get; set; }
        public string DataAutorizacao { get; set; }
        public decimal PesoCTe { get; set; }
        public decimal ValorFrete { get; set; }
        public decimal AliquotaICMS { get; set; }
        public decimal ValorICMS { get; set; }
        public decimal ValorPedagioMDFe { get; set; }
        public string SituacaoCTe { get; set; }
    }
}