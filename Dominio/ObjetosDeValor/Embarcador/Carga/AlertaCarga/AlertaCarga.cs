using System;

namespace Dominio.ObjetosDeValor.Embarcador.Carga.AlertaCarga
{
    public class AlertaCarga
    {
        public int Codigo { get; set; }
        public int? CodigoCarga { get; set; }
        public int? CodigoVeiculo { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime Data { get; set; }
        public DateTime? DataFim { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.TipoAlertaCarga TipoAlerta { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus Status { get; set; }
    }
}
