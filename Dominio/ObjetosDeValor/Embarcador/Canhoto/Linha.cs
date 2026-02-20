using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Canhoto
{
    public class Linha
    {
        public string LineText { get; set; }
        public List<Palavra> Words { get; set; }
        public double MaxHeight { get; set; }
        public double MinTop { get; set; }
    }
}
