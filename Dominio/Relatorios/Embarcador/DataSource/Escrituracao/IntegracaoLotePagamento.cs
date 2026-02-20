using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Relatorios.Embarcador.DataSource.Escrituracao
{
    public class IntegracaoLotePagamento
    {
        #region Propriedades

        public long Codigo { get; set; }
        public int NumeroDocumento { get; set; }
        public int SerieDocumento { get; set; }
        public string Chave { get; set; }
        public DateTime DataEmissao { get; set; }
        public string TipoDocumento { get; set; }
        public string Emissor { get; set; }
        public decimal ValorFrete { get; set; }
        public int NumeroPagamento { get; set; }
        private SituacaoPagamento SituacaoPagamento { get; set; }
        private SituacaoIntegracao SituacaoIntegracao { get; set; }
        public string RetornoIntegracao { get; set; }
        public string Filial { get; set; }
        public string TipoOperacao { get; set; }
        public string Carga { get; set; }
        public string NotaFiscal { get; set; }
        public string NumeroCancelamento { get; set; }
        public string SituacaoCancelamento { get; set; }
        public string MotivoCancelamento { get; set; }
        public int ProtocoloCTe { get; set; }
        private DateTime DataEnvioIntegracao { get; set; }
        private SituacaoCarga SituacaoCarga { get; set; }

        #endregion

        #region Propriedades com Regras

        public string SituacaoPagamentoFormatada
        {
            get { return SituacaoPagamento.ObterDescricao(); }
        }

        public string SituacaoIntegracaoFormatada
        {
            get { return SituacaoIntegracao.ObterDescricao(); }
        }

        public string DataEnvioIntegracaoFormatada
        {
            get
            {
                return DataEnvioIntegracao != DateTime.MinValue ? DataEnvioIntegracao.ToString("dd/MM/yyyy HH:mm") : "";
            }
        }

        public string SituacaoCargaFormatada
        {
            get { return SituacaoCarga.ObterDescricao(); }
        }

        #endregion
    }
}
