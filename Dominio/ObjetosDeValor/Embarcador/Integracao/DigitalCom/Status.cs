using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.DigitalCom
{
    public class Status
    {
        [JsonProperty("id")]
        public int Codigo { get; set; }

        [JsonProperty("messageKey")]
        public string ChaveMensagem { get; set; }

        [JsonProperty("tipoMensagem")]
        public string TipoMensagem { get; set; }

        [JsonProperty("descricao")]
        public string Descricao { get; set; }
    }
}
