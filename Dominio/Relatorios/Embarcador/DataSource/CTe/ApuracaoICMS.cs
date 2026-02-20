using System;
using Dominio.Enumeradores;

namespace Dominio.Relatorios.Embarcador.DataSource.CTe
{
    public class ApuracaoICMS
    {
        #region Propriedades

        public int Codigo { get; set; }

        public string CPFCNPJRemetente { get; set; }
        private TipoPessoa TipoRemetente { get; set; }
        public string IERemetente { get; set; }
        public string Remetente { get; set; }

        public string CPFCNPJDestinatario { get; set; }
        private TipoPessoa TipoDestinatario { get; set; }
        public string IEDestinatario { get; set; }
        public string Destinatario { get; set; }

        public string CPFCNPJTomador { get; set; }
        private TipoPessoa TipoTomador { get; set; }
        public string IETomador { get; set; }
        public string Tomador { get; set; }

        public string CPFCNPJExpedidor { get; set; }
        private TipoPessoa TipoExpedidor { get; set; }
        public string IEExpedidor { get; set; }
        public string Expedidor { get; set; }

        public string CPFCNPJRecebedor { get; set; }
        private TipoPessoa TipoRecebedor { get; set; }
        public string IERecebedor { get; set; }
        public string Recebedor { get; set; }

        public DateTime DataEmissao { get; set; }
        public decimal AliquotaICMSInterna { get; set; }
        public decimal PercentualICMSPartilha { get; set; }
        public decimal ValorICMSUFOrigem { get; set; }
        public decimal ValorICMSUFDestino { get; set; }
        public decimal ValorICMSFCPFim { get; set; }
        public string CaracteristicaTransporteCTe { get; set; }
        public string TipoProposta { get; set; }
        public int NumeroCTe { get; set; }
        public string NumeroControle { get; set; }

        #endregion

        #region Propriedades com Regras

        public string DataEmissaoFormatada
        {
            get { return DataEmissao != DateTime.MinValue ? DataEmissao.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public virtual string CPFCNPJRemetenteFormatado
        {
            get
            {
                return !string.IsNullOrWhiteSpace(CPFCNPJRemetente) ? TipoRemetente == TipoPessoa.Juridica ? CPFCNPJRemetente.ObterCnpjFormatado() : CPFCNPJRemetente.ObterCpfFormatado() : string.Empty;
            }
        }

        public virtual string CPFCNPJDestinatarioFormatado
        {
            get
            {
                return !string.IsNullOrWhiteSpace(CPFCNPJDestinatario) ? TipoDestinatario == TipoPessoa.Juridica ? CPFCNPJDestinatario.ObterCnpjFormatado() : CPFCNPJDestinatario.ObterCpfFormatado() : string.Empty;
            }
        }

        public virtual string CPFCNPJTomadorFormatado
        {
            get
            {
                return !string.IsNullOrWhiteSpace(CPFCNPJTomador) ? TipoTomador == TipoPessoa.Juridica ? CPFCNPJTomador.ObterCnpjFormatado() : CPFCNPJTomador.ObterCpfFormatado() : string.Empty;
            }
        }

        public virtual string CPFCNPJExpedidorFormatado
        {
            get
            {
                return !string.IsNullOrWhiteSpace(CPFCNPJExpedidor) ? TipoExpedidor == TipoPessoa.Juridica ? CPFCNPJExpedidor.ObterCnpjFormatado() : CPFCNPJExpedidor.ObterCpfFormatado() : string.Empty;
            }
        }

        public virtual string CPFCNPJRecebedorFormatado
        {
            get
            {
                return !string.IsNullOrWhiteSpace(CPFCNPJRecebedor) ? TipoRecebedor == TipoPessoa.Juridica ? CPFCNPJRecebedor.ObterCnpjFormatado() : CPFCNPJRecebedor.ObterCpfFormatado() : string.Empty;
            }
        }

        #endregion
    }
}
