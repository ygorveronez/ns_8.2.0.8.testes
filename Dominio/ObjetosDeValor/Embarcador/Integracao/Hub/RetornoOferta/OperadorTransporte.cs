using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.RetornoOferta
{
    public class OperadorTransporte
    {
        [JsonProperty("individualEntity")]
        public EntidadeIndividual EntidadeIndividual { get; set; }

        [JsonProperty("transportOperatorType")]
        public TipoOperadorTransporte TipoOperadorTransporte { get; set; }

        [JsonProperty("addresses")]
        public List<Endereco> Enderecos { get; set; }

        [JsonProperty("documents")]
        public List<Documento> Documentos { get; set; }
    }
}
