using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Enumeradores;

namespace Dominio.Relatorios.Embarcador.DataSource.CTe
{
    public class NFeCTeContainer
    {
        #region Propriedades

        public int Codigo { get; set; }
        public string ContainerDescricao { get; set; }
        public string NumeroBooking { get; set; }
        public string NumeroOS { get; set; }
        public string Viagem { get; set; }
        public string NavioTransbordo { get; set; }
        public string PortoOrigem { get; set; }
        public string TerminalOrigem { get; set; }
        public string PortoDestino { get; set; }
        public string TerminalDestino { get; set; }
        public string PortoTransbordo { get; set; }
        public string TerminalTransbordo { get; set; }
        public string TipoContainer { get; set; }

        public string Expedidor { get; set; }
        public string CNPJExpedidor { get; set; }
        public string Remetente { get; set; }
        public string CNPJRemetente { get; set; }
        public string Destinatario { get; set; }
        public string CNPJDestinatario { get; set; }
        public string Recebedor { get; set; }
        public string CNPJRecebedor { get; set; }
        public string Tomador { get; set; }
        public string CNPJTomador { get; set; }

        public string NumeroLacre { get; set; }
        public string Tara { get; set; }
        public string NumeroCTe { get; set; }
        public string NumeroControle { get; set; }
        public string StatusCTe { get; set; }
        public string TipoOperacao { get; set; }
        public string UFInicioPrestacao { get; set; }
        public string UFFimPrestacao { get; set; }
        public string PossuiCCe { get; set; }
        public string ProdutoPredominante { get; set; }
        private SituacaoCarga SituacaoCarga { get; set; }
        public string CargaIMO { get; set; }

        public int NumeroNota { get; set; }
        public string SerieNota { get; set; }
        private DateTime DataEmissaoNota { get; set; }
        public decimal ValorNota { get; set; }
        public decimal PesoNota { get; set; }
        public string ChaveNota { get; set; }

        public string ChaveCTeAquaviario { get; set; }
        public string ChaveCTeMultimodal { get; set; }
        public string ChaveCTeSVM { get; set; }
        public string PropostaComercial { get; set; }
        public string OOG { get; set; }
        public string Reefer { get; set; }
        public string NumeroReferenciaEDI { get; set; }
        public string ChaveCTE{ get; set; }
        public string NumeroPedidoEmbarcador { get; set; }
        public string NCM { get; set; }
        public string MasterBL { get; set; }
        public string Embarque { get; set; }
        public string NumeroDI { get; set; }
        public string NumeroControleCliente { get; set; }
        public string BookingReferenceFeeder { get; set; }
        public string AliquotaISS { get; set; }
        public string ValorISS { get; set; }
        public string ValorISSRetido { get; set; }

        #endregion

        #region Propriedades com Regras

        public string SituacaoCargaFormatada
        {
            get { return SituacaoCarga.ObterDescricao(); }
        }

        public string DataEmissaoNotaFormatada
        {
            get { return DataEmissaoNota != DateTime.MinValue ? DataEmissaoNota.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        #endregion
    }
}
