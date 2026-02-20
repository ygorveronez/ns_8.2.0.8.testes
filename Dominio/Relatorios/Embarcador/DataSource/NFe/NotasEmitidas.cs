using System;

namespace Dominio.Relatorios.Embarcador.DataSource.NFe
{
    public class NotasEmitidas
    {
        #region Propriedades

        public int Codigo { get; set; }
        public int CodigoNFe { get; set; }
        public int CodigoNFSe { get; set; }
        public int TipoNota { get; set; }
        public int Numero { get; set; }
        public int Serie { get; set; }
        private DateTime DataEmissao { get; set; }
        private DateTime DataSaida { get; set; }

        private string CpfCnpjPessoa { get; set; }
        private string TipoPessoa { get; set; }
        public string Pessoa { get; set; }

        public string DescricaoStatus { get; set; }
        public string DescricaoNaturezaOperacao { get; set; }
        public string DescricaoFinalidade { get; set; }
        public string DescricaoTipoEmissao { get; set; }
        public string Chave { get; set; }
        public string DescricaoAtividade { get; set; }
        public decimal ValorTotal { get; set; }
        public decimal ValorTotalProdutos { get; set; }
        public decimal ValorTotalServicos { get; set; }
        public string DescricaoTipoFrete { get; set; }
        public decimal PesoBruto { get; set; }
        public decimal PesoLiquido { get; set; }
        public string Usuario { get; set; }
        public string Observacao { get; set; }
        public string ObservacaoTributaria { get; set; }
        public string CFOP { get; set; }

        #endregion

        #region Propriedades com Regras

        public string DataEmissaoFormatada
        {
            get { return DataEmissao != DateTime.MinValue ? DataEmissao.ToString("dd/MM/yyyy") : ""; }
        }

        public string DataSaidaFormatada
        {
            get { return DataSaida != DateTime.MinValue ? DataSaida.ToString("dd/MM/yyyy") : ""; }
        }

        public string CpfCnpjPessoaFormatado
        {
            get
            {
                return !string.IsNullOrWhiteSpace(TipoPessoa) ? TipoPessoa.Equals("F") ? string.Format(@"{0:000\.000\.000\-00}", long.Parse(CpfCnpjPessoa)) :
                  TipoPessoa.Equals("J") ? string.Format(@"{0:00\.000\.000\/0000\-00}", long.Parse(CpfCnpjPessoa)) : string.Empty : CpfCnpjPessoa.ObterCpfOuCnpjFormatado();
            }
        }

        #endregion
    }
}
