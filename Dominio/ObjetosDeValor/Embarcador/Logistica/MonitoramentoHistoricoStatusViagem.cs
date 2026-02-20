using System;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public class MonitoramentoHistoricoStatusViagem
    {
        public int CodigoMonitoramento { get; set; }

        public double CodigoCliente { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public DateTime Data { get; set; }

        public Enumeradores.MonitoramentoStatusViagemTipoRegra TipoRegra { get; set; }

        public DateTime DataInicio { get; set; }

        public DateTime? DataFim { get; set; }

        public int? TempoSegundos { get; set; }

        public TimeSpan Tempo
        {
            get
            {
                return TimeSpan.FromSeconds(TempoSegundos ?? 0);
            }
        }

    }
}
