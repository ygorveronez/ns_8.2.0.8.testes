using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.CTe
{
    public class TakeOrPay
    {
        #region Propriedades

        public int Codigo { get; set; }
        public string NumeroCarga { get; set; }
        public int NumeroFatura { get; set; }
        private DateTime DataFatura { get; set; }
        private DateTime DataVencimentoBoleto { get; set; }
        public decimal ValorFrete { get; set; }
        public string NumeroBoleto { get; set; }

        public string CPFCNPJTomador { get; set; }
        public string Tomador { get; set; }
        public string GrupoTomador { get; set; }
        public string CodigoDocumentoTomador { get; set; }

        public string CodigoNavio { get; set; }
        public string CodigoPortoOrigem { get; set; }
        public string PortoOrigem { get; set; }
        public string CodigoPortoDestino { get; set; }
        public string PortoDestino { get; set; }

        private string EmailsFaturaIntegracao { get; set; }
        public string Viagem { get; set; }
        public string ETS { get; set; }
        public TipoPropostaMultimodal TipoProposta { get; set; }
        public int QtdDisponibilizada { get; set; }
        public int QtdNaoEmbarcadas { get; set; }
        public int DiasPrazoFaturamento { get; set; }

        public string CNPJTransportador { get; set; }
        public string Transportador { get; set; }

        public string SituacaoFatura { get; set; }
        public string ObservacaoFatura { get; set; }

        public int UnidadesContabilidade { get; set; }
        public decimal AliquotaICMSContabilidade { get; set; }
        public decimal ValorICMSContabilidade { get; set; }
        public decimal PTAXContabilidade { get; set; }
        public decimal ValorUSDContabilidade { get; set; }
        public string PONumberContabilidade { get; set; }

        #endregion

        #region Propriedades com Regras

        public string DescricaoTipoProposta
        {
            get { return TipoProposta.ObterDescricao(); }
        }

        public string DataFaturaFormatada
        {
            get { return DataFatura != DateTime.MinValue ? DataFatura.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string DataVencimentoBoletoFormatada
        {
            get { return DataVencimentoBoleto != DateTime.MinValue ? DataVencimentoBoleto.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string EmailFatura
        {
            get { return string.IsNullOrWhiteSpace(NumeroBoleto) ? EmailsFaturaIntegracao : string.Empty; }
        }

        #endregion
    }
}
