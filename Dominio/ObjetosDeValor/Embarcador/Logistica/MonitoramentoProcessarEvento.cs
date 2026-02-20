using System;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public class MonitoramentoProcessarEvento
    {
        public int CodigoProcessarEvento { get; set; }
        public DateTime DataProcessarEvento { get; set; }
        public long? CodigoPosicao { get; set; }
        public int? CodigoVeiculo { get; set; }
        public DateTime? DataVeiculoPosicao { get; set; }
        public double? LatitudePosicao { get; set; }
        public double? LongitudePosicao { get; set; }
        public int? IgnicaoPosicao { get; set; }
        public int? VelocidadePosicao { get; set; }
        public decimal? TemperaturaPosicao { get; set; }
        public bool? SensorTemperaturaPosicao { get; set; }
        public bool? EmAlvoPosicao { get; set; }
        public string CodigosClientesAlvoPosicao { get; set; }
        public int? CodigoMonitoramento { get; set; }
        public DateTime? DataCriacaoMonitoramento { get; set; }
        public DateTime? DataInicioMonitoramento { get; set; }
        public DateTime? DataFimMonitoramento { get; set; }
        public int? CodigoCarga { get; set; }
        public DateTime? DataInicioViagem { get; set; }
        public DateTime? DataCarregamentoCarga { get; set; }
        public DateTime? DataAgendamentoEntrega { get; set; }
    }
}
