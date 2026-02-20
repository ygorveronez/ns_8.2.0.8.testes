using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Pessoas
{
    public class FuncionarioComissaoTitulo
    {
        public int Codigo { get; set; }
        public int CodigoFuncionarioComissao { get; set; }
        public int CodigoTitulo { get; set; }
        public string Pessoa { get; set; }
        public double CNPJCPFPessoa { get; set; }
        public string TipoPessoa { get; set; }
        public int NumeroFatura { get; set; }
        public DateTime DataEmissao { get; set; }
        public DateTime DataVencimento { get; set; }
        public DateTime DataLiquidacao { get; set; }
        public decimal ValorOriginal { get; set; }
        public decimal ValorPago { get; set; }
        public decimal ValorISS { get; set; }
        public decimal ValorICMS { get; set; }
        public decimal ValorLiquido { get; set; }
        public decimal PercentualImpostoFederal { get; set; }
        public decimal ValorFinal { get; set; }

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
