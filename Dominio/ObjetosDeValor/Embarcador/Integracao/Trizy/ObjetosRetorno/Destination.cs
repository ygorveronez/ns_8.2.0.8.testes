using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy
{
    public class Destination
    {
        public string _id { get; set; }
        public string externalId { get; set; }
        public Client client { get; set; }
        public List<Documents> documents { get; set; }

    }
}
