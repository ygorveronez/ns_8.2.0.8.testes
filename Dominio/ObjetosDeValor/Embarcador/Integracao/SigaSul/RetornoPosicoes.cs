using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.SigaSul
{
    public class RetornoPosicoes
    {
        [JsonProperty(PropertyName = "pos_data_hora_gps", Required = Required.Default)]
        public string DataHora { get; set; }

        [JsonProperty(PropertyName = "pos_placa", Required = Required.Default)]
        public string Placa { get; set; }

        [JsonProperty(PropertyName = "pos_latitude", Required = Required.Default)]
        public string Latitude { get; set; }

        [JsonProperty(PropertyName = "pos_longitude", Required = Required.Default)]
        public string Longitude { get; set; }

        [JsonProperty(PropertyName = "pos_velocidade", Required = Required.Default)]
        public string Velociadade { get; set; }

        [JsonProperty(PropertyName = "pos_equip_id", Required = Required.Default)]
        public string IDEquipamento { get; set; }

        [JsonProperty(PropertyName = "pos_equip_modelo", Required = Required.Default)]
        public string Modelo { get; set; }

        [JsonProperty(PropertyName = "pos_ignicao", Required = Required.Default)]
        public bool Ignicao { get; set; }

    }

}
