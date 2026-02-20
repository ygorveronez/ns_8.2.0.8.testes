using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento
{
    public class webhook
    {
        public string id { get; set; }
        public string externalId { get; set; }
        public string status { get; set; }
        public List<string> events { get; set; }
        public webhookEndpoint endpoint { get; set; }

        public class webhookEndpoint
        {
            public string url { get; set; }
            public List<List<string>> headers { get; set; }
        }
    }
}