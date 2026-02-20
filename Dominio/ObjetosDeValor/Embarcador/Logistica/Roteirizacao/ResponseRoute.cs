using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao
{
    public class ResponseRoute
    {
        public List<ResposeLeg> Legs { get; set; }
        public string Weight_name { get; set; }
        public double Weight { get; set; }
        public double Distance { get; set; }
        public double Duration { get; set; }
        public string Geometry { get; set; }
    }
}
