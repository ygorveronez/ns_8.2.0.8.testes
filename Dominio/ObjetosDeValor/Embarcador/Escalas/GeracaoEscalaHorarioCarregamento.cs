using System;

namespace Dominio.ObjetosDeValor.Embarcador.Escalas
{
    public sealed class GeracaoEscalaHorarioCarregamento
    {
        public int CodigoCentroCarregamento { get; set; }

        public TimeSpan InicioCarregamento { get; set; }

        public TimeSpan TerminoCarregamento { get; set; }
    }
}
