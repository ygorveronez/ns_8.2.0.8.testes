using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Hub
{
    public class Matriz
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("phoneNumber")]
        public string NumeroTelefone { get; set; }

        [JsonProperty("individualEntity")]
        public EntidadeIndividual EntidadeIndividual { get; set; }

        [JsonProperty("legalEntity")]
        public EntidadeLegal PessoaJuridica { get; set; }

        [JsonProperty("addresses")]
        public List<Endereco> Enderecos { get; set; }

        [JsonProperty("documents")]
        public List<Documento> Documentos { get; set; }
    }
}
