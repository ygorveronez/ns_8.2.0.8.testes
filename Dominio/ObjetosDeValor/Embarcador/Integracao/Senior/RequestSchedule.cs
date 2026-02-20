using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Senior
{
    public class RequestSchedule
    {
        [JsonProperty(PropertyName = "key", Required = Required.Default)]
        public string Key { get; set; }

        [JsonProperty(PropertyName = "gbCompany", Required = Required.Default)]
        public Company Company { get; set; }

        [JsonProperty(PropertyName = "visitor", Required = Required.Default)]
        public Visitor Visitor { get; set; }

        [JsonProperty(PropertyName = "previsionDate", Required = Required.Default)]
        public string DataPrevisao { get; set; }

        [JsonProperty(PropertyName = "previsionTime", Required = Required.Default)]
        public string HoraPrevisao { get; set; }

        [JsonProperty(PropertyName = "schedulingSequence", Required = Required.Default)]
        public int Sequencia { get; set; }

        [JsonProperty(PropertyName = "visitType", Required = Required.Default)]
        public int TipoVisita { get; set; }

        [JsonProperty(PropertyName = "motivationCode", Required = Required.Default)]
        public string CodigoMotivacao { get; set; }

        [JsonProperty(PropertyName = "expirationDate", Required = Required.Default)]
        public string DataValidade { get; set; }

        [JsonProperty(PropertyName = "expirationTime", Required = Required.Default)]
        public string HoraValidade { get; set; }

    }
}
