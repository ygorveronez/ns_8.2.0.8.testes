using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Cargas.QuantidadeDescarga
{
    public class QuantidadeDescarga
    {
        #region Propriedades

        public int Codigo { get; set; }
        public string Operador { get; set; }
        public string NumeroCarga { get; set; }
        public int NumeroEntregas { get; set; }
        public int DiasAtrazo { get; set; }
        public DateTime DataDescarregamento { get; set; }
        public DateTime DataDescarregamentoProgramada { get; set; }
        public string ModeloVeiculo { get; set; }
        public string TipoCarga { get; set; }
        public string Transportador { get; set; }
        public decimal ValorFrete { get; set; }
        public string Destino { get; set; }
        public string Rota { get; set; }
        public string Destinatario { get; set; }
        public string Veiculos { get; set; }
        public string ClienteAdicional { get; set; }
        public string ClienteDonoContainer { get; set; }
        public string DataDeadLCargaNavioViagem { get; set; }
        public string DataDeadLineNavioViagem { get; set; }
        public string InicioDescarregamento { get; set; }
        public string CentroDescarregamentoDescricao { get; set; }
        public DateTime DataPrevisaoDescarregamento { get; set; }
        public DateTime DataPrevisaoChegadaOrigem { get; set; }
        public string Despachante { get; set; }
        public string NavioViagem { get; set; }
        public string NumeroBooking { get; set; }
        public string NumeroEXP { get; set; }
        public string NumeroPedidoEmbarcador { get; set; }
        public string NumeroPedidoProvisorio { get; set; }
        public string PortoViagemDestino { get; set; }
        public string PortoViagemOrigem { get; set; }
        public string PossuiGenset { get; set; }
        public string Remetente { get; set; }
        public int TempoDescarregamento { get; set; }
        public SituacaoCargaJanelaDescarregamento SituacaoCargaJanelaDescarregamento { get; set; }
        public string StatusGr { get; set; }
        private TipoProbe TipoProbe { get; set; }
        public string ViaTransporte { get; set; }
        public string Expedidor { get; set; }
        public string CnpjTransportador { get; set; }
        public decimal PesoCarga { get; set; }
        public string CidadeExpedidor { get; set; }
        public string CidadeRemetente { get; set; }
        public EnumSituacaoCargaLicenca Licenca { get; set; }
        public string OrdemEmbarque { get; set; }
        public SituacaoIntegracao SituacaoOrdemEmbarque { get; set; }
        public bool Rastreador { get; set; }
        public string Filial { get; set; }
        public string CNPJFilial { get; set; }
        public bool ForaPeriodo { get; set; }
        private DateTime DataCarregamentoProgramada { get; set; }
        private DateTime DataPrevisaoEntregaPedido { get; set; }
        private DateTime DataEntregaPlanejadaProximaEntrega { get; set; }
        private DateTime DataEntradaRaio { get; set; }
        public string StatusViagem { get; set; }
        public decimal KmAteDestino { get; set; }
        public string NotasFiscais { get; set; }
        private DateTime DataUltimaPosicao { get; set; }
        public string SetPointTransp { get; set; }
        public string RangeTempTransp { get; set; }
        public string TipoCargaTaura { get; set; }
        private DateTime ChegadaPlanejada { get; set; }

        #endregion

        #region Propriedades Calculadas

        public string LicencaDescricao
        {
            get { return this.Licenca.ObterDescricao() ?? ""; }
        }

        public string SituacaoCargaJanelaDescarregamentoDescricao
        {
            get { return SituacaoCargaJanelaDescarregamento.ObterDescricao() ?? ""; }
        }

        public string SituacaoOrdemEmbarqueDescricao
        {
            get { return SituacaoOrdemEmbarque.ObterDescricao() ?? ""; }
        }

        public string TipoProbeDescricao
        {
            get { return TipoProbe.ObterDescricao(); }
        }

        public string DataDescarregamentoFormatada
        {
            get { return DataDescarregamento != DateTime.MinValue ? DataDescarregamento.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataDescarregamentoProgramadaFormatada
        {
            get { return DataDescarregamentoProgramada != DateTime.MinValue ? DataDescarregamentoProgramada.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataPrevisaoDescarregamentoFormatada
        {
            get { return DataPrevisaoDescarregamento != DateTime.MinValue ? DataPrevisaoDescarregamento.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataPrevisaoChegadaOrigemFormatada
        {
            get { return DataPrevisaoChegadaOrigem != DateTime.MinValue ? DataPrevisaoChegadaOrigem.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string ForaPeriodoDescricao
        {
            get { return ForaPeriodo ? "Sim" : "Não"; }
        }

        public string DataCarregamentoProgramadaFormatada 
        {
            get { return DataCarregamentoProgramada != DateTime.MinValue ? DataCarregamentoProgramada.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataPrevisaoEntregaPedidoFormatada
        {
            get { return DataPrevisaoEntregaPedido != DateTime.MinValue ? DataPrevisaoEntregaPedido.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataEntregaPlanejadaProximaEntregaFormatada
        {
            get { return DataEntregaPlanejadaProximaEntrega != DateTime.MinValue ? DataEntregaPlanejadaProximaEntrega.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataEntradaRaioFormatada
        {
            get { return DataEntradaRaio != DateTime.MinValue ? DataEntradaRaio.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }
        
        public string DataUltimaPosicaoFormatada
        {
            get { return DataUltimaPosicao != DateTime.MinValue ? DataUltimaPosicao.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string ChegadaPlanejadaFormatada
        {
            get { return ChegadaPlanejada != DateTime.MinValue ? ChegadaPlanejada.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        #endregion
    }
}
