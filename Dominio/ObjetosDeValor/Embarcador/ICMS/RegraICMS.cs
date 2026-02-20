namespace Dominio.ObjetosDeValor.Embarcador.ICMS
{
    public class RegraICMS
    {
        public RegraICMS()
        {
            ObservacaoCTe = "";
            SimplesNacional = false;
            Aliquota = 0;
            AliquotaInternaDifal = 0;
            AliquotaSimples = 0;
            ValorICMS = 0;
            ValorICMSIncluso = 0;
            ValorBaseCalculoICMS = 0;
            ValorPis = 0;
            ValorCofins = 0;
            AliquotaPis = 0;
            AliquotaCofins = 0;
            PercentualReducaoBC = 0;
            PercentualInclusaoBC = 0;
            IncluirICMSBC = false;
            DescontarICMSDoValorAReceber = false;
            NaoImprimirImpostosDACTE = false;
            NaoEnviarImpostoICMSNaEmissaoCte = false;
            NaoIncluirPisCofinsBCEmComplementos = false;
            CST = "";
            CFOP = 0;
            CodigoRegra = 0;
            Descricao = "";
            ValorBaseCalculoPISCOFINS = 0;

        }

        public int CodigoRegra { get; set; }
        public int CFOP { get; set; }
        public decimal Aliquota { get; set; }
        public decimal AliquotaInternaDifal { get; set; }
        public decimal AliquotaSimples { get; set; }
        public decimal ValorICMS { get; set; }
        public decimal ValorCofins { get; set; }
        public decimal ValorPis { get; set; }
        public decimal AliquotaPis { get; set; }
        public decimal AliquotaCofins { get; set; }
        public decimal ValorICMSIncluso { get; set; }
        public decimal ValorCreditoPresumido { get; set; }
        public decimal ValorBaseCalculoICMS { get; set; }
        public decimal PercentualReducaoBC { get; set; }
        public bool IncluirICMSBC { get; set; }
        public bool IncluiICMSBaseCalculo { get; set; }
        public bool IncluirPisCofinsBC { get; set; }
        public bool NaoIncluirPisCofinsBCEmComplementos { get; set; }
        public decimal PercentualInclusaoBC { get; set; }
        public decimal PercentualICMSIncluirNoFrete { get; set; }
        public decimal PercentualCreditoPresumido { get; set; }
        public string CST { get; set; }
        public string ObservacaoCTe { get; set; }
        public bool SimplesNacional { get; set; }
        public bool DescontarICMSDoValorAReceber { get; set; }
        public bool NaoImprimirImpostosDACTE { get; set; }
        public bool NaoEnviarImpostoICMSNaEmissaoCte { get; set; }
        public Dominio.Entidades.CFOP ObjetoCFOP { get; set; }
        public bool NaoReduzirRetencaoICMSDoValorDaPrestacao { get; set; }
        public string Descricao { get; set; }
        public decimal ValorBaseCalculoPISCOFINS { get; set; }
    }
}
