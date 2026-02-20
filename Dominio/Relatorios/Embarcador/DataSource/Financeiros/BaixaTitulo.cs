using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Financeiros
{
    public class BaixaTitulo
    {
        public int Codigo { get; set; }
        public DateTime DataBaixa { get; set; }
        public string Observacao { get; set; }
        public string NumeroDocumentoOriginal { get; set; }
        public string TipoDocumentoOriginal { get; set; }
        public double CNPJCPFPessoa { get; set; }
        public string RazaoPessoa { get; set; }
        public string TipoPessoa { get; set; }
        public decimal ValorOriginal { get; set; }
        public decimal ValorBaixa { get; set; }
        public decimal ValorAcrescimoBaixa { get; set; }
        public decimal ValorDescontoBaixa { get; set; }
        public string PlanoCredito { get; set; }
        public string DescricaoPlanoCredito { get; set; }
        public string PlanoDebito { get; set; }
        public string DescricaoPlanoDebito { get; set; }
        public string PlanoConta { get; set; }
        public string DescricaoPlanoConta { get; set; }

        public string CPFCNPJPessoaFormatado
        {
            get
            {
                if (TipoPessoa == "E")
                    return "00.000.000/0000-00";
                else
                    return TipoPessoa == "J" ? string.Format(@"{0:00\.000\.000\/0000\-00}", CNPJCPFPessoa) : string.Format(@"{0:000\.000\.000\-00}", CNPJCPFPessoa);
            }
        }
    }
}
