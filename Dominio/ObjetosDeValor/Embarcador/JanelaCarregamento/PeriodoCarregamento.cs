using System;

namespace Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento
{
    public sealed class PeriodoCarregamento
    {
        public int Index { get; set; }

        public int Periodo { get; set; }

        public int CapacidadeCarregamentoSimultaneo { get; set; }

        public TimeSpan HoraInicio { get; set; }

        public TimeSpan HoraTermino { get; set; }
    }
}
