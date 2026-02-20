using System;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public class MonitoramentoVeiculo
    {
        public int Codigo { get; set; }
        public int CodigoMonitoramento { get; set; }
        public int CodigoVeiculo { get; set; }
        public DateTime DataInicio { get; set; }
        public string DataInicioFormatada { get { return DataInicio.ToString("dd/MM/yyyy"); } }
        public DateTime? DataFim { get; set; }
        public string DataFimFormatada { get { return DataFim?.ToString("dd/MM/yyyy") ?? ""; } }
        public string Polilinha { get; set; }
        public decimal? Distancia { get; set; }
        public int Carga { get; set; }
    }
}
