using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy
{
    public class Travel
    {
        public string _id { get; set; }
        public Destination destination { get; set; }
        public Origin origin { get; set; }
        public List<Stopovers> stopovers { get; set; }
    }
}
