using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Financeiros
{
    public class TituloSemMovimento
    {
        public int Codigo { get; set; }
        public double CNPJCPF { get; set; }
        public string TipoPessoa { get; set; }
        public string NomePessoa { get; set; }
        public string TipoTitulo { get; set; }
        public string StatusTitulo { get; set; }
        public DateTime DataEmissao { get; set; }
        public DateTime DataVencimento { get; set; }
        public decimal ValorDesonto { get; set; }
        public decimal ValorAcrescimo { get; set; }
        public string TipoDocumento { get; set; }
        public string NumeroDocumento { get; set; }
        public string Parcela { get; set; }
        public string Observacao { get; set; }
        public decimal ValorSaldo { get; set; }
        public decimal ValorTitulo { get; set; }
        public decimal ValorPendente { get; set; }
        public string TitulosAgrupados { get; set; }

        public string CPFCNPJPessoaFormatado
        {
            get
            {
                if (TipoPessoa == "E")
                    return "00.000.000/0000-00";
                else
                    return TipoPessoa == "J" ? string.Format(@"{0:00\.000\.000\/0000\-00}", CNPJCPF) : string.Format(@"{0:000\.000\.000\-00}", CNPJCPF);
            }
        }
    }
}
