using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda
{
    public partial class Entidade
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("phoneNumber")]
        public string Telefone { get; set; }

        [JsonProperty("individualEntity")]
        public EntidadeIndividual PessoaFisica { get; set; }

        [JsonProperty("legalEntity")]
        public EntidadeLegal PessoaJuridica { get; set; }

        [JsonProperty("addresses")]
        public List<Endereco> Enderecos { get; set; }

        [JsonProperty("documents")]
        public List<Documento> Documentos { get; set; }
    }
}
