using System;

namespace Dominio.ObjetosDeValor.Embarcador.Fechamento
{
    public sealed class FechamentoFretePeriodo
    {
        public DateTime DataFim { get; set; }

        public DateTime DataInicio { get; set; }

        public int Periodo { get; set; }
    }
}
