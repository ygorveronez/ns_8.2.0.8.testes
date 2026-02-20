using System;

namespace Dominio.ObjetosDeValor.Embarcador.TorreControle.CardAcompanhamentoCarga
{
    public class Monitoramento
    {
        public int CodigoMonitoramento { get; set; }
        public double DistanciaPrevista { get; set; }
        public double DistanciaRealizada { get; set; }
        public double DistanciaAteDestino { get; set; }
        public double DistanciaAteOrigem { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus StatusMonitoramento { get; set; }
        public int TotalTemperaturasRecebidas { get; set; }
        public int TotalTemperaturasDentroFaixa { get; set; }
        public double TemperaturaMonitoramento { get; set; }
        public double PercentualViagem { get; set; }
        public string StatusViagem { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra TipoRegraViagem { get; set; }
        public int GrupoStatusViagemCodigo { get; set; }
        public string GrupoStatusViagemDescricao { get; set; }
        public string GrupoStatusViagemCor { get; set; }
        public DateTime DataUltimaPosicao { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public int Ignicao { get; set; }
        public double NivelGPS { get; set; }
        public double NivelBateria { get; set; }
        public bool MonitoramentoCritico { get; set; }
    }
}
