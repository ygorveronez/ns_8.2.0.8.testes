using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy
{
    public class Itens
    {
        public Titulo title { get; set; }
        public Titulo subtitle { get; set; }
        public List<string> value { get; set; }
        public string step { get; set; }
        public bool galleryPhoto { get; set; }
        public int limitPhoto { get; set; }
        public bool required { get; set; }
        public Validator validator { get; set; }
    }
}
