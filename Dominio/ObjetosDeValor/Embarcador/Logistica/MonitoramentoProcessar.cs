using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public class MonitoramentoProcessar
    {
        public int CodigoMonitoramento { get; set; }
        public DateTime DataCriacaoMonitoramento { get; set; }
        public int? CodigoVeiculo { get; set; }
        public DateTime? DataInicioMonitoramento { get; set; }
        public DateTime? DataFimMonitoramento { get; set; }
        public int CodigoCarga { get; set; }
        public DateTime? DataInicioViagem { get; set; }
        public DateTime? DataCarregamentoCarga { get; set; }
    }
}
