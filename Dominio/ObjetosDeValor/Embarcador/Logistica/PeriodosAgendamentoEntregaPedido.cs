using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public sealed class PeriodosAgendamentoEntregaPedido
    {
        public DiaSemana Dia { get; set; }
        public List<PeriodoAgendamentoEntregaHorario> Periodos { get; set; }
    }
    
    public sealed class PeriodoAgendamentoEntregaHorario
    {
        public TimeSpan Inicio { get; set; }
        public TimeSpan Fim { get; set; }
    }
}
