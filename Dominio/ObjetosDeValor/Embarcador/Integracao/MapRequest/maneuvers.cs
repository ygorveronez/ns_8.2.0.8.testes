using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.MapRequest
{
    public class maneuvers
    {
        public int index { get; set; }

        public string narrative { get; set; }

        public string iconUrl { get; set; }

        public decimal distance { get; set; }

        public int time { get; set; }

        List<string> streets { get; set; }

        public decimal Southeast { get; set; }

        public latLng startPoint { get; set; }
    }
}
