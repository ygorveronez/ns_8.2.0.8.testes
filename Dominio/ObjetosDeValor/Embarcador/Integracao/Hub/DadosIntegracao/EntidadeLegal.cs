using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Hub
{
    public class EntidadeLegal
    {
        [JsonProperty("businessName")]
        public string RazaoSocial { get; set; }

        [JsonProperty("tradeName")]
        public string NomeFantasia { get; set; }

        [JsonProperty("tradePhoneNumber")]
        public string TelefoneEmpresa { get; set; }
    }
}
