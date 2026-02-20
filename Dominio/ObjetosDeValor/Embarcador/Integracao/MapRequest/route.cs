using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.MapRequest
{
    public class route
    {
        public decimal distance { get; set; }
        public List<int> locationSequence { get; set; }
        public List<location> locations { get; set; }
        public List<legs> legs { get; set; }
    }

    public class Route
    {
        public route route { get; set; }
    }
}
