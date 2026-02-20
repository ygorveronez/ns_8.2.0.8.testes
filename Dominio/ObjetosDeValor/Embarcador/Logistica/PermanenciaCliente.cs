using System;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public class PermanenciaCliente
    {
        public int CodigoCargaEntrega { get; set; }

        public double CodigoCliente { get; set; }

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
