using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.RetornoOferta
{
    public class EquipeVeiculo
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("phoneNumber")]
        public string Telefone { get; set; }

        [JsonProperty("individualEntity")]
        public EntidadeIndividual EntidadeIndividual { get; set; }

        [JsonProperty("transportOperatorType")]
        public Tipo TipoOperadorTransporte { get; set; }

        [JsonProperty("address")]
        public List<Endereco> Enderecos { get; set; }

        [JsonProperty("documents")]
        public List<DocumentoEquipeVeiculo> Documentos { get; set; }
    }
}
