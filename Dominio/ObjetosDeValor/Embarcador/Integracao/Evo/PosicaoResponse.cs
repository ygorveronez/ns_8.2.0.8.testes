using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Evo
{
    public class Item
    {
        [JsonProperty(PropertyName = "nm")]
        public string Placa { get; set; }
        [JsonProperty(PropertyName = "cls")]
        public int Cls { get; set; }
        /// <summary>
        /// Id do Item
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public int IdItem { get; set; }
        [JsonProperty(PropertyName = "mu")]
        public int Mu { get; set; }
        [JsonProperty(PropertyName = "pos")]
        public Pos Posicao { get; set; }
        [JsonProperty(PropertyName = "uacl")]
        public int Uacl { get; set; }
    }

    public class Pos
    {
        /// <summary>
        /// Hora (UTC)
        /// </summary>
        [JsonProperty(PropertyName = "t")]
        public int HoraUTC { get; set; }
        [JsonProperty(PropertyName = "f")]
        public int F { get; set; }
        [JsonProperty(PropertyName = "lc")]
        public int Lc { get; set; }

        /// <summary>
        /// Latitude
        /// </summary>
        [JsonProperty(PropertyName = "y")]
        public double Latitude { get; set; }

        /// <summary>
        /// Longitude
        /// </summary>
        [JsonProperty(PropertyName = "x")]
        public double Longitude { get; set; }
        
        /// <summary>
        /// curso
        /// </summary>
        public int c { get; set; }
        /// <summary>
        /// Altitude
        /// </summary>
        [JsonProperty(PropertyName = "z")]
        public int Altitude { get; set; }

        /// <summary>
        /// Velocidade
        /// </summary>
        [JsonProperty(PropertyName = "s")]
        public int Velocidade { get; set; }

        /// <summary>
        /// Contagem de satélites
        /// </summary>
        [JsonProperty(PropertyName = "sc")]
        public int ContagemSatelite { get; set; }
    }

    public class PosicaoResponse
    {
        [JsonProperty(PropertyName = "item")]
        public Item Item { get; set; }
        [JsonProperty(PropertyName = "flags")]
        public int Flags { get; set; }
    }

}
