using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Globus
{
    public class Cliente
    {
        [JsonProperty(PropertyName = "email", Required = Required.Default)]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "razaoSocial", Required = Required.Default)]
        public string RazaoSocial { get; set; }

        [JsonProperty(PropertyName = "nomeFantasia", Required = Required.Default)]
        public string NomeFantasia { get; set; }

        [JsonProperty(PropertyName = "documentos", Required = Required.Default)]
        public List<Documento> Documentos { get; set; }

        [JsonProperty(PropertyName = "endereco", Required = Required.Default)]
        public Endereco Endereco { get; set; }

        [JsonProperty(PropertyName = "telefone", Required = Required.Default)]
        public string Telefone { get; set; }

        [JsonProperty(PropertyName = "contaPlanoFinanceiro", Required = Required.Default)]
        public string ContaPlanoFinanceiro { get; set; }

        [JsonProperty(PropertyName = "contasContabeis", Required = Required.Default)]
        public string ContasContabeis { get; set; }
    }
}
