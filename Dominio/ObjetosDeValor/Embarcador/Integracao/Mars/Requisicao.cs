using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Mars
{
    public class Requisicao
    {
        [JsonProperty("dtCargoNumber")]
        public string CodigoCargaEmbarcador { get; set; }

        [JsonProperty("protNumber")]
        public string CodigoIntegracao { get; set; }

        [JsonProperty("totalFreightValue")]
        public string ValorFretePagar { get; set; }

        [JsonProperty("costType")]
        public string TipoCusto { get; set; }

        [JsonProperty("documents")]
        public List<Documento> Documentos { get; set; }
    }
}
