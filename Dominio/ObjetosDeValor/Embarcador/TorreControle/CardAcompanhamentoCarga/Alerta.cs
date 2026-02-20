using System;

namespace Dominio.ObjetosDeValor.Embarcador.TorreControle.CardAcompanhamentoCarga
{
    public class Dados
    {
        public Alerta Alerta { get; set; }
    }

    public class Alerta
    {
        public DateTime DataAlerta { get; set; }
        public DateTime DataCadastro { get; set; }
        public bool AlertaTratado { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.TorreControle.CardAcompanhamentoCarga.AlertaCarga AlertaCarga { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.TorreControle.CardAcompanhamentoCarga.AlertaMonitoramento AlertaMonitoramento { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.TorreControle.CardAcompanhamentoCarga.Tratativa Tratativa { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.TorreControle.CardAcompanhamentoCarga.MonitoramentoEvento MonitoramentoEvento { get; set; }
    }

}
