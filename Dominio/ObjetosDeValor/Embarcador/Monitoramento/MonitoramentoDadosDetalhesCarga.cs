using System;

namespace Dominio.ObjetosDeValor.Embarcador.Monitoramento
{
    public class MonitoramentoDadosDetalhesCarga
    {
        public int CodigoCarga { get; set; }
        public bool MonitoramentoCritico { get; set; }
        public DateTime DataPosicao { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus StatusMonitoramento { get; set; }
    }
}
