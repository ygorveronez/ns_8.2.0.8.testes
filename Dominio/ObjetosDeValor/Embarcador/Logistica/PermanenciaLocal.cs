using System;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public class PermanenciaLocal
    {
        public int CodigoPermanencia { get; set; }

        public int CodigoLocal { get; set; }

        public int CodigoCarga { get; set; }

        public string Descricao { get; set; }

        public double CodigoCliente { get; set; }

        public DateTime DataInicio { get; set; }

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
