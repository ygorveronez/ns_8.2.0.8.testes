using System;
using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Magalog
{
    public class RetornoOcorrencia
    {
        public long idMessage { get; set; }

        public DateTime createAt { get; set; }

        public string message { get; set; }

        [JsonProperty("object")]
        public Object Object { get; set; }

        public CTe cte { get; set; }
    }
}
