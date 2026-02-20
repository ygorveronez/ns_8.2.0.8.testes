using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Cargas.Quantidade
{
    public class Quantidade
    {
        #region Propriedades

        public int Codigo { get; set; }
        public string Operador { get; set; }
        public string NumeroCarga { get; set; }
        public int NumeroEntregas { get; set; }
        public int DiasAtrazo { get; set; }
        public DateTime DataCarregamento { get; set; }
        public string Observacao { get; set; }
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
        public string DataInicioCarregamento { get; set; }
        public string DataPrevisaoChegadaOrigem { get; set; }
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
        public SituacaoCargaJanelaCarregamento SituacaoCargaJanelaCarregamento { get; set; }
        public string StatusGr { get; set; }
        private TipoProbe TipoProbe { get; set; }
        public string ViaTransporte { get; set; }
        public string Expedidor { get; set; }
        public string CnpjTransportador { get; set; }
        public decimal PesoCarga { get; set; }
        public string ObservacaoCarregamento { get; set; }
        public string CidadeExpedidor { get; set; }
        public string CidadeRemetente { get; set; }
        public EnumSituacaoCargaLicenca Licenca { get; set; }
        public string OrdemEmbarque { get; set; }
        public SituacaoIntegracao SituacaoOrdemEmbarque { get; set; }
        public bool Rastreador { get; set; }
        public string Filial { get; set; }
        public string CNPJFilial { get; set; }
        public string PaisDestino { get; set; }
        private DateTime ETANavio { get; set; }
        private DateTime ETSNavio { get; set; }
        private PagamentoMaritimo TipoTomador { get; set; }
        public string Temperatura { get; set; }
        public string CodigoInland { get; set; }
        public string PedidoOriginal { get; set; }
        public string TipoOperacao { get; set; }
        public bool ForaPeriodo { get; set; }
        private DateTime DataUltimaPosicao { get; set; }
        private bool Divisoria { get; set; }
        private bool CargaPerigosa { get; set; }
        public decimal CustoPlanejado { get; set; }
        public decimal CustoAtual { get; set; }
        public string RazaoLeilao { get; set; }
        public string NumeroContrato { get; set; }
        public DateTime DataInicialContrato { get; set; }
        public DateTime DataFinalContrato { get; set; }
        public string TipoContratoFreteDescricao { get; set; }

        #endregion

        #region Propriedades Calculadas

        public string ETANavioFormatada
        {
            get { return ETANavio != DateTime.MinValue ? ETANavio.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string ETSNavioFormatada
        {
            get { return ETSNavio != DateTime.MinValue ? ETSNavio.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string TipoTomadorFormatada
        {
            get { return TipoTomador.ObterDescricao() ?? ""; }
        }

        public string LicencaDescricao
        {
            get { return this.Licenca.ObterDescricao() ?? ""; }
        }

        public string SituacaoCargaJanelaCarregamentoDescricao
        {
            get { return SituacaoCargaJanelaCarregamento.ObterDescricao() ?? ""; }
        }

        public string SituacaoOrdemEmbarqueDescricao
        {
            get { return SituacaoOrdemEmbarque.ObterDescricao() ?? ""; }
        }

        public string TipoProbeDescricao
        {
            get { return TipoProbe.ObterDescricao(); }
        }

        public string EXPAno
        {
            get 
            {
                string[] str = string.IsNullOrWhiteSpace(NumeroEXP) ? new string[0] : NumeroEXP.Split('-');
                return str.Length > 0 ? str[0].Replace("/", "") : "";
            }
        }

        public string SubEXP
        {
            get
            {
                string[] str = string.IsNullOrWhiteSpace(NumeroEXP) ? new string[0] : NumeroEXP.Split('-');
                return str.Length > 1 ? str[1] : "";
            }
        }

        public string ForaPeriodoDescricao
        {
            get { return ForaPeriodo ? "Sim" : "Não"; }
        }

        public string DataUltimaPosicaoFormatada
        {
            get { return DataUltimaPosicao != DateTime.MinValue ? DataUltimaPosicao.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty; }
        }

        public string DivisoriaDescricao
        {
            get { return Divisoria ? "Sim" : "Não"; }
        }

        public string CargaPerigosaDescricao
        {
            get { return CargaPerigosa ? "Sim" : "Não"; }
        }

        public string DataInicialContratoFormatada
        {
            get { return DataInicialContrato != DateTime.MinValue ? DataInicialContrato.ToDateTimeString() : "" ;  }
        }

        public string DataFinalContratoFormatada
        {
            get { return DataFinalContrato != DateTime.MinValue ? DataFinalContrato.ToDateTimeString() : ""; ; }
        }

        #endregion
    }
}
