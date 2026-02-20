using System;
using CsvHelper.Configuration.Attributes;

namespace Dominio.Relatorios.Embarcador.DataSource.Financeiros
{
    public class ResumoContasAPagar
    {
        [Name("Sumarização")]
        [Index(0)]
        public string TipoSumarizacao { get; set; }

        [Name("Tipo Doc.")]
        [Index(1)]
        public string TipoDocumento { get; set; }

        [Name("Grupo de Pessoas")]
        [Index(2)]
        public string GrupoPessoasFornecedor { get; set; }

        [Name("Valor")]
        [Index(3)]
        public decimal Valor { get; set; }

        [Name("Documento")]
        [Index(4)]
        public string NumeroDocumento { get; set; }

        [Name("Título")]
        [Index(5)]
        public int NumeroTitulo { get; set; }

        [Name("Data de Emissão")]
        [Index(6)]
        public DateTime DataEmissao { get; set; }

        [Name("Data de Vencimento")]
        [Index(7)]
        public DateTime DataVencimento { get; set; }

        [Name("CPF/CNPJ do Fornecedor")]
        [Index(8)]
        public string CPFCNPJFornecedorFormatado
        {
            get
            {
                if (TipoPessoaFornecedor == "J")
                    return String.Format(@"{0:00\.000\.000\/0000\-00}", this.CPFCNPJFornecedor);
                else
                    return String.Format(@"{0:000\.000\.000\-00}", this.CPFCNPJFornecedor);
            }
        }

        [Name("Nome do Fornecedor")]
        [Index(9)]
        public string NomeFornecedor { get; set; }

        [Name("Tipo de Documento")]
        [Index(10)]
        public string TipoDocumentoTituloOrigem { get; set; }

        [Ignore]
        public string TipoPessoaFornecedor { get; set; }
        [Ignore]
        public double CPFCNPJFornecedor { get; set; }

        [Ignore]
        public decimal Saldo { get; set; }
    }
}
