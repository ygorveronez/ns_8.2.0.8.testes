using System;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public class ParametrosPerdaSinalMonitoramento
    {
        public int codigoMonitoramento { get; set; }
        public int CodigoVeiculo { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public Dominio.Entidades.Embarcador.Logistica.AlertaMonitor AlertaMonitor { get; set; }
    }
}
