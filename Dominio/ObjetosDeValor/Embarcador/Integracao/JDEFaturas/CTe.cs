using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.JDEFaturas
{
    public class CTe
    {
        [JsonProperty(PropertyName = "cte")]
        public string ChaveCTe { get; set; }

        [JsonProperty(PropertyName = "valor")]
        public decimal Valor { get; set; }
    }
}
