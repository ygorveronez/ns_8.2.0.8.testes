using Newtonsoft.Json;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Hub
{
    public class EntidadeIndividual
    {
        [JsonProperty("fullName")]
        public string NomeCompleto { get; set; }

        [JsonProperty("dateOfBirth")]
        public DateTime? DataNascimento { get; set; }

        [JsonProperty("mobilePhoneNumber")]
        public string TelefoneCelular { get; set; }
    }
}
