using System;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public class MonitoramentoTrocaDeAlvo
    {

        public int CodigoTrocaAlvo { get; set; }
        public DateTime DataTrocaAlvo { get; set; }
        public int CodigoMonitoramento { get; set; }
        public DateTime? DataCriacaoMonitoramento { get; set; }
        public DateTime? DataInicioMonitoramento { get; set; }
        public DateTime? DataFimMonitoramento { get; set; }
        public int CodigoCarga { get; set; }
        public int CodigoVeiculo { get; set; }
        public int CodigoTipoOperacao { get; set; }
        public bool RealizarBaixaEntradaNoRaio { get; set; }
        public bool NaoProcessarTrocaAlvoViaMonitoramentoTipoOperacao { get; set; }
        public long CodigoPosicaoAtual { get; set; }
        public DateTime? DataVeiculoPosicaoAtual { get; set; }
        public double? LatitudePosicaoAtual { get; set; }
        public double? LongitudePosicaoAtual { get; set; }
        public bool? EmAlvoPosicaoAtual { get; set; }
        public bool? EmLocalPosicaoAtual { get; set; }
        public string CodigosClientesAlvoPosicaoAtual { get; set; }
        public string CodigosLocaisPosicaoAtual { get; set; }
        public string CodigosSubareasAlvoPosicaoAtual { get; set; }
        public string CodigosLocalPosicaoAtual { get; set; }
        public long CodigoPosicaoAnterior { get; set; }
        public DateTime? DataVeiculoPosicaoAnterior { get; set; }
        public double? LatitudePosicaoAnterior { get; set; }
        public double? LongitudePosicaoAnterior { get; set; }
        public bool? EmAlvoPosicaoAnterior { get; set; }
        public bool? EmLocalPosicaoAnterior { get; set; }
        public string CodigosClientesAlvoPosicaoAnterior { get; set; }
        public string CodigosSubareasAlvoPosicaoAnterior { get; set; }
        public string CodigosLocaisPosicaoAnterior { get; set; }
        public string CodigosClientesEntregas { get; set; }
    }
}
