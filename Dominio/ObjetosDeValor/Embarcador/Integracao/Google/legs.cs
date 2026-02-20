using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Google
{
    public class legs
    {
        public descritivo distance { get; set; }
        public descritivo duration { get; set; }
        public string end_address { get; set; }
        public location end_location { get; set; }
        public string start_address { get; set; }
        public location start_location { get; set; }

        public List<steps> steps { get; set; }
    }
}
