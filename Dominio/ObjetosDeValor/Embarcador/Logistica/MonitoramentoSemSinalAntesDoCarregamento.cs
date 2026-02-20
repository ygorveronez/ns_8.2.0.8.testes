using System;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public class MonitoramentoSemSinalAntesDoCarregamento
    {
        public int CodigoMonitoramento { get; set; }

        public int CodigoCarga { get; set; }
        public int CodigoVeiculo { get; set; }
        public DateTime? DataCarregamentoCarga { get; set; }
        public DateTime? DataInicio { get; set; }
    }
}
