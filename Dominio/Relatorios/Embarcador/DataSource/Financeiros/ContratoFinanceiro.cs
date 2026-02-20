using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Financeiros
{
    public class ContratoFinanceiro
    {
        #region Propriedades

        public int Codigo { get; set; }
        private DateTime DataEmissao { get; set; }
        public int Numero { get; set; }
        public string NumeroDocumento { get; set; }
        public string Fornecedor { get; set; }
        public string Empresa { get; set; }
        public string Veiculo { get; set; }
        public string NumeroDocumentoEntrada { get; set; }
        public int QuantidadeParcela { get; set; }
        private SituacaoContratoFinanciamento Situacao { get; set; }
        public decimal ValorCapital { get; set; }
        public decimal ValorJuros { get; set; }
        public decimal ValorPagoCapital { get; set; }
        public decimal ValorPagoJuros { get; set; }
        public decimal ValorPagoParcela { get; set; }
        public decimal ValorAcrescimo { get; set; }
        public decimal ValorAcrescimoTitulo { get; set; }

        #endregion

        #region Propriedades com Regras 

        public string SituacaoFormatada
        {
            get { return Situacao.ObterDescricao(); }
        }
        public string DataEmissaoFormatada
        {
            get { return DataEmissao != DateTime.MinValue ? DataEmissao.ToString("dd/MM/yyyy") : string.Empty; }
        }

        #endregion
    }
}
