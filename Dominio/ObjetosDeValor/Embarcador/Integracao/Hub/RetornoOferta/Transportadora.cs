using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.RetornoOferta
{
    public class Transportadora
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("phoneNumber")]
        public string Telefone { get; set; }

        [JsonProperty("entityIndividual")]
        public EntidadeIndividual EntidadeIndividual { get; set; }

        [JsonProperty("entityLegal")]
        public EntidadeLegal EntidadeLegal { get; set; }

        [JsonProperty("documents")]
        public List<Documento> Documentos { get; set; }

        [JsonProperty("addresses")]
        public List<Endereco> Enderecos { get; set; }
    }
}
