using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Financeiro
{
    public class CTeTituloReceber
    {
        #region Propriedades

        public int Codigo { get; set; }
        public int Numero { get; set; }
        public int CodigoEmpresa { get; set; }
        public string Observacao { get; set; }
        public string Origem { get; set; }
        public string Destino { get; set; }
        public string Transportador { get; set; }
        public DateTime DataEmissao { get; set; }
        public DateTime DataVencimento { get; set; }
        public DateTime DataLiquidacao { get; set; }
        public decimal ValorAReceber { get; set; }
        public StatusTitulo StatusTitulo { get; set; }
        public int Serie { get; set; }
        public string TipoDocumento { get; set; }
        public int NumeroFatura { get; set; }
        public int NumeroPagamento { get; set; }
        public int QuantidadeParcelas { get; set; }
        public int SequenciaParcela { get; set; }
        public decimal ValorParcelaPaga { get; set; }
        public TipoDocumentoCreditoDebito ModeloDocumento { get; set; }
        public int SequenciaParcelaPaga { get; set; }
        public int CodigoTitulo { get; set; }

        #endregion

        #region Propriedades com Regras

        public string DescricaoStatusTitulo
        {
            get { return StatusTitulo.ObterDescricao(); }
        }

        public string DescricaoDataEmissao
        {
            get { return DataEmissao != DateTime.MinValue ? DataEmissao.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string DescricaoDataVencimento
        {
            get { return DataVencimento != DateTime.MinValue ? DataVencimento.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string DescricaoDataLiquidacao
        {
            get { return DataLiquidacao != DateTime.MinValue ? DataLiquidacao.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string DescricaoSequenciaParcela
        {
            get { return ModeloDocumento == TipoDocumentoCreditoDebito.Debito ? $"{SequenciaParcela}/{QuantidadeParcelas}" : string.Empty; }
        }

        public string DescricaoQuantidadeParcelas
        {
            get { return ModeloDocumento == TipoDocumentoCreditoDebito.Debito ? QuantidadeParcelas.ToString() : string.Empty; }
        }

        public string DescricaoValorParcelaPaga
        {
            get { return ModeloDocumento == TipoDocumentoCreditoDebito.Debito ? ValorParcelaPaga.ToString("n2") : string.Empty; }
        }

        public string DescricaoSequenciaParcelaPaga
        {
            get { return ModeloDocumento == TipoDocumentoCreditoDebito.Debito ? $"{SequenciaParcelaPaga}/{QuantidadeParcelas}" : string.Empty; }
        }

        public string DescricaoValorAReceber
        {
            get { return ModeloDocumento == TipoDocumentoCreditoDebito.Debito && ValorAReceber > 0 ? (-ValorAReceber).ToString("n2") : ValorAReceber.ToString("n2"); }
        }

        #endregion
    }
}
