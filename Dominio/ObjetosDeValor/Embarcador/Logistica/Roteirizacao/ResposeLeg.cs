using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao
{
    public class ResposeLeg
    {
        public List<Step> Steps { get; set; }
        public double Weight { get; set; }
        public double Distance { get; set; }
        public string Summary { get; set; }
        public double Duration { get; set; }
    }
}
