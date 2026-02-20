using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.KMM
{
    public class RNTRC
    {
        [JsonProperty(PropertyName = "numero", Required = Required.Default)]
        public string Numero { get; set; }

        [JsonProperty(PropertyName = "data_emissao", Required = Required.Default)]
        public string DataEmissao { get; set; }

        [JsonProperty(PropertyName = "data_vencimento", Required = Required.Default)]
        public string DataVencimento { get; set; }

        [JsonProperty(PropertyName = "tipo_transportador", Required = Required.Default)]
        public string TipoTransportador { get; set; }
    }
}
