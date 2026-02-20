using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.MapRequest
{
    public class legs
    {
        public decimal distance { get; set; }

        public int time { get; set; }

        public List<maneuvers> maneuvers { get; set; }
    }
}
