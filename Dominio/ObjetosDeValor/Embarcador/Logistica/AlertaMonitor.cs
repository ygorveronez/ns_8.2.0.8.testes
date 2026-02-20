using System;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public class AlertaMonitor
    {
        public int Codigo { get; set; }
        public int? CodigoCarga { get; set; }
        public int? CodigoVeiculo { get; set; }
        public int CodigoMonitoramentoEvento { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime Data { get; set; }
        public DateTime? DataFim { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta TipoAlerta { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus Status { get; set; }
        public bool AlertaReprogramado { get; set; }
        public int TempoReprogramado { get; set; }
        public long rn { get; set; }
    }
}
