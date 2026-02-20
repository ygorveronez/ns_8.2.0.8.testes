using System;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public class HorarioCarregamento
    {
        public virtual Dominio.Entidades.Cliente Cliente { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana Dia { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraTermino { get; set; }

    }
}
