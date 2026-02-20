using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.TorreControle
{
    public class ResumoConsultaQualidadeEntrega
    {
        public int QtdDisponivelParaConsulta { get; set; }
        public int QtdNaoDisponivelParaConsulta { get; set; }
        public int QtdTotalParaConsulta { get; set; }
    }
}
