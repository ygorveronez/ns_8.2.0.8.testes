using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Financeiros
{
    public class ReciboFinanceiro
    {
        #region Propriedades

        public int Via { get; set; }
        public DateTime Data { get; set; }
        public decimal ValorTotal { get; set; }
        public string Observacao { get; set; }
        public string Documento { get; set; }
        public string TipoDocumento { get; set; }

        public int FornecedoresCount { get; set; }

        public string Pessoa { get; set; }
        public string CNPJPessoa { get; set; }
        public string TipoPessoa { get; set; }

        public string CNPJEmpresa { get; set; }
        public string TipoEmpresa { get; set; }
        public string NomeEmpresa { get; set; }
        public string CidadeEmpresa { get; set; }
        public string EstadoEmpresa { get; set; }

        public decimal ValorPago { get; set; }
        public int Parcela { get; set; }
        public decimal Acrescimo { get; set; }
        public decimal Desconto { get; set; }
        public string NumeroCartao { get; set; }
        public string TipoPagamentos { get; set; }
        public string ObservacaoUsuario { get; set; }

        #endregion

        #region Propriedades com Regras

        public string DataFormatada
        {
            get { return Data != DateTime.MinValue ? Data.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string CNPJEmpresaFormatado
        {
            get { return !string.IsNullOrEmpty(CNPJEmpresa) ? CNPJEmpresa.ObterCpfOuCnpjFormatado(TipoEmpresa) : string.Empty; }
        }

        public string CNPJPessoaFormatado
        {
            get { return !string.IsNullOrEmpty(CNPJPessoa) ? CNPJPessoa.ObterCpfOuCnpjFormatado(TipoPessoa) : string.Empty; }
        }

        public string LocalidadeEmpresaFormatada
        {
            get { return !string.IsNullOrEmpty(CidadeEmpresa) ? (CidadeEmpresa + " - " + EstadoEmpresa) : string.Empty; }
        }

        #endregion
    }
}
