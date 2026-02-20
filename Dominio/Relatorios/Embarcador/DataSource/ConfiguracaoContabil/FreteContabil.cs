using System;

namespace Dominio.Relatorios.Embarcador.DataSource.ConfiguracaoContabil
{
    public sealed class FreteContabil
    {
        public string CentroCusto { get; set; }

        private string _cnpjTransportador;
        public string CnpjTransportador
        {
            get { return _cnpjTransportador.ObterCnpjFormatado(); }
            set { _cnpjTransportador = value; }
        }

        public int Codigo { get; set; }

        public string ContaContabil { get; set; }

        public double CpfCnpjCompanhia { get; set ; }

        public string CpfCnpjCompanhiaFormatado
        {
            get { return CpfCnpjCompanhia > 0 ? CpfCnpjCompanhia.ToString().ObterCpfOuCnpjFormatado(TipoPessoaCompanhia) : ""; }
        }

        public double CpfCnpjEmitente { get; set; }

        public string CpfCnpjEmitenteFormatado
        {
            get { return CpfCnpjEmitente > 0 ? CpfCnpjEmitente.ToString().ObterCpfOuCnpjFormatado(TipoPessoaEmitente) : ""; }
        }

        public DateTime DataEmissao { get; set; }

        public string DataEmissaoFormatada {
            get { return DataEmissao == DateTime.MinValue ? "" : DataEmissao.ToString("dd/MM/yyyy");  }
        }

        public string Destino { get; set; }

        public string NomeCompanhia { get; set; }

        public string NomeEmitente { get; set; }

        public string NomeTransportador { get; set; }

        public string Origem { get; set; }

        public int Serie { get; set; }

        public string TipoContabilizacao { get; set; }

        public string TipoPessoaCompanhia { get; set; }

        public string TipoPessoaEmitente { get; set; }

        public decimal ValorLancamento { get; set; }
    }
}
