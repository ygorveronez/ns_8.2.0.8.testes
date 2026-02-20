using System;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public class ControleViagem
    {
        public int MonitoramentoCodigo { get; set; }
        public int CargaCodigo { get; set; }
        public string CargaCodigoEmbarcador { get; set; }
        public DateTime DataCriacaoCarga { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga CargaSituacao { get; set; }
        public string Pedido { get; set; }
        public DateTime? DataCarregamentoPedido { get; set; }
        public DateTime? DataPrevisaoInicioViagem { get; set; }
        public DateTime? DataPrevisaoChegadaPlanta { get; set; }
        public int FilialCodigo { get; set; }
        public string FilialCNPJ { get; set; }
        public string FilialDescricao { get; set; }
        public int TransportadorCodigo { get; set; }
        public string TransportadorRazaoSocial { get; set; }
        public string TransportadorNomeFantasia { get; set; }
        public string VeiculoTracaoPlaca { get; set; }
        public string VeiculoReboquePlaca { get; set; }
        public string TecnologiaRastreadorDescricao { get; set; }
        public string Processo { get; set; }
        public string Operacao { get; set; }
        public DateTime DataPrevista { get; set; }
        public double ClienteDestinoCodigo { get; set; }
        public string ClienteDestinoNome { get; set; }
        public string ClienteDestinoLocalidade { get; set; }
        public string StatusViagem { get; set; }
        public string TempoTransito { get; set; }
        public string PosicaoDescricao { get; set; }
        public DateTime? PosicaoDataVeiculo { get; set; }
        public bool EmAlvo { get; set; }
        public string CodigosClientesAlvos { get; set; }
        public decimal? DistanciaPrevista { get; set; }
        public decimal? DistanciaRealizada { get; set; }
        public decimal? DistanciaAteOrigem { get; set; }
        public decimal? DistanciaAteDestino { get; set; }
    }
}
