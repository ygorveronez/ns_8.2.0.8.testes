using System;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public sealed class ParametroCalcularValorDiariaAutomatica
    {

        public int CodigoCarga { get; set; }

        public DateTime DataInicio { get; set; }

        public int Minutos { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.LocalFreeTime LocalFreeTime { get; set; }
    }
}
